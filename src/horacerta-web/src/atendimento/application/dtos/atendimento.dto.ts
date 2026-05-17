export type AtendimentoDto = {
  id: string;
  agendamentoId: string;
  valorNegociado: number;
  estado: string;
};

export type RegistrarAtendimentoRequest = {
  proprietarioId: string;
  clienteId: string;
  valorNegociado?: number | null;
};

export type AlterarEstadoAtendimentoRequest = {
  estado: string;
};
