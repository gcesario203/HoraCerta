'use client';

import { useEffect, useMemo, useState, type CSSProperties } from 'react';
import { Button, Empty } from 'antd';
import { LeftOutlined, RightOutlined } from '@ant-design/icons';
import dayjs, { type Dayjs } from 'dayjs';
import isoWeek from 'dayjs/plugin/isoWeek';
import { labelEstado } from '../format';

dayjs.extend(isoWeek);

const DEFAULT_START = 7;
const DEFAULT_END = 21;
const HOUR_HEIGHT = 52;
const MIN_HOUR_SPAN = 4;

export type WeekTimeSlot = {
  id: string;
  inicio: string;
  fim?: string | null;
  status?: string;
  label?: string;
};

type WeekTimeGridProps = {
  slots: WeekTimeSlot[];
  weekStart: Dayjs;
  onWeekChange: (start: Dayjs) => void;
  selectedId?: string;
  onSelect?: (id: string) => void;
  selectable?: boolean;
  emptyText?: string;
};

function slotDurationMinutes(inicio: string, fim?: string | null) {
  if (fim) {
    const diff = dayjs(fim).diff(dayjs(inicio), 'minute');
    if (diff > 0) return diff;
  }
  return 60;
}

function computeHourRange(slots: WeekTimeSlot[]) {
  if (slots.length === 0) {
    return { start: DEFAULT_START, end: DEFAULT_END };
  }
  let minH = 24;
  let maxH = 0;
  for (const s of slots) {
    const start = dayjs(s.inicio);
    const end = s.fim ? dayjs(s.fim) : start.add(slotDurationMinutes(s.inicio, s.fim), 'minute');
    minH = Math.min(minH, start.hour());
    maxH = Math.max(maxH, end.hour() + (end.minute() > 0 ? 1 : 0));
  }
  const start = Math.max(0, Math.min(minH, DEFAULT_START));
  const end = Math.min(24, Math.max(maxH, DEFAULT_END, start + MIN_HOUR_SPAN));
  return { start, end };
}

function minutesFromStart(iso: string, startHour: number) {
  const d = dayjs(iso);
  return (d.hour() - startHour) * 60 + d.minute();
}

function useCompactLayout() {
  const [compact, setCompact] = useState(false);
  useEffect(() => {
    const mq = window.matchMedia('(max-width: 767px)');
    const update = () => setCompact(mq.matches);
    update();
    mq.addEventListener('change', update);
    return () => mq.removeEventListener('change', update);
  }, []);
  return compact;
}

