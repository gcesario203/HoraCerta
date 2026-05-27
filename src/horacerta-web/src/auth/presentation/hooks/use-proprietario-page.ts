'use client';

import { useEffect } from 'react';
import { usePathname, useRouter } from 'next/navigation';
import { useProprietarioSessao } from './use-proprietario-sessao';

/**
 * Garante hidratação da sessão do proprietário e redireciona para login se ausente.
 * Use em páginas `/proprietario/*` no lugar de `useProprietarioSessao` isolado.
 */
export function useProprietarioPage() {
  const { proprietarioId, hydrated, canAct } = useProprietarioSessao();
  const router = useRouter();
  const pathname = usePathname();

  const ready = hydrated && !!proprietarioId;

  useEffect(() => {
    if (hydrated && !proprietarioId) {
      const redirect = encodeURIComponent(pathname);
      router.replace(`/login?redirect=${redirect}`);
    }
  }, [hydrated, proprietarioId, pathname, router]);

  return {
    proprietarioId: proprietarioId ?? null,
    hydrated,
    ready,
    canAct: canAct && !!proprietarioId,
  };
}
