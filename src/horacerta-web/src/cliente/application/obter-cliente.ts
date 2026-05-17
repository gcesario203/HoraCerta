import { clienteRepository } from '../infrastructure/api/cliente.repository';

export const obterClienteUseCase = {
  execute(clienteId: string) {
    return clienteRepository.obter(clienteId);
  },
};
