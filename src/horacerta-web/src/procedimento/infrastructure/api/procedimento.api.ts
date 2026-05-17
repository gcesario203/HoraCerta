import type { AxiosInstance } from 'axios';
import type { CriarProcedimentoRequest, ProcedimentoDto } from '../../application/dtos/procedimento.dto';

export function listarProcedimentosApi(client: AxiosInstance, proprietarioId: string) {
  return client.get<ProcedimentoDto[]>(`/proprietarios/${proprietarioId}/procedimentos`);
}

export function criarProcedimentoApi(
  client: AxiosInstance,
  proprietarioId: string,
  data: CriarProcedimentoRequest,
) {
  return client.post<ProcedimentoDto>(`/proprietarios/${proprietarioId}/procedimentos`, data);
}

export function inativarProcedimentoApi(
  client: AxiosInstance,
  proprietarioId: string,
  procedimentoId: string,
) {
  return client.post<ProcedimentoDto>(
    `/proprietarios/${proprietarioId}/procedimentos/${procedimentoId}/inativar`,
  );
}
