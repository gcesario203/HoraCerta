'use client';

import { useMemo, type CSSProperties } from 'react';
import { Button, Empty } from 'antd';
import { LeftOutlined, RightOutlined } from '@ant-design/icons';
import dayjs, { type Dayjs } from 'dayjs';
import isoWeek from 'dayjs/plugin/isoWeek';
import { labelEstado } from '../format';

dayjs.extend(isoWeek);

const START_HOUR = 7;
const END_HOUR = 21;
const HOUR_HEIGHT = 52;

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

function minutesFromStart(iso: string) {
  const d = dayjs(iso);
  return (d.hour() - START_HOUR) * 60 + d.minute();
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
  const weekEnd = weekStart.endOf('isoWeek');
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

  const hours = useMemo(() => {
    const list: number[] = [];
    for (let h = START_HOUR; h < END_HOUR; h += 1) list.push(h);
    return list;
  }, []);

  const totalHeight = (END_HOUR - START_HOUR) * HOUR_HEIGHT;
  const todayKey = dayjs().format('YYYY-MM-DD');
  const nowMinutes = dayjs().hour() * 60 + dayjs().minute();
  const nowTop =
    ((nowMinutes - START_HOUR * 60) / 60) * HOUR_HEIGHT;

  const irSemana = (delta: number) => onWeekChange(weekStart.add(delta, 'week'));

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

      {slots.length === 0 ? (
        <Empty description={emptyText} style={{ margin: '24px 0' }} />
      ) : (
        <div className="hc-week-calendar__scroll">
          <div
            className="hc-week-calendar__grid"
            style={{ '--hc-week-hours': END_HOUR - START_HOUR } as CSSProperties}
          >
            <div className="hc-week-calendar__corner" style={{ gridColumn: 1, gridRow: 1 }} />
            {days.map((day, index) => {
              const key = day.format('YYYY-MM-DD');
              const isToday = key === todayKey;
              return (
                <div
                  key={key}
                  style={{ gridColumn: index + 2, gridRow: 1 }}
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

            {days.map((day, index) => {
              const key = day.format('YYYY-MM-DD');
              const isToday = key === todayKey;
              const daySlots = weekSlots.filter((s) => dayjs(s.inicio).format('YYYY-MM-DD') === key);

              return (
                <div
                  key={key}
                  className={`hc-week-calendar__day-col${isToday ? ' hc-week-calendar__day-col--today' : ''}`}
                  style={{ gridColumn: index + 2, gridRow: 2, height: totalHeight }}
                >
                  {hours.map((h) => (
                    <div
                      key={h}
                      className="hc-week-calendar__hour-line"
                      style={{ top: (h - START_HOUR) * HOUR_HEIGHT, height: HOUR_HEIGHT }}
                    />
                  ))}
                  {isToday && nowTop >= 0 && nowTop <= totalHeight && (
                    <div className="hc-week-calendar__now" style={{ top: nowTop }} />
                  )}
                  {daySlots.map((slot) => {
                    const top = (minutesFromStart(slot.inicio) / 60) * HOUR_HEIGHT;
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
      )}
    </div>
  );
}
