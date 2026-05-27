import dayjs from 'dayjs';
import type { AgendamentoListagemDto } from '@/agendamento/application/dtos/agendamento.dto';
import type { SlotHorarioDto } from '@/slot-horario/application/dtos/slot-horario.dto';
import type { WeekTimeSlot } from '../components/week-time-grid';

const DEFAULT_DURATION_MIN = 60;

function fimFromInicio(inicio: string, minutos: number) {
  return dayjs(inicio).add(minutos, 'minute').toISOString();
}

/** Monta eventos da agenda do proprietário (horários livres + agendamentos). */
export function buildAgendaEvents(
  slots: SlotHorarioDto[],
  agendamentos: AgendamentoListagemDto[],
  duracaoPorProcedimentoMin?: Record<string, number>,
): WeekTimeSlot[] {
  const agendamentoEvents: WeekTimeSlot[] = agendamentos
    .filter((a) => !['CANCELADO'].includes(a.estado))
    .map((a) => {
      const min =
        duracaoPorProcedimentoMin?.[a.procedimentoNome] ?? DEFAULT_DURATION_MIN;
      return {
        id: `ag-${a.agendamentoId}`,
        inicio: a.slotInicio,
        fim: fimFromInicio(a.slotInicio, min),
        status: a.estado,
        label: `${a.clienteNome} · ${a.procedimentoNome}`,
      };
    });

  const slotEvents: WeekTimeSlot[] = slots.map((s) => ({
    id: s.id,
    inicio: s.inicio,
    fim: s.fim,
    status: 'DISPONIVEL',
    label: 'Disponível',
  }));

  return [...agendamentoEvents, ...slotEvents];
}
