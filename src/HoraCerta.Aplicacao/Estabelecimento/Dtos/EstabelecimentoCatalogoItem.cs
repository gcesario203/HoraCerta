namespace HoraCerta.Aplicacao.Estabelecimento.Dtos;

public record ProcedimentoCatalogoResumo(
    string Id,
    string Nome,
    decimal Valor,
    int TempoEstimadoMinutos);

public record SlotCatalogoResumo(
    string Id,
    DateTime Inicio,
    DateTime? Fim);

public record EstabelecimentoCatalogoItem(
    string Id,
    string Nome,
    int QuantidadeProcedimentos,
    int QuantidadeHorariosDisponiveis,
    DateTime? ProximoHorarioInicio,
    decimal? PrecoMinimo,
    decimal? PrecoMaximo,
    IReadOnlyList<ProcedimentoCatalogoResumo> Procedimentos,
    IReadOnlyList<SlotCatalogoResumo> ProximosHorarios);
