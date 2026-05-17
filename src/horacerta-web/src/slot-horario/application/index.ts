import { slotHorarioRepository } from '../infrastructure/api/slot-horario.repository';

export const listarSlotsDisponiveisUseCase = {
  execute: (proprietarioId: string) => slotHorarioRepository.listarDisponiveis(proprietarioId),
};

export const criarSlotUseCase = {
  execute: (proprietarioId: string, inicio: string) =>
    slotHorarioRepository.criar(proprietarioId, { inicio }),
};
