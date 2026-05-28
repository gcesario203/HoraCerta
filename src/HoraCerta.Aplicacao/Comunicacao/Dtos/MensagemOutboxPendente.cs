using HoraCerta.Aplicacao.Comunicacao.Enums;

namespace HoraCerta.Aplicacao.Comunicacao.Dtos;

public record MensagemOutboxPendente(
    string Id,
    TipoMensagemOutbox Tipo,
    string TelefoneDestino,
    string ProprietarioId,
    string Corpo,
    string? IdempotencyKey,
    string? PayloadJson,
    int Tentativas,
    DateTime ProximaTentativaEm);
