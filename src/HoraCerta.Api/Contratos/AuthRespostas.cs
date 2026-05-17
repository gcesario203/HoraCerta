namespace HoraCerta.Api.Contratos;

public record LoginResposta(string Token, string ProprietarioId);

public record AgendamentoListagemResposta(
    string AgendamentoId,
    string ClienteId,
    string ClienteNome,
    string ProcedimentoNome,
    DateTime SlotInicio,
    string Estado);

public record AvaliacaoResposta(
    string AgendamentoId,
    string ProprietarioId,
    int Nota,
    string? Comentario,
    DateTime DataAvaliacao);
