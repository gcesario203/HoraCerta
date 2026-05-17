'use client';

import { useParams } from 'next/navigation';
import { BookingWizard } from '@/cliente/presentation/components/booking-wizard';
import { ClienteShell } from '@/shared/presentation/layouts/cliente-shell';

export default function AgendarPage() {
  const params = useParams<{ proprietarioId: string }>();
  const proprietarioId = params.proprietarioId;

  return (
    <ClienteShell
      proprietarioId={proprietarioId}
      title="Agendar"
      subtitle="Escolha o serviço, um horário na agenda e confira o resumo antes de enviar."
      wide
    >
      <BookingWizard proprietarioId={proprietarioId} />
    </ClienteShell>
  );
}
