'use client';

import { useEffect } from 'react';
import { createApiClient } from '@/shared/infrastructure/http/axios-client';
import { useAuthStore } from '@/auth/presentation/stores/auth.store';
import { useClienteSessaoStore } from '@/cliente/presentation/stores/cliente-sessao.store';
import { obterSessaoCliente } from '@/cliente/application/sessao-cliente';

const bffRoot = createApiClient('', true);

export function SessaoHydrator() {
  const setAuth = useAuthStore((s) => s.setSession);
  const setHydrated = useAuthStore((s) => s.setHydrated);
  const setCliente = useClienteSessaoStore((s) => s.setSessao);

  useEffect(() => {
    void (async () => {
      try {
        const { data } = await bffRoot.get<{ proprietarioId: string }>(
          '/api/bff/auth/session',
        );
        if (data.proprietarioId) setAuth(data.proprietarioId);
      } catch {
        /* sem sessão */
      } finally {
        setHydrated();
      }

      const sessao = await obterSessaoCliente();
      if (sessao) setCliente(sessao.clienteId, sessao.proprietarioId);
    })();
  }, [setAuth, setHydrated, setCliente]);

  return null;
}
