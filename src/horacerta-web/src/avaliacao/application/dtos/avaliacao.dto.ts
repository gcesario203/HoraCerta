export type AvaliacaoDto = {
  agendamentoId: string;
  proprietarioId: string;
  nota: number;
  comentario: string | null;
  dataAvaliacao: string;
};

export type AvaliarAgendamentoRequest = {
  proprietarioId: string;
  nota: number;
  comentario?: string | null;
};
