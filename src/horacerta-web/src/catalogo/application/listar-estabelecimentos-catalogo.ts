import { publicApiClient } from '@/shared/infrastructure/http/axios-client';
import { listarEstabelecimentosCatalogoApi } from '../infrastructure/api/catalogo.api';

export function listarEstabelecimentosCatalogo(busca?: string) {
  return listarEstabelecimentosCatalogoApi(publicApiClient, busca).then((r) => r.data);
}
