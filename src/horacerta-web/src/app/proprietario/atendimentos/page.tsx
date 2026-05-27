'use client';

import { useCallback, useEffect, useMemo, useState } from 'react';
import Link from 'next/link';
import { App, Card, Modal, Select, Table, Tag, Typography } from 'antd';
import { listarAgendamentosProprietarioUseCase } from '@/agendamento/application';
import {
  alterarEstadoAtendimentoUseCase,
  listarAtendimentosUseCase,
} from '@/atendimento/application';
import type { AtendimentoDto } from '@/atendimento/application/dtos/atendimento.dto';
import { useProprietarioPage } from '@/auth/presentation/hooks/use-proprietario-page';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { PageHeader } from '@/shared/presentation/components/page-header';
import { formatarDataHora, formatarMoeda, labelEstado } from '@/shared/presentation/format';

const estados = ['REALIZADO', 'CANCELADO', 'FALHA'];

type AtendimentoRow = AtendimentoDto & {
  clienteNome?: string;
  procedimentoNome?: string;
  slotInicio?: string;
};

export default function AtendimentosPage() {
  const { proprietarioId, ready, canAct } = useProprietarioPage();
  const { message } = App.useApp();
  const [lista, setLista] = useState<AtendimentoRow[]>([]);
  const [loading, setLoading] = useState(true);

  const carregar = useCallback(async () => {
    if (!ready || !canAct || !proprietarioId) {
      if (ready && !proprietarioId) setLoading(false);
      return;
    }
    setLoading(true);
    try {
      const [atendimentos, agendamentos] = await Promise.all([
        listarAtendimentosUseCase.execute(proprietarioId),
        listarAgendamentosProprietarioUseCase.execute(proprietarioId),
      ]);
      const porAgendamento = new Map(
        agendamentos.map((a) => [a.agendamentoId, a]),
      );
      setLista(
        atendimentos.map((t) => {
          const ag = porAgendamento.get(t.agendamentoId);
          return {
            ...t,
            clienteNome: ag?.clienteNome,
            procedimentoNome: ag?.procedimentoNome,
            slotInicio: ag?.slotInicio,
          };
        }),
      );
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setLoading(false);
    }
  }, [ready, canAct, proprietarioId, message]);

  useEffect(() => {
    void carregar();
  }, [carregar]);

  const alterarEstado = async (atendimentoId: string, estado: string) => {
    if (!canAct || !proprietarioId) return;
    try {
      await alterarEstadoAtendimentoUseCase.execute(proprietarioId, atendimentoId, {
        estado,
      });
      message.success('Estado atualizado');
      await carregar();
    } catch (error) {
      message.error(extractApiMessage(error));
    }
  };

  const dataSource = useMemo(() => lista, [lista]);

  if (!ready) {
    return null;
  }

  return (
    <>
      <PageHeader
        title="Atendimentos"
        description="Atendimentos já registrados. Para registrar um novo, use Agendamentos em um horário confirmado."
      />
      <Typography.Paragraph type="secondary">
        <Link href="/proprietario/agendamentos">Ir para Agendamentos</Link>
      </Typography.Paragraph>
      <Card className="hc-card-elevated" variant="borderless">
        <Table
          rowKey="id"
          loading={loading}
          pagination={{ pageSize: 15, showSizeChanger: true }}
          dataSource={dataSource}
          locale={{ emptyText: 'Nenhum atendimento registrado ainda.' }}
          columns={[
            {
              title: 'Cliente',
              dataIndex: 'clienteNome',
              render: (n: string | undefined) => n ?? '—',
            },
            {
              title: 'Procedimento',
              dataIndex: 'procedimentoNome',
              render: (n: string | undefined) => n ?? '—',
            },
            {
              title: 'Horário',
              dataIndex: 'slotInicio',
              render: (d: string | undefined) => (d ? formatarDataHora(d) : '—'),
            },
            {
              title: 'Valor',
              dataIndex: 'valorNegociado',
              render: (v: number) => formatarMoeda(v),
            },
            {
              title: 'Estado',
              dataIndex: 'estado',
              render: (e: string) => <Tag>{labelEstado(e)}</Tag>,
            },
            {
              title: 'Alterar estado',
              render: (_, r) => (
                <Select
                  style={{ width: 180 }}
                  placeholder="Novo estado"
                  value={undefined}
                  options={estados.map((e) => ({ value: e, label: labelEstado(e) }))}
                  onChange={(v) => {
                    if (!v) return;
                    Modal.confirm({
                      title: `Alterar estado para “${labelEstado(v)}”?`,
                      content: 'Esta alteração será registrada no atendimento.',
                      onOk: () => alterarEstado(r.id, v),
                    });
                  }}
                />
              ),
            },
          ]}
        />
      </Card>
    </>
  );
}
