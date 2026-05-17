namespace HoraCerta.Aplicacao.Integracao.Lembretes;

public record LembretePendente(
    string Id,
    string ProprietarioId,
    string ClienteId,
    string AgendamentoId,
    string TelefoneCliente,
    DateTime SlotInicio,
    DateTime EnviarEm,
    LembreteStatus Status);
