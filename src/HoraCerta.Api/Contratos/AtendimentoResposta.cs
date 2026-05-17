namespace HoraCerta.Api.Contratos;

public record AtendimentoResposta(
    string Id,
    string AgendamentoId,
    decimal ValorNegociado,
    string Estado);
