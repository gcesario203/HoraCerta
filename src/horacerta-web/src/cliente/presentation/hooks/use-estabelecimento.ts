'use client';

import { useEffect, useState } from 'react';
import { obterProprietarioPublicoUseCase } from '@/cliente/application/obter-proprietario-publico';
import type { ProprietarioPublicoDto } from '@/cliente/application/dtos/proprietario-publico.dto';

export function useEstabelecimento(proprietarioId: string) {
  const [estabelecimento, setEstabelecimento] = useState<ProprietarioPublicoDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!proprietarioId) {
      setEstabelecimento(null);
      setLoading(false);
      return;
    }
    let cancelled = false;
    setLoading(true);
    obterProprietarioPublicoUseCase
      .execute(proprietarioId)
      .then((data) => {
        if (!cancelled) setEstabelecimento(data);
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

  return {
    estabelecimento,
    loading,
    invalido: !loading && !estabelecimento,
    nome: estabelecimento?.nome ?? 'Estabelecimento',
  };
}
