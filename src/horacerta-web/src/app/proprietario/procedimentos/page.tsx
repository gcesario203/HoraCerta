'use client';

import { useCallback, useEffect, useState } from 'react';
import { App, Button, Form, Input, InputNumber, Modal, Space, Table, Tag } from 'antd';
import {
  criarProcedimentoUseCase,
  inativarProcedimentoUseCase,
  listarProcedimentosUseCase,
} from '@/procedimento/application';
import type { ProcedimentoDto } from '@/procedimento/application/dtos/procedimento.dto';
import { useAuthStore } from '@/auth/presentation/stores/auth.store';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { formatarMoeda, labelEstado } from '@/shared/presentation/format';

export default function ProcedimentosPage() {
  const proprietarioId = useAuthStore((s) => s.proprietarioId);
  const { message } = App.useApp();
  const [lista, setLista] = useState<ProcedimentoDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [salvando, setSalvando] = useState(false);

  const carregar = useCallback(async () => {
    if (!proprietarioId) return;
    setLoading(true);
    try {
      const data = await listarProcedimentosUseCase.execute(proprietarioId);
      setLista(data);
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setLoading(false);
    }
  }, [proprietarioId, message]);

  useEffect(() => {
    carregar();
  }, [carregar]);

  const criar = async (values: {
    nome: string;
    valor: number;
    tempoEstimadoMinutos: number;
  }) => {
    if (!proprietarioId) return;
    setSalvando(true);
    try {
      await criarProcedimentoUseCase.execute(proprietarioId, values);
      message.success('Procedimento criado');
      setModalOpen(false);
      await carregar();
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setSalvando(false);
    }
  };

  const inativar = async (id: string) => {
    if (!proprietarioId) return;
    try {
      await inativarProcedimentoUseCase.execute(proprietarioId, id);
      message.success('Procedimento inativado');
      await carregar();
    } catch (error) {
      message.error(extractApiMessage(error));
    }
  };

  return (
    <>
      <Space style={{ marginBottom: 16 }}>
        <Button type="primary" onClick={() => setModalOpen(true)}>
          Novo procedimento
        </Button>
      </Space>
      <Table
        rowKey="id"
        loading={loading}
        dataSource={lista}
        columns={[
          { title: 'Nome', dataIndex: 'nome' },
          { title: 'Valor', render: (_, r) => formatarMoeda(r.valor) },
          { title: 'Duração (min)', dataIndex: 'tempoEstimadoMinutos' },
          {
            title: 'Estado',
            dataIndex: 'estado',
            render: (e: string) => <Tag>{labelEstado(e)}</Tag>,
          },
          {
            title: 'Ações',
            render: (_, r) =>
              r.estado === 'ATIVO' ? (
                <Button size="small" danger onClick={() => inativar(r.id)}>
                  Inativar
                </Button>
              ) : null,
          },
        ]}
      />
      <Modal
        title="Novo procedimento"
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        footer={null}
        destroyOnHidden
      >
        <Form layout="vertical" onFinish={criar}>
          <Form.Item label="Nome" name="nome" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item label="Valor (R$)" name="valor" rules={[{ required: true }]}>
            <InputNumber min={0} step={0.01} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item
            label="Tempo estimado (minutos)"
            name="tempoEstimadoMinutos"
            rules={[{ required: true }]}
          >
            <InputNumber min={5} style={{ width: '100%' }} />
          </Form.Item>
          <Button type="primary" htmlType="submit" block loading={salvando}>
            Salvar
          </Button>
        </Form>
      </Modal>
    </>
  );
}
