'use client';

import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import dayjs from 'dayjs';
import isoWeek from 'dayjs/plugin/isoWeek';
import { App, Button, Card, Empty, Form, Input, Radio, Result, Skeleton, Space, Steps, Typography } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import { iniciarAgendamentoUseCase } from '@/agendamento/application';
import { criarClienteUseCase } from '@/cliente/application';
import { obterClienteUseCase } from '@/cliente/application/obter-cliente';
import { salvarSessaoCliente } from '@/cliente/application/sessao-cliente';
import { useClienteEstabelecimento } from '@/cliente/presentation/hooks/use-cliente-estabelecimento';
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
  const { ready, clienteId: sessaoClienteId, semSessao, sair } =
    useClienteEstabelecimento(proprietarioId);
  const setSessaoStore = useClienteSessaoStore((s) => s.setSessao);

  const [step, setStep] = useState(0);
  const [clienteId, setClienteId] = useState<string | null>(null);
  const [clienteNome, setClienteNome] = useState<string>();
  const [mostrarFormIdentificacao, setMostrarFormIdentificacao] = useState(false);
  const [procedimentos, setProcedimentos] = useState<ProcedimentoDto[]>([]);
  const [slots, setSlots] = useState<SlotHorarioDto[]>([]);
  const [procedimentoId, setProcedimentoId] = useState<string>();
  const [slotId, setSlotId] = useState<string>();
  const [weekStart, setWeekStart] = useState(() => dayjs().startOf('isoWeek'));
  const [loadingProc, setLoadingProc] = useState(true);
  const [loadingSlots, setLoadingSlots] = useState(false);
  const [loading, setLoading] = useState(false);
  const [concluido, setConcluido] = useState(false);
  const sessaoAutoAvancada = useRef(false);

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
    if (ready && semSessao) {
      setStep(0);
      setClienteId(null);
      setClienteNome(undefined);
      setMostrarFormIdentificacao(true);
    }
  }, [ready, semSessao]);

  useEffect(() => {
    if (!ready || semSessao || !sessaoClienteId || sessaoAutoAvancada.current) return;
    sessaoAutoAvancada.current = true;
    setClienteId(sessaoClienteId);
    void obterClienteUseCase
      .execute(sessaoClienteId)
      .then((c) => setClienteNome(c.nome))
      .catch(() => undefined);
    setStep(1);
    setMostrarFormIdentificacao(false);
  }, [ready, semSessao, sessaoClienteId]);

  const carregarSlots = useCallback(async () => {
    setLoadingSlots(true);
    try {
      const data = await listarSlotsDisponiveisUseCase.execute(proprietarioId);
      setSlots(data);
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
      setSessaoStore(cliente.id, proprietarioId);
      setClienteId(cliente.id);
      setClienteNome(cliente.nome);
      setMostrarFormIdentificacao(false);
      setStep(1);
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setLoading(false);
    }
  };

  const continuarComSessao = () => {
    if (clienteId) setStep(1);
  };

  const trocarIdentidade = async () => {
    sessaoAutoAvancada.current = false;
    await sair();
    setClienteId(null);
    setClienteNome(undefined);
    setMostrarFormIdentificacao(true);
    setStep(0);
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

  const voltar = () => {
    setStep((s) => {
      const prev = Math.max(0, s - 1);
      if (prev === 0 && clienteId && !mostrarFormIdentificacao) {
        setMostrarFormIdentificacao(false);
      }
      return prev;
    });
  };

  if (concluido) {
    return (
      <Card className="hc-card-elevated" variant="borderless">
        <Result
          status="success"
          title="Agendamento enviado!"
          subTitle="Seu pedido está pendente de confirmação pelo estabelecimento. Você receberá um lembrete antes do horário."
          extra={[
            <Button
              key="meus"
              type="primary"
              size="large"
              block
              href={`/e/${proprietarioId}/meus-agendamentos`}
            >
              Meus agendamentos
            </Button>,
            <Button key="home" block href={`/e/${proprietarioId}`}>
              Voltar ao início
            </Button>,
          ]}
        />
      </Card>
    );
  }

  if (!ready) {
    return <Skeleton active paragraph={{ rows: 8 }} />;
  }

  const passoIdentificacao =
    clienteId && !mostrarFormIdentificacao ? (
      <Card className="hc-card-elevated" variant="borderless">
        <Typography.Title level={5} style={{ marginTop: 0 }}>
          Olá, {clienteNome ?? 'visitante'}!
        </Typography.Title>
        <Typography.Paragraph type="secondary">
          Você já está identificado neste estabelecimento. Continue para escolher o serviço.
        </Typography.Paragraph>
        <Space direction="vertical" style={{ width: '100%' }}>
          <Button type="primary" block size="large" onClick={continuarComSessao}>
            Continuar
          </Button>
          <Button block size="large" onClick={trocarIdentidade}>
            Não sou eu — trocar dados
          </Button>
        </Space>
      </Card>
    ) : (
      <Card className="hc-card-elevated" variant="borderless">
        <Form layout="vertical" onFinish={cadastrarCliente}>
          <Form.Item label="Nome" name="nome" rules={[{ required: true, message: 'Informe seu nome' }]}>
            <Input size="large" prefix={<UserOutlined />} />
          </Form.Item>
          <Form.Item
            label="Telefone"
            name="telefone"
            rules={[
              { required: true, message: 'Informe seu telefone' },
              {
                pattern: /^[\d\s()+-]{10,}$/,
                message: 'Telefone inválido (mínimo 10 dígitos)',
              },
            ]}
          >
            <Input size="large" inputMode="tel" placeholder="(11) 99999-9999" />
          </Form.Item>
          <Button type="primary" htmlType="submit" block size="large" loading={loading}>
            Continuar
          </Button>
        </Form>
      </Card>
    );

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

      {step === 0 && passoIdentificacao}

      {step === 1 && (
        <Card className="hc-card-elevated" variant="borderless">
          {clienteNome ? (
            <Typography.Paragraph type="secondary" style={{ marginBottom: 16 }}>
              Agendando como <strong>{clienteNome}</strong>
            </Typography.Paragraph>
          ) : null}
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
            <Button block size="large" onClick={voltar}>
              Voltar
            </Button>
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
