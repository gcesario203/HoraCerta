'use client';

import { useCallback, useEffect, useState } from 'react';
import { App, Button, Card, DatePicker, Form, Modal, Segmented, Table, Tag } from 'antd';
import dayjs from 'dayjs';
import isoWeek from 'dayjs/plugin/isoWeek';
import { criarSlotUseCase, listarSlotsDisponiveisUseCase } from '@/slot-horario/application';
import type { SlotHorarioDto } from '@/slot-horario/application/dtos/slot-horario.dto';
import { useProprietarioSessao } from '@/auth/presentation/hooks/use-proprietario-sessao';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { PageHeader } from '@/shared/presentation/components/page-header';
import { SlotCalendarGrid } from '@/shared/presentation/components/slot-calendar-grid';
import { WeekTimeGrid } from '@/shared/presentation/components/week-time-grid';
import { formatarDataHora, labelEstado } from '@/shared/presentation/format';

dayjs.extend(isoWeek);

type ViewMode = 'semana' | 'lista' | 'tabela';

const VIEW_STORAGE_KEY = 'horacerta-agenda-view';

export default function AgendaPage() {
  const { proprietarioId, canAct } = useProprietarioSessao();
  const { message } = App.useApp();
  const [lista, setLista] = useState<SlotHorarioDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [salvando, setSalvando] = useState(false);
  const [view, setView] = useState<ViewMode>('semana');
  const [weekStart, setWeekStart] = useState(() => dayjs().startOf('isoWeek'));

  useEffect(() => {
    const saved = localStorage.getItem(VIEW_STORAGE_KEY) as ViewMode | null;
    if (saved === 'semana' || saved === 'lista' || saved === 'tabela') setView(saved);
  }, []);

  const carregar = useCallback(async () => {
    if (!canAct || !proprietarioId) return;
    setLoading(true);
    try {
      const data = await listarSlotsDisponiveisUseCase.execute(proprietarioId);
      setLista(data);
      if (data.length > 0) {
        setWeekStart(dayjs(data[0].inicio).startOf('isoWeek'));
      }
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setLoading(false);
    }
  }, [canAct, proprietarioId, message]);

  useEffect(() => {
    void carregar();
  }, [carregar]);

  const criar = async (values: { inicio: dayjs.Dayjs }) => {
    if (!canAct || !proprietarioId) return;
    setSalvando(true);
    try {
      await criarSlotUseCase.execute(proprietarioId, values.inicio.toISOString());
      message.success('Horário disponibilizado');
      setModalOpen(false);
      setWeekStart(values.inicio.startOf('isoWeek'));
      await carregar();
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setSalvando(false);
    }
  };

  const alterarView = (v: ViewMode) => {
    setView(v);
    localStorage.setItem(VIEW_STORAGE_KEY, v);
  };

  return (
    <>
      <PageHeader
        title="Agenda"
        description="Visualize e disponibilize horários no estilo de um calendário semanal."
        extra={
          <Button type="primary" onClick={() => setModalOpen(true)}>
            Novo horário
          </Button>
        }
      />

      <Card className="hc-card-elevated" variant="borderless" style={{ marginBottom: 16 }}>
        <Segmented
          value={view}
          onChange={(v) => alterarView(v as ViewMode)}
          options={[
            { label: 'Semana', value: 'semana' },
            { label: 'Lista', value: 'lista' },
            { label: 'Tabela', value: 'tabela' },
          ]}
        />
      </Card>

      <Card className="hc-card-elevated hc-card-elevated--wide" variant="borderless" loading={loading}>
        {view === 'semana' ? (
          <WeekTimeGrid
            slots={lista}
            weekStart={weekStart}
            onWeekChange={setWeekStart}
            emptyText="Nenhum horário disponível nesta semana"
          />
        ) : null}
        {view === 'lista' ? <SlotCalendarGrid slots={lista} /> : null}
        {view === 'tabela' ? (
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
        ) : null}
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
