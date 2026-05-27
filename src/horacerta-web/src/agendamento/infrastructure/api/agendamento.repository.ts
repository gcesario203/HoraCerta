import { bffApiClient, publicApiClient } from '@/shared/infrastructure/http/axios-client';
import type {
  AcaoAgendamentoRequest,
  IniciarAgendamentoRequest,
  RemarcarAgendamentoRequest,
} from '../../application/dtos/agendamento.dto';
import {
  cancelarAgendamentoApi,
  confirmarAgendamentoApi,
  iniciarAgendamentoApi,
  listarAgendamentosClienteApi,
  listarAgendamentosProprietarioApi,
  remarcarAgendamentoApi,
} from './agendamento.api';

export class AgendamentoRepository {
  iniciar(data: IniciarAgendamentoRequest) {
    return iniciarAgendamentoApi(publicApiClient, data).then((r) => r.data);
  }

  listarPorCliente(clienteId: string, proprietarioId?: string) {
    return listarAgendamentosClienteApi(publicApiClient, clienteId, proprietarioId).then(
      (r) => r.data,
    );
  }

  listarPorProprietario(proprietarioId: string) {
    return listarAgendamentosProprietarioApi(bffApiClient, proprietarioId).then((r) => r.data);
  }

  confirmar(agendamentoId: string, data: AcaoAgendamentoRequest) {
    return confirmarAgendamentoApi(bffApiClient, agendamentoId, data).then((r) => r.data);
  }

  cancelar(agendamentoId: string, data: AcaoAgendamentoRequest) {
    return cancelarAgendamentoApi(bffApiClient, agendamentoId, data).then((r) => r.data);
  }

  remarcar(agendamentoId: string, data: RemarcarAgendamentoRequest) {
    return remarcarAgendamentoApi(bffApiClient, agendamentoId, data).then((r) => r.data);
  }
}

export const agendamentoRepository = new AgendamentoRepository();
