namespace HoraCerta.Api.Contratos;

public record ProcedimentoResposta(
    string Id,
    string Nome,
    decimal Valor,
    int TempoEstimadoMinutos,
    string Estado);
