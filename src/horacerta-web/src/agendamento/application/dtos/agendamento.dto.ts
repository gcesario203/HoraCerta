export type AgendamentoDto = {
  id: string;
  clienteId: string;
  procedimentoId: string;
  slotHorarioId: string | null;
  estado: string;
  reagendamentoId: string | null;
};

export type AgendamentoClienteListagemDto = {
  agendamentoId: string;
  procedimentoNome: string;
  slotInicio: string | null;
  estado: string;
};

export type AgendamentoListagemDto = {
  agendamentoId: string;
  clienteId: string;
  clienteNome: string;
  procedimentoNome: string;
  slotInicio: string;
  estado: string;
};

export type IniciarAgendamentoRequest = {
  proprietarioId: string;
  clienteId: string;
  procedimentoId: string;
  slotHorarioId: string;
};

export type AcaoAgendamentoRequest = {
  proprietarioId: string;
  clienteId: string;
};

export type RemarcarAgendamentoRequest = AcaoAgendamentoRequest & {
  novoSlotHorarioId: string;
};
