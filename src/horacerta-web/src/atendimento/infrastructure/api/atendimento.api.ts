import type { AxiosInstance } from 'axios';
import type {
  AlterarEstadoAtendimentoRequest,
  AtendimentoDto,
  RegistrarAtendimentoRequest,
} from '../../application/dtos/atendimento.dto';

export function listarAtendimentosApi(client: AxiosInstance, proprietarioId: string) {
  return client.get<AtendimentoDto[]>(`/proprietarios/${proprietarioId}/atendimentos`);
}

export function registrarAtendimentoApi(
  client: AxiosInstance,
  agendamentoId: string,
  data: RegistrarAtendimentoRequest,
) {
  return client.post<AtendimentoDto>(`/agendamentos/${agendamentoId}/atendimento`, data);
}

export function alterarEstadoAtendimentoApi(
  client: AxiosInstance,
  proprietarioId: string,
  atendimentoId: string,
  data: AlterarEstadoAtendimentoRequest,
) {
  return client.patch<AtendimentoDto>(
    `/proprietarios/${proprietarioId}/atendimentos/${atendimentoId}/estado`,
    data,
  );
}
