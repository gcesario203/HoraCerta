'use client';

import { useCallback, useEffect, useState } from 'react';
import { App, Select, Table, Tag } from 'antd';
import {
  alterarEstadoAtendimentoUseCase,
  listarAtendimentosUseCase,
} from '@/atendimento/application';
import type { AtendimentoDto } from '@/atendimento/application/dtos/atendimento.dto';
import { useProprietarioSessao } from '@/auth/presentation/hooks/use-proprietario-sessao';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { labelEstado } from '@/shared/presentation/format';

const estados = ['REALIZADO', 'CANCELADO', 'FALHA'];

export default function AtendimentosPage() {
  const { proprietarioId, canAct } = useProprietarioSessao();
  const { message } = App.useApp();
  const [lista, setLista] = useState<AtendimentoDto[]>([]);
  const [loading, setLoading] = useState(true);

  const carregar = useCallback(async () => {
    if (!canAct || !proprietarioId) return;
    setLoading(true);
    try {
      const data = await listarAtendimentosUseCase.execute(proprietarioId);
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

  return (
    <Table
      rowKey="id"
      loading={loading}
      dataSource={lista}
      columns={[
        { title: 'Agendamento', dataIndex: 'agendamentoId' },
        { title: 'Valor', dataIndex: 'valorNegociado' },
        {
          title: 'Estado',
          dataIndex: 'estado',
          render: (e: string) => <Tag>{labelEstado(e)}</Tag>,
        },
        {
          title: 'Alterar estado',
          render: (_, r) => (
            <Select
              style={{ width: 160 }}
              placeholder="Novo estado"
              options={estados.map((e) => ({ value: e, label: labelEstado(e) }))}
              onChange={(v) => alterarEstado(r.id, v)}
            />
          ),
        },
      ]}
    />
  );
}
