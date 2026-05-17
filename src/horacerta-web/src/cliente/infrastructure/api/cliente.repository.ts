import { publicApiClient } from '@/shared/infrastructure/http/axios-client';
import type { CriarClienteRequest } from '../../application/dtos/cliente.dto';
import { criarClienteApi, obterClienteApi } from './cliente.api';

export class ClienteRepository {
  criar(data: CriarClienteRequest) {
    return criarClienteApi(publicApiClient, data).then((r) => r.data);
  }

  obter(clienteId: string) {
    return obterClienteApi(publicApiClient, clienteId).then((r) => r.data);
  }
}

export const clienteRepository = new ClienteRepository();
