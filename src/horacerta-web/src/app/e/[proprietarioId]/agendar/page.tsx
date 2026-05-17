'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { useParams } from 'next/navigation';
import { App, Button, Card, Form, Input, Radio, Result, Steps, Typography } from 'antd';
import { iniciarAgendamentoUseCase } from '@/agendamento/application';
import { criarClienteUseCase } from '@/cliente/application';
import { salvarSessaoCliente } from '@/cliente/application/sessao-cliente';
import { useClienteSessaoStore } from '@/cliente/presentation/stores/cliente-sessao.store';
import { listarProcedimentosPublicoUseCase } from '@/procedimento/application';
import type { ProcedimentoDto } from '@/procedimento/application/dtos/procedimento.dto';
import { listarSlotsDisponiveisUseCase } from '@/slot-horario/application';
import type { SlotHorarioDto } from '@/slot-horario/application/dtos/slot-horario.dto';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { formatarDataHora, formatarMoeda } from '@/shared/presentation/format';

export default function AgendarPage() {
  const params = useParams<{ proprietarioId: string }>();
  const proprietarioId = params.proprietarioId;
  const { message } = App.useApp();
  const setSessao = useClienteSessaoStore((s) => s.setSessao);

  const [step, setStep] = useState(0);
  const [clienteId, setClienteId] = useState<string | null>(null);
  const [procedimentos, setProcedimentos] = useState<ProcedimentoDto[]>([]);
  const [slots, setSlots] = useState<SlotHorarioDto[]>([]);
  const [procedimentoId, setProcedimentoId] = useState<string>();
  const [slotId, setSlotId] = useState<string>();
  const [loading, setLoading] = useState(false);
  const [concluido, setConcluido] = useState(false);

  useEffect(() => {
    listarProcedimentosPublicoUseCase.execute(proprietarioId).then(setProcedimentos).catch(() => {
      message.error('Não foi possível carregar procedimentos');
    });
  }, [proprietarioId, message]);

  useEffect(() => {
    if (step === 2) {
      listarSlotsDisponiveisUseCase.execute(proprietarioId).then(setSlots).catch(() => {
        message.error('Não foi possível carregar horários');
      });
    }
  }, [step, proprietarioId, message]);

  const cadastrarCliente = async (values: { nome: string; telefone: string }) => {
    setLoading(true);
    try {
      const cliente = await criarClienteUseCase.execute(values);
      await salvarSessaoCliente({ clienteId: cliente.id, proprietarioId });
      setSessao(cliente.id, proprietarioId);
      setClienteId(cliente.id);
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

  if (concluido) {
    return (
      <main style={{ padding: 24, maxWidth: 480, margin: '0 auto' }}>
        <Result
          status="success"
          title="Agendamento enviado!"
          subTitle="Seu pedido está pendente de confirmação pelo estabelecimento. Você receberá um lembrete antes do horário."
          extra={[
            <Link key="meus" href={`/e/${proprietarioId}/meus-agendamentos`}>
              <Button type="primary">Meus agendamentos</Button>
            </Link>,
            <Link key="home" href={`/e/${proprietarioId}`}>
              <Button>Voltar</Button>
            </Link>,
          ]}
        />
      </main>
    );
  }

  return (
    <main style={{ padding: 24, maxWidth: 520, margin: '0 auto' }}>
      <Typography.Title level={3}>Agendar</Typography.Title>
      <Steps
        current={step}
        style={{ marginBottom: 24 }}
        items={[
          { title: 'Seus dados' },
          { title: 'Procedimento' },
          { title: 'Horário' },
        ]}
      />

      {step === 0 && (
        <Card>
          <Form layout="vertical" onFinish={cadastrarCliente}>
            <Form.Item label="Nome" name="nome" rules={[{ required: true }]}>
              <Input />
            </Form.Item>
            <Form.Item label="Telefone" name="telefone" rules={[{ required: true }]}>
              <Input />
            </Form.Item>
            <Button type="primary" htmlType="submit" block loading={loading}>
              Continuar
            </Button>
          </Form>
        </Card>
      )}

      {step === 1 && (
        <Card>
          <Radio.Group
            style={{ width: '100%' }}
            value={procedimentoId}
            onChange={(e) => setProcedimentoId(e.target.value)}
          >
            {procedimentos.map((p) => (
              <Radio key={p.id} value={p.id} style={{ display: 'block', marginBottom: 12 }}>
                {p.nome} — {formatarMoeda(p.valor)} ({p.tempoEstimadoMinutos} min)
              </Radio>
            ))}
          </Radio.Group>
          <Button
            type="primary"
            block
            style={{ marginTop: 16 }}
            disabled={!procedimentoId}
            onClick={() => setStep(2)}
          >
            Continuar
          </Button>
        </Card>
      )}

      {step === 2 && (
        <Card>
          <Radio.Group
            style={{ width: '100%' }}
            value={slotId}
            onChange={(e) => setSlotId(e.target.value)}
          >
            {slots.map((s) => (
              <Radio key={s.id} value={s.id} style={{ display: 'block', marginBottom: 12 }}>
                {formatarDataHora(s.inicio)}
              </Radio>
            ))}
          </Radio.Group>
          <Button
            type="primary"
            block
            style={{ marginTop: 16 }}
            disabled={!slotId}
            loading={loading}
            onClick={confirmarAgendamento}
          >
            Confirmar agendamento
          </Button>
        </Card>
      )}

      <Typography.Paragraph style={{ marginTop: 16 }}>
        <Link href={`/e/${proprietarioId}`}>Voltar</Link>
      </Typography.Paragraph>
    </main>
  );
}