export function WeekTimeGrid({
  slots,
  weekStart,
  onWeekChange,
  selectedId,
  onSelect,
  selectable = false,
  emptyText = 'Nenhum horário nesta semana',
}: WeekTimeGridProps) {
  const compact = useCompactLayout();
  const weekEnd = weekStart.endOf('isoWeek');
  const [dayIndex, setDayIndex] = useState(() => {
    const today = dayjs();
    if (today.isBefore(weekStart, 'day') || today.isAfter(weekEnd, 'day')) return 0;
    return today.diff(weekStart, 'day');
  });

  const days = useMemo(() => {
    const list: Dayjs[] = [];
    for (let i = 0; i < 7; i += 1) {
      list.push(weekStart.add(i, 'day'));
    }
    return list;
  }, [weekStart]);

  const weekSlots = useMemo(
    () =>
      slots.filter((s) => {
        const d = dayjs(s.inicio);
        return !d.isBefore(weekStart, 'day') && !d.isAfter(weekEnd, 'day');
      }),
    [slots, weekStart, weekEnd],
  );

  const { start: startHour, end: endHour } = useMemo(() => computeHourRange(slots), [slots]);

  const hours = useMemo(() => {
    const list: number[] = [];
    for (let h = startHour; h < endHour; h += 1) list.push(h);
    return list;
  }, [startHour, endHour]);

  const totalHeight = (endHour - startHour) * HOUR_HEIGHT;
  const todayKey = dayjs().format('YYYY-MM-DD');
  const nowMinutes = dayjs().hour() * 60 + dayjs().minute();
  const nowTop = ((nowMinutes - startHour * 60) / 60) * HOUR_HEIGHT;

  const irSemana = (delta: number) => onWeekChange(weekStart.add(delta, 'week'));

  const primeiraSemanaComSlot = useMemo(() => {
    if (slots.length === 0) return null;
    const sorted = [...slots].sort(
      (a, b) => dayjs(a.inicio).valueOf() - dayjs(b.inicio).valueOf(),
    );
    return dayjs(sorted[0].inicio).startOf('isoWeek');
  }, [slots]);

  const visibleDays = compact ? [days[dayIndex] ?? days[0]] : days;
  const colOffset = compact ? 0 : 0;

  useEffect(() => {
    const today = dayjs();
    if (today.isBefore(weekStart, 'day') || today.isAfter(weekEnd, 'day')) {
      setDayIndex(0);
      return;
    }
    setDayIndex(today.diff(weekStart, 'day'));
  }, [weekStart, weekEnd]);

  if (slots.length === 0) {
    return (
      <div className="hc-week-calendar">
        <Empty description={emptyText} style={{ margin: '24px 0' }} />
      </div>
    );
  }

  if (weekSlots.length === 0) {
    return (
      <div className="hc-week-calendar">
        <div className="hc-week-calendar__toolbar">
          <Button icon={<LeftOutlined />} onClick={() => irSemana(-1)} aria-label="Semana anterior" />
          <Button onClick={() => onWeekChange(dayjs().startOf('isoWeek'))}>Hoje</Button>
          <Button icon={<RightOutlined />} onClick={() => irSemana(1)} aria-label="Próxima semana" />
          <span className="hc-week-calendar__range">
            {weekStart.format('D MMM')} – {weekEnd.format('D MMM YYYY')}
          </span>
        </div>
        <Empty description={emptyText} style={{ margin: '24px 0' }}>
          {primeiraSemanaComSlot && !primeiraSemanaComSlot.isSame(weekStart, 'week') ? (
            <Button type="primary" onClick={() => onWeekChange(primeiraSemanaComSlot)}>
              Ir para a próxima semana com horários
            </Button>
          ) : null}
        </Empty>
      </div>
    );
  }

  return (
    <div className="hc-week-calendar">
      <div className="hc-week-calendar__toolbar">
        <Button icon={<LeftOutlined />} onClick={() => irSemana(-1)} aria-label="Semana anterior" />
        <Button onClick={() => onWeekChange(dayjs().startOf('isoWeek'))}>Hoje</Button>
        <Button icon={<RightOutlined />} onClick={() => irSemana(1)} aria-label="Próxima semana" />
        <span className="hc-week-calendar__range">
          {weekStart.format('D MMM')} – {weekEnd.format('D MMM YYYY')}
        </span>
      </div>

      {compact ? (
        <div className="hc-week-calendar__day-picker" role="tablist" aria-label="Dia da semana">
          {days.map((day, index) => {
            const key = day.format('YYYY-MM-DD');
            const isToday = key === todayKey;
            const count = weekSlots.filter(
              (s) => dayjs(s.inicio).format('YYYY-MM-DD') === key,
            ).length;
            return (
              <button
                key={key}
                type="button"
                role="tab"
                aria-selected={dayIndex === index}
                className={[
                  'hc-week-calendar__day-pill',
                  dayIndex === index ? 'hc-week-calendar__day-pill--active' : '',
                  isToday ? 'hc-week-calendar__day-pill--today' : '',
                ]
                  .filter(Boolean)
                  .join(' ')}
                onClick={() => setDayIndex(index)}
              >
                <span className="hc-week-calendar__pill-weekday">{day.format('ddd')}</span>
                <span className="hc-week-calendar__pill-num">{day.format('D')}</span>
                {count > 0 ? <span className="hc-week-calendar__pill-dot" /> : null}
              </button>
            );
          })}
        </div>
      ) : (
        <p className="hc-week-calendar__scroll-hint">Deslize horizontalmente para ver todos os dias</p>
      )}

      <div className={`hc-week-calendar__scroll${compact ? ' hc-week-calendar__scroll--compact' : ''}`}>
        <div
          className={`hc-week-calendar__grid${compact ? ' hc-week-calendar__grid--compact' : ''}`}
          style={
            {
              '--hc-week-hours': endHour - startHour,
              '--hc-week-cols': visibleDays.length,
            } as CSSProperties
          }
        >
          <div className="hc-week-calendar__corner" style={{ gridColumn: 1, gridRow: 1 }} />
          {visibleDays.map((day, index) => {
            const key = day.format('YYYY-MM-DD');
            const isToday = key === todayKey;
            const col = index + 2 + colOffset;
            return (
              <div
                key={key}
                style={{ gridColumn: col, gridRow: 1 }}
                className={`hc-week-calendar__day-head${isToday ? ' hc-week-calendar__day-head--today' : ''}`}
              >
                <span className="hc-week-calendar__weekday">{day.format('ddd')}</span>
                <span className="hc-week-calendar__day-num">{day.format('D')}</span>
              </div>
            );
          })}

          <div
            className="hc-week-calendar__time-col"
            style={{ gridColumn: 1, gridRow: 2, height: totalHeight }}
          >
            {hours.map((h) => (
              <div key={h} className="hc-week-calendar__time-label" style={{ height: HOUR_HEIGHT }}>
                {String(h).padStart(2, '0')}:00
              </div>
            ))}
          </div>

          {visibleDays.map((day, index) => {
            const key = day.format('YYYY-MM-DD');
            const isToday = key === todayKey;
            const daySlots = weekSlots.filter((s) => dayjs(s.inicio).format('YYYY-MM-DD') === key);
            const col = index + 2 + colOffset;

            return (
              <div
                key={key}
                className={`hc-week-calendar__day-col${isToday ? ' hc-week-calendar__day-col--today' : ''}`}
                style={{ gridColumn: col, gridRow: 2, height: totalHeight }}
              >
                {hours.map((h) => (
                  <div
                    key={h}
                    className="hc-week-calendar__hour-line"
                    style={{ top: (h - startHour) * HOUR_HEIGHT, height: HOUR_HEIGHT }}
                  />
                ))}
                {isToday && nowTop >= 0 && nowTop <= totalHeight && (
                  <div className="hc-week-calendar__now" style={{ top: nowTop }} />
                )}
                {daySlots.map((slot) => {
                  const top = (minutesFromStart(slot.inicio, startHour) / 60) * HOUR_HEIGHT;
                  const height = Math.max(
                    (slotDurationMinutes(slot.inicio, slot.fim) / 60) * HOUR_HEIGHT - 2,
                    28,
                  );
                  const selected = selectedId === slot.id;
                  const status = slot.status ?? 'DISPONIVEL';
                  return (
                    <button
                      key={slot.id}
                      type="button"
                      className={[
                        'hc-week-slot',
                        selectable ? 'hc-week-slot--selectable' : '',
                        selected ? 'hc-week-slot--selected' : '',
                        `hc-week-slot--${status.toLowerCase()}`,
                      ]
                        .filter(Boolean)
                        .join(' ')}
                      style={{ top, height }}
                      onClick={() => selectable && onSelect?.(slot.id)}
                      disabled={!selectable}
                      title={slot.label ?? dayjs(slot.inicio).format('HH:mm')}
                    >
                      <span className="hc-week-slot__time">
                        {dayjs(slot.inicio).format('HH:mm')}
                        {slot.fim ? ` – ${dayjs(slot.fim).format('HH:mm')}` : ''}
                      </span>
                      {slot.label ? (
                        <span className="hc-week-slot__label">{slot.label}</span>
                      ) : (
                        <span className="hc-week-slot__label">{labelEstado(status)}</span>
                      )}
                    </button>
                  );
                })}
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}
