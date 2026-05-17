import { procedimentoRepository } from '../infrastructure/api/procedimento.repository';
import type { CriarProcedimentoRequest } from './dtos/procedimento.dto';

export const listarProcedimentosUseCase = {
  execute: (proprietarioId: string) => procedimentoRepository.listarAtivos(proprietarioId),
};

export const listarProcedimentosPublicoUseCase = listarProcedimentosUseCase;

export const criarProcedimentoUseCase = {
  execute: (proprietarioId: string, data: CriarProcedimentoRequest) =>
    procedimentoRepository.criar(proprietarioId, data),
};

export const inativarProcedimentoUseCase = {
  execute: (proprietarioId: string, procedimentoId: string) =>
    procedimentoRepository.inativar(proprietarioId, procedimentoId),
};
