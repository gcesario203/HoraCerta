import { clienteRepository } from '../infrastructure/api/cliente.repository';

export const obterProprietarioPublicoUseCase = {
  execute(proprietarioId: string) {
    return clienteRepository.obterProprietarioPublico(proprietarioId);
  },
};
