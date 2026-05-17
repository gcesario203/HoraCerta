'use client';

import { useCallback, useEffect, useState } from 'react';
import { App, Button, Card, Form, Input, InputNumber, Modal, Table, Tag } from 'antd';
import {
  criarProcedimentoUseCase,
  inativarProcedimentoUseCase,
  listarProcedimentosUseCase,
} from '@/procedimento/application';
import type { ProcedimentoDto } from '@/procedimento/application/dtos/procedimento.dto';
import { useProprietarioSessao } from '@/auth/presentation/hooks/use-proprietario-sessao';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { PageHeader } from '@/shared/presentation/components/page-header';
import { formatarMoeda, labelEstado } from '@/shared/presentation/format';

export default function ProcedimentosPage() {
  const { proprietarioId, canAct } = useProprietarioSessao();
  const { message } = App.useApp();
  const [lista, setLista] = useState<ProcedimentoDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [salvando, setSalvando] = useState(false);

  const carregar = useCallback(async () => {
    if (!canAct || !proprietarioId) return;
    setLoading(true);
    try {
      const data = await listarProcedimentosUseCase.execute(proprietarioId);
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

  const criar = async (values: {
    nome: string;
    valor: number;
    tempoEstimadoMinutos: number;
  }) => {
    if (!canAct || !proprietarioId) return;
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
    if (!canAct || !proprietarioId) return;
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
      <PageHeader
        title="Procedimentos"
        description="Cadastre os serviços que os clientes podem agendar."
        extra={
          <Button type="primary" onClick={() => setModalOpen(true)}>
            Novo procedimento
          </Button>
        }
      />
      <Card className="hc-card-elevated" variant="borderless">
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
      </Card>
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
