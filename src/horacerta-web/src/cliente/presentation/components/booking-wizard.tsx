'use client';

import { useCallback, useEffect, useMemo, useState } from 'react';
import Link from 'next/link';
import dayjs from 'dayjs';
import isoWeek from 'dayjs/plugin/isoWeek';
import { App, Button, Card, Empty, Form, Input, Radio, Result, Skeleton, Space, Steps } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import { iniciarAgendamentoUseCase } from '@/agendamento/application';
import { criarClienteUseCase } from '@/cliente/application';
import { obterClienteUseCase } from '@/cliente/application/obter-cliente';
import { salvarSessaoCliente, obterSessaoCliente } from '@/cliente/application/sessao-cliente';
import { useClienteSessaoStore } from '@/cliente/presentation/stores/cliente-sessao.store';
import { listarProcedimentosPublicoUseCase } from '@/procedimento/application';
import type { ProcedimentoDto } from '@/procedimento/application/dtos/procedimento.dto';
import { listarSlotsDisponiveisUseCase } from '@/slot-horario/application';
import type { SlotHorarioDto } from '@/slot-horario/application/dtos/slot-horario.dto';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { WeekTimeGrid } from '@/shared/presentation/components/week-time-grid';
import { formatarMoeda } from '@/shared/presentation/format';
import { BookingSummary } from './booking-summary';

dayjs.extend(isoWeek);

type BookingWizardProps = {
  proprietarioId: string;
};

