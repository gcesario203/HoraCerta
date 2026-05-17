'use client';

import { useCallback, useEffect, useState } from 'react';
import {
  App,
  Button,
  Descriptions,
  Drawer,
  InputNumber,
  Modal,
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
import { listarSlotsDisponiveisUseCase } from '@/slot-horario/application';
import type { SlotHorarioDto } from '@/slot-horario/application/dtos/slot-horario.dto';
import { useProprietarioSessao } from '@/auth/presentation/hooks/use-proprietario-sessao';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { formatarDataHora, labelEstado } from '@/shared/presentation/format';

export default function AgendamentosPage() {
  const { proprietarioId, canAct } = useProprietarioSessao();
  const { message } = App.useApp();
  const [lista, setLista] = useState<AgendamentoListagemDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [slots, setSlots] = useState<SlotHorarioDto[]>([]);
  const [remarcarId, setRemarcarId] = useState<string | null>(null);
  const [novoSlot, setNovoSlot] = useState<string>();
  const [avaliacao, setAvaliacao] = useState<AvaliacaoDto | null>(null);
  const [drawerAg, setDrawerAg] = useState<AgendamentoListagemDto | null>(null);
  const [valorAtendimento, setValorAtendimento] = useState<number | undefined>();

  const carregar = useCallback(async () => {
    if (!canAct || !proprietarioId) return;
    setLoading(true);
    try {
      const data = await listarAgendamentosProprietarioUseCase.execute(proprietarioId);
      setLista(data);
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setLoading(false);
    }
  }, [canAct, proprietarioId, message]);

  useEffect(() => {
    carregar();
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
    setRemarcarId(ag.agendamentoId);
    try {
      const s = await listarSlotsDisponiveisUseCase.execute(proprietarioId);
      setSlots(s);
    } catch (error) {
      message.error(extractApiMessage(error));
    }
  };

  const remarcar = async () => {
    if (!proprietarioId || !remarcarId || !novoSlot) return;
    const ag = lista.find((a) => a.agendamentoId === remarcarId);
    if (!ag) return;
    try {
      await remarcarAgendamentoUseCase.execute(remarcarId, {
        proprietarioId,
        clienteId: ag.clienteId,
        novoSlotHorarioId: novoSlot,
      });
      message.success('Agendamento remarcado');
      setRemarcarId(null);
      setNovoSlot(undefined);
      await carregar();
    } catch (error) {
      message.error(extractApiMessage(error));
    }
  };

  const registrarAtendimento = async (ag: AgendamentoListagemDto, valor?: number) => {
    if (!canAct || !proprietarioId) return;
    try {
      await registrarAtendimentoUseCase.execute(ag.agendamentoId, {
        proprietarioId,
        clienteId: ag.clienteId,
        valorNegociado: valor ?? null,
      });
      message.success('Atendimento registrado');
      await carregar();
    } catch (error) {
      message.error(extractApiMessage(error));
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

  return (
    <>
      <Table
        rowKey="agendamentoId"
        loading={loading}
        dataSource={lista}
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
                  <Button size="small" type="primary" onClick={() => acao('confirmar', r)}>
                    Confirmar
                  </Button>
                )}
                {['PENDENTE', 'CONFIRMADO'].includes(r.estado) && (
                  <Button size="small" danger onClick={() => acao('cancelar', r)}>
                    Cancelar
                  </Button>
                )}
                {r.estado === 'CONFIRMADO' && (
                  <Button size="small" onClick={() => abrirRemarcar(r)}>
                    Remarcar
                  </Button>
                )}
                {r.estado === 'CONFIRMADO' && (
                  <Button size="small" onClick={() => setDrawerAg(r)}>
                    Atendimento
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

      <Modal
        title="Remarcar agendamento"
        open={!!remarcarId}
        onOk={remarcar}
        onCancel={() => setRemarcarId(null)}
        okButtonProps={{ disabled: !novoSlot }}
      >
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
      </Modal>

      <Drawer
        title="Registrar atendimento"
        open={!!drawerAg}
        onClose={() => setDrawerAg(null)}
      >
        {drawerAg && (
          <Space direction="vertical" style={{ width: '100%' }}>
            <p>
              {drawerAg.clienteNome} — {drawerAg.procedimentoNome}
            </p>
            <InputNumber
              placeholder="Valor negociado (opcional)"
              style={{ width: '100%' }}
              min={0}
              value={valorAtendimento}
              onChange={(v) => setValorAtendimento(v ?? undefined)}
            />
            <Button
              type="primary"
              onClick={() => {
                registrarAtendimento(drawerAg, valorAtendimento);
                setDrawerAg(null);
                setValorAtendimento(undefined);
              }}
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
