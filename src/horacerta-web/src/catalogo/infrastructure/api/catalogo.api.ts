import type { AxiosInstance } from 'axios';
import type { EstabelecimentoCatalogoDto } from '../../application/dtos/catalogo.dto';

export function listarEstabelecimentosCatalogoApi(
  client: AxiosInstance,
  busca?: string,
) {
  const params = busca?.trim() ? { busca: busca.trim() } : undefined;
  return client.get<EstabelecimentoCatalogoDto[]>('/catalogo/estabelecimentos', { params });
}
