'use client';

import { useEffect, useState } from 'react';
import { publicApiClient } from '@/shared/infrastructure/http/axios-client';

type ProprietarioPublico = { id: string; nome: string };

export function useEstabelecimento(proprietarioId: string) {
  const [estabelecimento, setEstabelecimento] = useState<ProprietarioPublico | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!proprietarioId) {
      setEstabelecimento(null);
      setLoading(false);
      return;
    }
    let cancelled = false;
    setLoading(true);
    publicApiClient
      .get<ProprietarioPublico>(`/proprietarios/${proprietarioId}`)
      .then((r) => {
        if (!cancelled) setEstabelecimento(r.data);
      })
      .catch(() => {
        if (!cancelled) setEstabelecimento(null);
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [proprietarioId]);

  return { estabelecimento, loading, nome: estabelecimento?.nome ?? 'Estabelecimento' };
}
