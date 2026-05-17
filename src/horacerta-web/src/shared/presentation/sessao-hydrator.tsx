'use client';

import { useEffect } from 'react';
import { createApiClient } from '@/shared/infrastructure/http/axios-client';
import { useAuthStore } from '@/auth/presentation/stores/auth.store';
import { useClienteSessaoStore } from '@/cliente/presentation/stores/cliente-sessao.store';
import { obterSessaoCliente } from '@/cliente/application/sessao-cliente';

const bffRoot = createApiClient('', true);

export function SessaoHydrator() {
  const setAuth = useAuthStore((s) => s.setSession);
  const setCliente = useClienteSessaoStore((s) => s.setSessao);

  useEffect(() => {
    bffRoot
      .get<{ proprietarioId: string }>('/api/bff/auth/session')
      .then((r) => setAuth(r.data.proprietarioId))
      .catch(() => undefined);

    obterSessaoCliente().then((sessao) => {
      if (sessao) setCliente(sessao.clienteId, sessao.proprietarioId);
    });
  }, [setAuth, setCliente]);

  return null;
}
