export type ProcedimentoCatalogoDto = {
  id: string;
  nome: string;
  valor: number;
  tempoEstimadoMinutos: number;
};

export type SlotCatalogoDto = {
  id: string;
  inicio: string;
  fim: string | null;
};

export type EstabelecimentoCatalogoDto = {
  id: string;
  nome: string;
  quantidadeProcedimentos: number;
  quantidadeHorariosDisponiveis: number;
  proximoHorarioInicio: string | null;
  precoMinimo: number | null;
  precoMaximo: number | null;
  procedimentos: ProcedimentoCatalogoDto[];
  proximosHorarios: SlotCatalogoDto[];
};
