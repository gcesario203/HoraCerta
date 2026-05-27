import type { ClienteSessaoCookie } from '@/shared/infrastructure/cookies';
import { createApiClient } from '@/shared/infrastructure/http/axios-client';

const bffRoot = createApiClient('');

export async function obterSessaoCliente(): Promise<ClienteSessaoCookie | null> {
  try {
    const { data } = await bffRoot.get<ClienteSessaoCookie>('/api/bff/cliente-sessao');
    return data;
  } catch {
    return null;
  }
}

export async function salvarSessaoCliente(sessao: ClienteSessaoCookie): Promise<void> {
  await bffRoot.post('/api/bff/cliente-sessao', sessao);
}

export async function limparSessaoCliente(): Promise<void> {
  await bffRoot.delete('/api/bff/cliente-sessao');
}
