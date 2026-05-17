import { avaliacaoRepository } from '../infrastructure/api/avaliacao.repository';
import type { AvaliarAgendamentoRequest } from './dtos/avaliacao.dto';

export const avaliarAgendamentoUseCase = {
  execute: (clienteId: string, agendamentoId: string, data: AvaliarAgendamentoRequest) =>
    avaliacaoRepository.avaliar(clienteId, agendamentoId, data),
};

export const obterAvaliacaoUseCase = {
  execute: (proprietarioId: string, agendamentoId: string) =>
    avaliacaoRepository.obter(proprietarioId, agendamentoId),
};
