'use client';

import { useCallback, useEffect, useState } from 'react';
import { App, Button, DatePicker, Form, Modal, Table, Tag } from 'antd';
import dayjs from 'dayjs';
import { criarSlotUseCase, listarSlotsDisponiveisUseCase } from '@/slot-horario/application';
import type { SlotHorarioDto } from '@/slot-horario/application/dtos/slot-horario.dto';
import { useAuthStore } from '@/auth/presentation/stores/auth.store';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { formatarDataHora, labelEstado } from '@/shared/presentation/format';

export default function AgendaPage() {
  const proprietarioId = useAuthStore((s) => s.proprietarioId);
  const { message } = App.useApp();
  const [lista, setLista] = useState<SlotHorarioDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [salvando, setSalvando] = useState(false);

  const carregar = useCallback(async () => {
    if (!proprietarioId) return;
    setLoading(true);
    try {
      const data = await listarSlotsDisponiveisUseCase.execute(proprietarioId);
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

  const criar = async (values: { inicio: dayjs.Dayjs }) => {
    if (!proprietarioId) return;
    setSalvando(true);
    try {
      await criarSlotUseCase.execute(
        proprietarioId,
        values.inicio.toISOString(),
      );
      message.success('Horário disponibilizado');
      setModalOpen(false);
      await carregar();
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setSalvando(false);
    }
  };

  return (
    <>
      <Button type="primary" style={{ marginBottom: 16 }} onClick={() => setModalOpen(true)}>
        Novo horário
      </Button>
      <Table
        rowKey="id"
        loading={loading}
        dataSource={lista}
        columns={[
          { title: 'Início', render: (_, r) => formatarDataHora(r.inicio) },
          {
            title: 'Fim',
            render: (_, r) => (r.fim ? formatarDataHora(r.fim) : '—'),
          },
          {
            title: 'Status',
            dataIndex: 'status',
            render: (s: string) => <Tag>{labelEstado(s)}</Tag>,
          },
        ]}
      />
      <Modal
        title="Disponibilizar horário"
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        footer={null}
        destroyOnHidden
      >
        <Form layout="vertical" onFinish={criar}>
          <Form.Item label="Data e hora" name="inicio" rules={[{ required: true }]}>
            <DatePicker showTime style={{ width: '100%' }} format="DD/MM/YYYY HH:mm" />
          </Form.Item>
          <Button type="primary" htmlType="submit" block loading={salvando}>
            Salvar
          </Button>
        </Form>
      </Modal>
    </>
  );
}
