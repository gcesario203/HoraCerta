'use client';

import { useCallback, useEffect, useState } from 'react';
import {
  App,
  Button,
  Card,
  Descriptions,
  Drawer,
  InputNumber,
  Modal,
  Popconfirm,
  Select,
  Space,
  Table,
  Tag,
} from 'antd';
import {
  cancelarAgendamentoUseCase,
  confirmarAgendamentoUseCase,
  listarAgendamentosProprietarioUseCase,
  remarcarAgendamentoUseCase,
} from '@/agendamento/application';
import type { AgendamentoListagemDto } from '@/agendamento/application/dtos/agendamento.dto';
import { registrarAtendimentoUseCase } from '@/atendimento/application';
import { obterAvaliacaoUseCase } from '@/avaliacao/application';
import type { AvaliacaoDto } from '@/avaliacao/application/dtos/avaliacao.dto';
import { useProprietarioPage } from '@/auth/presentation/hooks/use-proprietario-page';
import { listarSlotsDisponiveisUseCase } from '@/slot-horario/application';
import type { SlotHorarioDto } from '@/slot-horario/application/dtos/slot-horario.dto';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { PageHeader } from '@/shared/presentation/components/page-header';
import { formatarDataHora, labelEstado } from '@/shared/presentation/format';

export default function AgendamentosPage() {
  const { proprietarioId, ready, canAct } = useProprietarioPage();
  const { message } = App.useApp();
  const [lista, setLista] = useState<AgendamentoListagemDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [slots, setSlots] = useState<SlotHorarioDto[]>([]);
  const [remarcarAg, setRemarcarAg] = useState<AgendamentoListagemDto | null>(null);
  const [novoSlot, setNovoSlot] = useState<string>();
  const [remarcando, setRemarcando] = useState(false);
  const [avaliacao, setAvaliacao] = useState<AvaliacaoDto | null>(null);
  const [drawerAg, setDrawerAg] = useState<AgendamentoListagemDto | null>(null);
  const [valorAtendimento, setValorAtendimento] = useState<number | undefined>();
  const [registrandoAtendimento, setRegistrandoAtendimento] = useState(false);

  const carregar = useCallback(async () => {
    if (!ready || !canAct || !proprietarioId) {
      if (ready && !proprietarioId) setLoading(false);
      return;
    }
    setLoading(true);
    try {
      const data = await listarAgendamentosProprietarioUseCase.execute(proprietarioId);
      setLista(data);
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setLoading(false);
    }
  }, [ready, canAct, proprietarioId, message]);

  useEffect(() => {
    void carregar();
  }, [carregar]);

  const acao = async (
    tipo: 'confirmar' | 'cancelar',
    ag: AgendamentoListagemDto,
  ) => {
    if (!canAct || !proprietarioId) return;
    try {
      const body = { proprietarioId, clienteId: ag.clienteId };
      if (tipo === 'confirmar') {
        await confirmarAgendamentoUseCase.execute(ag.agendamentoId, body);
        message.success('Agendamento confirmado');
      } else {
        await cancelarAgendamentoUseCase.execute(ag.agendamentoId, body);
        message.success('Agendamento cancelado');
      }
      await carregar();
    } catch (error) {
      message.error(extractApiMessage(error));
    }
  };

  const abrirRemarcar = async (ag: AgendamentoListagemDto) => {
    if (!canAct || !proprietarioId) return;
    setRemarcarAg(ag);
    setNovoSlot(undefined);
    try {
      const s = await listarSlotsDisponiveisUseCase.execute(proprietarioId);
      setSlots(s);
    } catch (error) {
      message.error(extractApiMessage(error));
    }
  };

  const remarcar = async () => {
    if (!proprietarioId || !remarcarAg || !novoSlot) return;
    setRemarcando(true);
    try {
      await remarcarAgendamentoUseCase.execute(remarcarAg.agendamentoId, {
        proprietarioId,
        clienteId: remarcarAg.clienteId,
        novoSlotHorarioId: novoSlot,
      });
      message.success('Agendamento remarcado');
      setRemarcarAg(null);
      setNovoSlot(undefined);
      await carregar();
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setRemarcando(false);
    }
  };

  const registrarAtendimento = async () => {
    if (!canAct || !proprietarioId || !drawerAg) return;
    setRegistrandoAtendimento(true);
    try {
      await registrarAtendimentoUseCase.execute(drawerAg.agendamentoId, {
        proprietarioId,
        clienteId: drawerAg.clienteId,
        valorNegociado: valorAtendimento ?? null,
      });
      message.success('Atendimento registrado');
      setDrawerAg(null);
      setValorAtendimento(undefined);
      await carregar();
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setRegistrandoAtendimento(false);
    }
  };

  const verAvaliacao = async (agendamentoId: string) => {
    if (!canAct || !proprietarioId) return;
    try {
      const av = await obterAvaliacaoUseCase.execute(proprietarioId, agendamentoId);
      setAvaliacao(av);
    } catch {
      message.info('Sem avaliação para este agendamento');
    }
  };

  if (!ready) {
    return null;
  }

  return (
    <>
      <PageHeader
        title="Agendamentos"
        description="Confirme, cancele, remarque ou registre o atendimento. Para alterar o estado após o serviço, use Atendimentos."
      />
      <Card className="hc-card-elevated" variant="borderless">
        <Table
          rowKey="agendamentoId"
          loading={loading}
          pagination={{ pageSize: 15, showSizeChanger: true }}
          dataSource={lista}
          locale={{ emptyText: 'Nenhum agendamento. Os pedidos dos clientes aparecerão aqui.' }}
          columns={[
            { title: 'Cliente', dataIndex: 'clienteNome' },
            { title: 'Procedimento', dataIndex: 'procedimentoNome' },
            {
              title: 'Horário',
              render: (_, r) => formatarDataHora(r.slotInicio),
            },
            {
              title: 'Estado',
              dataIndex: 'estado',
              render: (e: string) => <Tag>{labelEstado(e)}</Tag>,
            },
            {
              title: 'Ações',
              render: (_, r) => (
                <Space wrap>
                  {r.estado === 'PENDENTE' && (
                    <Popconfirm
                      title="Confirmar este agendamento?"
                      onConfirm={() => acao('confirmar', r)}
                    >
                      <Button size="small" type="primary">
                        Confirmar
                      </Button>
                    </Popconfirm>
                  )}
                  {['PENDENTE', 'CONFIRMADO'].includes(r.estado) && (
                    <Popconfirm
                      title="Cancelar este agendamento?"
                      description="Esta ação não pode ser desfeita."
                      okButtonProps={{ danger: true }}
                      onConfirm={() => acao('cancelar', r)}
                    >
                      <Button size="small" danger>
                        Cancelar
                      </Button>
                    </Popconfirm>
                  )}
                  {r.estado === 'CONFIRMADO' && (
                    <Button size="small" onClick={() => abrirRemarcar(r)}>
                      Remarcar
                    </Button>
                  )}
                  {r.estado === 'CONFIRMADO' && (
                    <Button size="small" onClick={() => setDrawerAg(r)}>
                      Registrar atendimento
                    </Button>
                  )}
                  <Button size="small" onClick={() => verAvaliacao(r.agendamentoId)}>
                    Avaliação
                  </Button>
                </Space>
              ),
            },
          ]}
        />
      </Card>

      <Modal
        title="Remarcar agendamento"
        open={!!remarcarAg}
        onOk={remarcar}
        confirmLoading={remarcando}
        onCancel={() => setRemarcarAg(null)}
        okButtonProps={{ disabled: !novoSlot }}
      >
        {remarcarAg ? (
          <Space direction="vertical" style={{ width: '100%' }}>
            <p>
              <strong>{remarcarAg.clienteNome}</strong> — {remarcarAg.procedimentoNome}
              <br />
              Horário atual: {formatarDataHora(remarcarAg.slotInicio)}
            </p>
            {slots.length === 0 ? (
              <p style={{ color: 'var(--hc-text-muted)' }}>
                Nenhum horário disponível para remarcação. Libere horários na Agenda.
              </p>
            ) : (
              <Select
                style={{ width: '100%' }}
                placeholder="Novo horário"
                value={novoSlot}
                onChange={setNovoSlot}
                options={slots.map((s) => ({
                  value: s.id,
                  label: formatarDataHora(s.inicio),
                }))}
              />
            )}
          </Space>
        ) : null}
      </Modal>

      <Drawer
        title="Registrar atendimento"
        open={!!drawerAg}
        onClose={() => {
          if (!registrandoAtendimento) {
            setDrawerAg(null);
            setValorAtendimento(undefined);
          }
        }}
      >
        {drawerAg && (
          <Space direction="vertical" style={{ width: '100%' }}>
            <p>
              {drawerAg.clienteNome} — {drawerAg.procedimentoNome}
              <br />
              {formatarDataHora(drawerAg.slotInicio)}
            </p>
            <InputNumber
              placeholder="Valor negociado (opcional)"
              style={{ width: '100%' }}
              min={0}
              prefix="R$"
              value={valorAtendimento}
              onChange={(v) => setValorAtendimento(v ?? undefined)}
            />
            <Button
              type="primary"
              loading={registrandoAtendimento}
              onClick={registrarAtendimento}
            >
              Registrar atendimento
            </Button>
          </Space>
        )}
      </Drawer>

      <Modal
        title="Avaliação"
        open={!!avaliacao}
        onCancel={() => setAvaliacao(null)}
        footer={null}
      >
        {avaliacao && (
          <Descriptions column={1}>
            <Descriptions.Item label="Nota">{avaliacao.nota}</Descriptions.Item>
            <Descriptions.Item label="Comentário">
              {avaliacao.comentario ?? '—'}
            </Descriptions.Item>
            <Descriptions.Item label="Data">
              {formatarDataHora(avaliacao.dataAvaliacao)}
            </Descriptions.Item>
          </Descriptions>
        )}
      </Modal>
    </>
  );
}
