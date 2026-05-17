import { clienteRepository } from '../infrastructure/api/cliente.repository';
import type { CriarClienteRequest } from './dtos/cliente.dto';

export const criarClienteUseCase = {
  execute: (data: CriarClienteRequest) => clienteRepository.criar(data),
};

export const obterClienteUseCase = {
  execute: (clienteId: string) => clienteRepository.obter(clienteId),
};
