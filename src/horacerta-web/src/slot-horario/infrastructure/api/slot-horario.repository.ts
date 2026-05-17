import { bffApiClient, publicApiClient } from '@/shared/infrastructure/http/axios-client';
import { criarSlotApi, listarSlotsDisponiveisApi } from './slot-horario.api';

export class SlotHorarioRepository {
  listarDisponiveis(proprietarioId: string) {
    return listarSlotsDisponiveisApi(publicApiClient, proprietarioId).then((r) => r.data);
  }

  criar(proprietarioId: string, data: { inicio: string }) {
    return criarSlotApi(bffApiClient, proprietarioId, data).then((r) => r.data);
  }
}

export const slotHorarioRepository = new SlotHorarioRepository();
