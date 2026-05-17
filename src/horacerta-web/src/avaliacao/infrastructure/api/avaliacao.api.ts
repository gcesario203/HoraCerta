import type { AxiosInstance } from 'axios';
import type { AvaliacaoDto, AvaliarAgendamentoRequest } from '../../application/dtos/avaliacao.dto';

export function avaliarAgendamentoApi(
  client: AxiosInstance,
  clienteId: string,
  agendamentoId: string,
  data: AvaliarAgendamentoRequest,
) {
  return client.post<AvaliacaoDto>(
    `/clientes/${clienteId}/agendamentos/${agendamentoId}/avaliar`,
    data,
  );
}

export function obterAvaliacaoApi(
  client: AxiosInstance,
  proprietarioId: string,
  agendamentoId: string,
) {
  return client.get<AvaliacaoDto>(
    `/proprietarios/${proprietarioId}/agendamentos/${agendamentoId}/avaliacao`,
  );
}
