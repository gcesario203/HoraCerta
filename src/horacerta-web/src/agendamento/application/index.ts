import { agendamentoRepository } from '../infrastructure/api/agendamento.repository';
import type {
  AcaoAgendamentoRequest,
  IniciarAgendamentoRequest,
  RemarcarAgendamentoRequest,
} from './dtos/agendamento.dto';

export const iniciarAgendamentoUseCase = {
  execute: (data: IniciarAgendamentoRequest) => agendamentoRepository.iniciar(data),
};

export const listarAgendamentosClienteUseCase = {
  execute: (clienteId: string) => agendamentoRepository.listarPorCliente(clienteId),
};

export const listarAgendamentosProprietarioUseCase = {
  execute: (proprietarioId: string) => agendamentoRepository.listarPorProprietario(proprietarioId),
};

export const confirmarAgendamentoUseCase = {
  execute: (agendamentoId: string, data: AcaoAgendamentoRequest) =>
    agendamentoRepository.confirmar(agendamentoId, data),
};

export const cancelarAgendamentoUseCase = {
  execute: (agendamentoId: string, data: AcaoAgendamentoRequest) =>
    agendamentoRepository.cancelar(agendamentoId, data),
};

export const remarcarAgendamentoUseCase = {
  execute: (agendamentoId: string, data: RemarcarAgendamentoRequest) =>
    agendamentoRepository.remarcar(agendamentoId, data),
};
