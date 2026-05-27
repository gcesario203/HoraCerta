import type { AxiosInstance } from 'axios';
import type { ClienteDto, CriarClienteRequest } from '../../application/dtos/cliente.dto';
import type { ProprietarioPublicoDto } from '../../application/dtos/proprietario-publico.dto';

export function criarClienteApi(client: AxiosInstance, data: CriarClienteRequest) {
  return client.post<ClienteDto>('/clientes', data);
}

export function obterClienteApi(client: AxiosInstance, clienteId: string) {
  return client.get<ClienteDto>(`/clientes/${clienteId}`);
}

export function obterProprietarioPublicoApi(client: AxiosInstance, proprietarioId: string) {
  return client.get<ProprietarioPublicoDto>(`/proprietarios/${proprietarioId}`);
}
