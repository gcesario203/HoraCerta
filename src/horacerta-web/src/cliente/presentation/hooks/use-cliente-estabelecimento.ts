'use client';

import { useCallback, useEffect, useState } from 'react';
import { limparSessaoCliente, obterSessaoCliente } from '@/cliente/application/sessao-cliente';
import { useClienteSessaoStore } from '@/cliente/presentation/stores/cliente-sessao.store';

type ClienteEstabelecimentoState = {
  ready: boolean;
  clienteId: string | null;
  semSessao: boolean;
  sair: () => Promise<void>;
};

/**
 * Resolve a sessão do cliente para um estabelecimento específico.
 * Se o cookie for de outro estabelecimento, trata como sem sessão aqui
 * (sem apagar o cookie — o visitante pode voltar ao estabelecimento original).
 */
export function useClienteEstabelecimento(proprietarioId: string): ClienteEstabelecimentoState {
  const storeClienteId = useClienteSessaoStore((s) => s.clienteId);
  const storeProprietarioId = useClienteSessaoStore((s) => s.proprietarioId);
  const setSessao = useClienteSessaoStore((s) => s.setSessao);
  const clearSessao = useClienteSessaoStore((s) => s.clearSessao);

  const [ready, setReady] = useState(false);
  const [clienteId, setClienteId] = useState<string | null>(null);
  const [semSessao, setSemSessao] = useState(false);

  const sair = useCallback(async () => {
    await limparSessaoCliente();
    clearSessao();
    setClienteId(null);
    setSemSessao(true);
  }, [clearSessao]);

  useEffect(() => {
    let cancelled = false;

    const resolver = async () => {
      let cid = storeClienteId;
      let pid = storeProprietarioId;

      if (!cid || !pid) {
        const sessao = await obterSessaoCliente();
        if (cancelled) return;
        if (!sessao) {
          setClienteId(null);
          setSemSessao(true);
          setReady(true);
          return;
        }
        cid = sessao.clienteId;
        pid = sessao.proprietarioId;
      }

      if (pid !== proprietarioId) {
        setClienteId(null);
        setSemSessao(true);
        setReady(true);
        return;
      }

      if (!storeClienteId || storeProprietarioId !== proprietarioId) {
        setSessao(cid, pid);
      }

      if (!cancelled) {
        setClienteId(cid);
        setSemSessao(false);
        setReady(true);
      }
    };

    void resolver();
    return () => {
      cancelled = true;
    };
  }, [proprietarioId, storeClienteId, storeProprietarioId, setSessao]);

  return { ready, clienteId, semSessao, sair };
}
