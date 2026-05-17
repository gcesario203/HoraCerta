export type ProcedimentoDto = {
  id: string;
  nome: string;
  valor: number;
  tempoEstimadoMinutos: number;
  estado: string;
};

export type CriarProcedimentoRequest = {
  nome: string;
  valor: number;
  tempoEstimadoMinutos: number;
};
