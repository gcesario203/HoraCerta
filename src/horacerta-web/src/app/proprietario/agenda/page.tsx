'use client';

import { useCallback, useEffect, useState } from 'react';
import { App, Button, Card, DatePicker, Form, Modal, Segmented, Table, Tag } from 'antd';
import dayjs from 'dayjs';
import { criarSlotUseCase, listarSlotsDisponiveisUseCase } from '@/slot-horario/application';
import type { SlotHorarioDto } from '@/slot-horario/application/dtos/slot-horario.dto';
import { useProprietarioSessao } from '@/auth/presentation/hooks/use-proprietario-sessao';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { PageHeader } from '@/shared/presentation/components/page-header';
import { SlotCalendarGrid } from '@/shared/presentation/components/slot-calendar-grid';
import { formatarDataHora, labelEstado } from '@/shared/presentation/format';

type ViewMode = 'calendario' | 'tabela';

export default function AgendaPage() {
  const { proprietarioId, canAct } = useProprietarioSessao();
  const { message } = App.useApp();
  const [lista, setLista] = useState<SlotHorarioDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [salvando, setSalvando] = useState(false);
  const [view, setView] = useState<ViewMode>('calendario');

  const carregar = useCallback(async () => {
    if (!canAct || !proprietarioId) return;
    setLoading(true);
    try {
      const data = await listarSlotsDisponiveisUseCase.execute(proprietarioId);
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

  const criar = async (values: { inicio: dayjs.Dayjs }) => {
    if (!canAct || !proprietarioId) return;
    setSalvando(true);
    try {
      await criarSlotUseCase.execute(proprietarioId, values.inicio.toISOString());
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
      <PageHeader
        title="Agenda"
        description="Disponibilize horários para seus clientes agendarem online."
        extra={
          <Button type="primary" onClick={() => setModalOpen(true)}>
            Novo horário
          </Button>
        }
      />

      <Card className="hc-card-elevated" variant="borderless" style={{ marginBottom: 16 }}>
        <Segmented
          value={view}
          onChange={(v) => setView(v as ViewMode)}
          options={[
            { label: 'Calendário', value: 'calendario' },
            { label: 'Tabela', value: 'tabela' },
          ]}
        />
      </Card>

      <Card className="hc-card-elevated" variant="borderless" loading={loading}>
        {view === 'calendario' ? (
          <SlotCalendarGrid slots={lista} />
        ) : (
          <Table
            rowKey="id"
            pagination={false}
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
                render: (s: string) => (
                  <Tag color={s === 'DISPONIVEL' ? 'success' : 'default'}>
                    {labelEstado(s)}
                  </Tag>
                ),
              },
            ]}
          />
        )}
      </Card>

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
          <Button type="primary" htmlType="submit" block loading={salvando} size="large">
            Salvar
          </Button>
        </Form>
      </Modal>
    </>
  );
}
