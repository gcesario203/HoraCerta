'use client';

import { useCallback, useEffect, useMemo, useState } from 'react';
import { App, Button, Card, DatePicker, Empty, Form, Modal, Segmented, Table, Tag } from 'antd';
import dayjs from 'dayjs';
import isoWeek from 'dayjs/plugin/isoWeek';
import { listarAgendamentosProprietarioUseCase } from '@/agendamento/application';
import { useProprietarioPage } from '@/auth/presentation/hooks/use-proprietario-page';
import { listarProcedimentosUseCase } from '@/procedimento/application';
import type { ProcedimentoDto } from '@/procedimento/application/dtos/procedimento.dto';
import { criarSlotUseCase, listarSlotsDisponiveisUseCase } from '@/slot-horario/application';
import type { SlotHorarioDto } from '@/slot-horario/application/dtos/slot-horario.dto';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { PageHeader } from '@/shared/presentation/components/page-header';
import { SlotCalendarGrid } from '@/shared/presentation/components/slot-calendar-grid';
import { WeekTimeGrid } from '@/shared/presentation/components/week-time-grid';
import { buildAgendaEvents } from '@/shared/presentation/utils/build-agenda-events';
import { formatarDataHora, labelEstado } from '@/shared/presentation/format';

dayjs.extend(isoWeek);

type ViewMode = 'semana' | 'lista' | 'tabela';

const VIEW_STORAGE_KEY = 'horacerta-agenda-view';

export default function AgendaPage() {
  const { proprietarioId, ready, canAct } = useProprietarioPage();
  const { message } = App.useApp();
  const [slots, setSlots] = useState<SlotHorarioDto[]>([]);
  const [agendamentos, setAgendamentos] = useState<
    Awaited<ReturnType<typeof listarAgendamentosProprietarioUseCase.execute>>
  >([]);
  const [procedimentos, setProcedimentos] = useState<ProcedimentoDto[]>([]);
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
    if (!ready || !canAct || !proprietarioId) {
      if (ready && !proprietarioId) setLoading(false);
      return;
    }
    setLoading(true);
    try {
      const [slotsData, agData, procedimentos] = await Promise.all([
        listarSlotsDisponiveisUseCase.execute(proprietarioId),
        listarAgendamentosProprietarioUseCase.execute(proprietarioId),
        listarProcedimentosUseCase.execute(proprietarioId),
      ]);
      setSlots(slotsData);
      setAgendamentos(agData);
      setProcedimentos(procedimentos);
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setLoading(false);
    }
  }, [ready, canAct, proprietarioId, message]);

  useEffect(() => {
    void carregar();
  }, [carregar]);

  const duracaoPorProcedimento = useMemo(() => {
    const map: Record<string, number> = {};
    for (const p of procedimentos) {
      map[p.nome] = p.tempoEstimadoMinutos;
    }
    return map;
  }, [procedimentos]);

  const eventos = useMemo(
    () => buildAgendaEvents(slots, agendamentos, duracaoPorProcedimento),
    [slots, agendamentos, duracaoPorProcedimento],
  );

  const eventosLista = useMemo(
    () =>
      [...eventos].sort((a, b) => dayjs(a.inicio).valueOf() - dayjs(b.inicio).valueOf()),
    [eventos],
  );

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

  if (!ready) {
    return null;
  }

  return (
    <>
      <PageHeader
        title="Agenda"
        description="Visão semanal no estilo calendário: horários livres e agendamentos confirmados."
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
          eventos.length === 0 && !loading ? (
            <Empty description="Nenhum horário ou agendamento. Disponibilize horários para começar.">
              <Button type="primary" onClick={() => setModalOpen(true)}>
                Disponibilizar horário
              </Button>
            </Empty>
          ) : (
            <WeekTimeGrid
              slots={eventos}
              weekStart={weekStart}
              onWeekChange={setWeekStart}
              emptyText="Nenhum evento nesta semana"
            />
          )
        ) : null}
        {view === 'lista' ? (
          eventosLista.length === 0 && !loading ? (
            <Empty description="Nenhum horário ou agendamento" />
          ) : (
            <SlotCalendarGrid slots={eventosLista} />
          )
        ) : null}
        {view === 'tabela' ? (
          <Table
            rowKey="id"
            pagination={{ pageSize: 20, showSizeChanger: true }}
            dataSource={eventosLista}
            locale={{ emptyText: 'Nenhum horário ou agendamento' }}
            columns={[
              { title: 'Início', render: (_, r) => formatarDataHora(r.inicio) },
              {
                title: 'Fim',
                render: (_, r) => (r.fim ? formatarDataHora(r.fim) : '—'),
              },
              {
                title: 'Descrição',
                dataIndex: 'label',
                render: (l: string | undefined, r) => l ?? labelEstado(r.status ?? ''),
              },
              {
                title: 'Status',
                dataIndex: 'status',
                render: (s: string) => (
                  <Tag color={s === 'DISPONIVEL' ? 'success' : 'processing'}>
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
