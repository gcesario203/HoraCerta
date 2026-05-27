'use client';

import { Empty, Tag } from 'antd';
import dayjs from 'dayjs';
import type { WeekTimeSlot } from './week-time-grid';
import { formatarDataHora, labelEstado } from '../format';

type SlotCalendarGridProps = {
  slots: WeekTimeSlot[];
};

export function SlotCalendarGrid({ slots }: SlotCalendarGridProps) {
  if (slots.length === 0) {
    return <Empty description="Nenhum horário disponível" />;
  }

  const grouped = slots.reduce<Record<string, WeekTimeSlot[]>>((acc, slot) => {
    const key = dayjs(slot.inicio).format('YYYY-MM-DD');
    if (!acc[key]) acc[key] = [];
    acc[key].push(slot);
    return acc;
  }, {});

  const days = Object.keys(grouped).sort();

  return (
    <div className="hc-calendar-grid">
      {days.map((day) => (
        <section key={day} className="hc-calendar-day">
          <div className="hc-calendar-day__label">
            {dayjs(day).format('dddd, DD [de] MMMM')}
          </div>
          {grouped[day]
            .sort((a, b) => dayjs(a.inicio).valueOf() - dayjs(b.inicio).valueOf())
            .map((slot) => (
              <div key={slot.id} className="hc-slot-chip">
                <span className="hc-slot-chip__time">{formatarDataHora(slot.inicio)}</span>
                <span className="hc-slot-chip__label">
                  {slot.label ?? labelEstado(slot.status ?? 'DISPONIVEL')}
                </span>
                <Tag color={slot.status === 'DISPONIVEL' ? 'success' : 'processing'}>
                  {labelEstado(slot.status ?? 'DISPONIVEL')}
                </Tag>
              </div>
            ))}
        </section>
      ))}
    </div>
  );
}
