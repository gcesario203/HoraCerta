import { atendimentoRepository } from '../infrastructure/api/atendimento.repository';
import type {
  AlterarEstadoAtendimentoRequest,
  RegistrarAtendimentoRequest,
} from './dtos/atendimento.dto';

export const listarAtendimentosUseCase = {
  execute: (proprietarioId: string) => atendimentoRepository.listar(proprietarioId),
};

export const registrarAtendimentoUseCase = {
  execute: (agendamentoId: string, data: RegistrarAtendimentoRequest) =>
    atendimentoRepository.registrar(agendamentoId, data),
};

export const alterarEstadoAtendimentoUseCase = {
  execute: (
    proprietarioId: string,
    atendimentoId: string,
    data: AlterarEstadoAtendimentoRequest,
  ) => atendimentoRepository.alterarEstado(proprietarioId, atendimentoId, data),
};
