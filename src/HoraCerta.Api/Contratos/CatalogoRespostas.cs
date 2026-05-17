namespace HoraCerta.Api.Contratos;

public record ProcedimentoCatalogoResposta(
    string Id,
    string Nome,
    decimal Valor,
    int TempoEstimadoMinutos);

public record SlotCatalogoResposta(
    string Id,
    DateTime Inicio,
    DateTime? Fim);

public record EstabelecimentoCatalogoResposta(
    string Id,
    string Nome,
    int QuantidadeProcedimentos,
    int QuantidadeHorariosDisponiveis,
    DateTime? ProximoHorarioInicio,
    decimal? PrecoMinimo,
    decimal? PrecoMaximo,
    IReadOnlyList<ProcedimentoCatalogoResposta> Procedimentos,
    IReadOnlyList<SlotCatalogoResposta> ProximosHorarios);
