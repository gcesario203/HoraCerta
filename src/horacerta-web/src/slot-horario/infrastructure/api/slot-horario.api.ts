import type { AxiosInstance } from 'axios';
import type { CriarSlotRequest, SlotHorarioDto } from '../../application/dtos/slot-horario.dto';

export function listarSlotsDisponiveisApi(client: AxiosInstance, proprietarioId: string) {
  return client.get<SlotHorarioDto[]>(`/proprietarios/${proprietarioId}/slots/disponiveis`);
}

export function criarSlotApi(
  client: AxiosInstance,
  proprietarioId: string,
  data: CriarSlotRequest,
) {
  return client.post<SlotHorarioDto>(`/proprietarios/${proprietarioId}/slots`, data);
}
