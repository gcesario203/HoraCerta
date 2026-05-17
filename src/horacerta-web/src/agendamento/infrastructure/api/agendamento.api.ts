import type { AxiosInstance } from 'axios';
import type {
  AcaoAgendamentoRequest,
  AgendamentoClienteListagemDto,
  AgendamentoDto,
  AgendamentoListagemDto,
  IniciarAgendamentoRequest,
  RemarcarAgendamentoRequest,
} from '../../application/dtos/agendamento.dto';

export function iniciarAgendamentoApi(client: AxiosInstance, data: IniciarAgendamentoRequest) {
  return client.post<AgendamentoDto>('/agendamentos/iniciar', data);
}

export function listarAgendamentosClienteApi(client: AxiosInstance, clienteId: string) {
  return client.get<AgendamentoClienteListagemDto[]>(`/clientes/${clienteId}/agendamentos`);
}

export function listarAgendamentosProprietarioApi(
  client: AxiosInstance,
  proprietarioId: string,
) {
  return client.get<AgendamentoListagemDto[]>(`/proprietarios/${proprietarioId}/agendamentos`);
}

export function confirmarAgendamentoApi(
  client: AxiosInstance,
  agendamentoId: string,
  data: AcaoAgendamentoRequest,
) {
  return client.post<AgendamentoDto>(`/agendamentos/${agendamentoId}/confirmar`, data);
}

export function cancelarAgendamentoApi(
  client: AxiosInstance,
  agendamentoId: string,
  data: AcaoAgendamentoRequest,
) {
  return client.post<AgendamentoDto>(`/agendamentos/${agendamentoId}/cancelar`, data);
}

export function remarcarAgendamentoApi(
  client: AxiosInstance,
  agendamentoId: string,
  data: RemarcarAgendamentoRequest,
) {
  return client.post<AgendamentoDto>(`/agendamentos/${agendamentoId}/remarcar`, data);
}
