namespace HoraCerta.Api.Contratos;

public record AgendamentoClienteListagemResposta(
    string AgendamentoId,
    string ProcedimentoNome,
    DateTime? SlotInicio,
    string Estado);