export function BookingWizard({ proprietarioId }: BookingWizardProps) {
  const { message } = App.useApp();
  const sessaoClienteId = useClienteSessaoStore((s) => s.clienteId);
  const sessaoProprietarioId = useClienteSessaoStore((s) => s.proprietarioId);
  const setSessao = useClienteSessaoStore((s) => s.setSessao);

  const [inicializado, setInicializado] = useState(false);
  const [step, setStep] = useState(0);
  const [clienteId, setClienteId] = useState<string | null>(null);
  const [clienteNome, setClienteNome] = useState<string>();
  const [procedimentos, setProcedimentos] = useState<ProcedimentoDto[]>([]);
  const [slots, setSlots] = useState<SlotHorarioDto[]>([]);
  const [procedimentoId, setProcedimentoId] = useState<string>();
  const [slotId, setSlotId] = useState<string>();
  const [weekStart, setWeekStart] = useState(() => dayjs().startOf('isoWeek'));
  const [loadingProc, setLoadingProc] = useState(true);
  const [loadingSlots, setLoadingSlots] = useState(false);
  const [loading, setLoading] = useState(false);
  const [concluido, setConcluido] = useState(false);

  const procedimento = useMemo(
    () => procedimentos.find((p) => p.id === procedimentoId),
    [procedimentos, procedimentoId],
  );
  const slot = useMemo(() => slots.find((s) => s.id === slotId), [slots, slotId]);

  useEffect(() => {
    listarProcedimentosPublicoUseCase
      .execute(proprietarioId)
      .then(setProcedimentos)
      .catch(() => message.error('Não foi possível carregar procedimentos'))
      .finally(() => setLoadingProc(false));
  }, [proprietarioId, message]);

  useEffect(() => {
    let cancelled = false;
    const init = async () => {
      let cid = sessaoClienteId;
      let pid = sessaoProprietarioId;
      if (!cid) {
        const sessao = await obterSessaoCliente();
        if (sessao) {
          setSessao(sessao.clienteId, sessao.proprietarioId);
          cid = sessao.clienteId;
          pid = sessao.proprietarioId;
        }
      }
      if (!cancelled && cid && pid === proprietarioId) {
        setClienteId(cid);
        try {
          const cliente = await obterClienteUseCase.execute(cid);
          setClienteNome(cliente.nome);
        } catch {
          /* nome opcional */
        }
        setStep(1);
      }
      if (!cancelled) setInicializado(true);
    };
    void init();
    return () => {
      cancelled = true;
    };
  }, [proprietarioId, sessaoClienteId, sessaoProprietarioId, setSessao]);

  const carregarSlots = useCallback(async () => {
    setLoadingSlots(true);
    try {
      const data = await listarSlotsDisponiveisUseCase.execute(proprietarioId);
      setSlots(data);
      if (data.length > 0) {
        setWeekStart(dayjs(data[0].inicio).startOf('isoWeek'));
      }
    } catch {
      message.error('Não foi possível carregar horários');
    } finally {
      setLoadingSlots(false);
    }
  }, [proprietarioId, message]);

  useEffect(() => {
    if (step === 2) void carregarSlots();
  }, [step, carregarSlots]);

  const cadastrarCliente = async (values: { nome: string; telefone: string }) => {
    setLoading(true);
    try {
      const cliente = await criarClienteUseCase.execute(values);
      await salvarSessaoCliente({ clienteId: cliente.id, proprietarioId });
      setSessao(cliente.id, proprietarioId);
      setClienteId(cliente.id);
      setClienteNome(cliente.nome);
      setStep(1);
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setLoading(false);
    }
  };

  const confirmarAgendamento = async () => {
    if (!clienteId || !procedimentoId || !slotId) return;
    setLoading(true);
    try {
      await iniciarAgendamentoUseCase.execute({
        proprietarioId,
        clienteId,
        procedimentoId,
        slotHorarioId: slotId,
      });
      setConcluido(true);
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setLoading(false);
    }
  };

  const voltar = () => setStep((s) => Math.max(0, s - 1));

  if (concluido) {
    return (
      <Card className="hc-card-elevated" variant="borderless">
        <Result
          status="success"
          title="Agendamento enviado!"
          subTitle="Seu pedido está pendente de confirmação pelo estabelecimento. Você receberá um lembrete antes do horário."
          extra={[
            <Link key="meus" href={`/e/${proprietarioId}/meus-agendamentos`}>
              <Button type="primary" size="large" block>
                Meus agendamentos
              </Button>
            </Link>,
            <Link key="home" href={`/e/${proprietarioId}`}>
              <Button block>Voltar ao início</Button>
            </Link>,
          ]}
        />
      </Card>
    );
  }

  if (!inicializado) {
    return <Skeleton active paragraph={{ rows: 8 }} />;
  }

  return (
    <>
      <Steps
        className="hc-wizard-steps"
        current={step}
        items={[
          { title: 'Identificação' },
          { title: 'Serviço' },
          { title: 'Horário' },
          { title: 'Revisão' },
        ]}
      />

      {step === 0 && (
        <Card className="hc-card-elevated" variant="borderless">
          <Form layout="vertical" onFinish={cadastrarCliente}>
            <Form.Item label="Nome" name="nome" rules={[{ required: true }]}>
              <Input size="large" prefix={<UserOutlined />} />
            </Form.Item>
            <Form.Item label="Telefone" name="telefone" rules={[{ required: true }]}>
              <Input size="large" inputMode="tel" />
            </Form.Item>
            <Button type="primary" htmlType="submit" block size="large" loading={loading}>
              Continuar
            </Button>
          </Form>
        </Card>
      )}

      {step === 1 && (
        <Card className="hc-card-elevated" variant="borderless">
          {loadingProc ? (
            <Skeleton active />
          ) : procedimentos.length === 0 ? (
            <Empty description="Nenhum serviço disponível no momento" />
          ) : (
            <Radio.Group
              style={{ width: '100%' }}
              value={procedimentoId}
              onChange={(e) => setProcedimentoId(e.target.value)}
            >
              {procedimentos.map((p) => (
                <Radio key={p.id} value={p.id} className="hc-service-option">
                  <strong>{p.nome}</strong>
                  <br />
                  <span style={{ color: 'var(--hc-text-muted)', fontSize: '0.9rem' }}>
                    {formatarMoeda(p.valor)} · {p.tempoEstimadoMinutos} min
                  </span>
                </Radio>
              ))}
            </Radio.Group>
          )}
          <Space style={{ width: '100%', marginTop: 16 }} direction="vertical">
            <Button
              type="primary"
              block
              size="large"
              disabled={!procedimentoId}
              onClick={() => setStep(2)}
            >
              Continuar
            </Button>
            {clienteId ? (
              <Button block size="large" onClick={voltar}>
                Voltar
              </Button>
            ) : null}
          </Space>
        </Card>
      )}

      {step === 2 && (
        <Card className="hc-card-elevated hc-card-elevated--wide" variant="borderless">
          {loadingSlots ? (
            <Skeleton active paragraph={{ rows: 10 }} />
          ) : slots.length === 0 ? (
            <Empty description="Nenhum horário disponível. Tente outra semana ou volte mais tarde." />
          ) : (
            <WeekTimeGrid
              slots={slots}
              weekStart={weekStart}
              onWeekChange={setWeekStart}
              selectedId={slotId}
              onSelect={setSlotId}
              selectable
              emptyText="Nenhum horário nesta semana"
            />
          )}
          <Space style={{ width: '100%', marginTop: 16 }} direction="vertical">
            <Button
              type="primary"
              block
              size="large"
              disabled={!slotId}
              onClick={() => setStep(3)}
            >
              Continuar
            </Button>
            <Button block size="large" onClick={voltar}>
              Voltar
            </Button>
          </Space>
        </Card>
      )}

      {step === 3 && procedimento && slot && (
        <Card className="hc-card-elevated" variant="borderless">
          <BookingSummary
            procedimentoNome={procedimento.nome}
            procedimentoValor={procedimento.valor}
            procedimentoMinutos={procedimento.tempoEstimadoMinutos}
            slotInicio={slot.inicio}
            clienteNome={clienteNome}
          />
          <Space style={{ width: '100%', marginTop: 16 }} direction="vertical">
            <Button
              type="primary"
              block
              size="large"
              loading={loading}
              onClick={confirmarAgendamento}
            >
              Confirmar agendamento
            </Button>
            <Button block size="large" onClick={voltar}>
              Voltar
            </Button>
          </Space>
        </Card>
      )}
    </>
  );
}
