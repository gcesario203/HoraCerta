using HoraCerta.Aplicacao.Comunicacao.Enums;

namespace HoraCerta.Aplicacao.Comunicacao.Ports;

public interface IEnfileiradorMensagemWhatsApp
{
    void Enfileirar(
        TipoMensagemOutbox tipo,
        string telefoneDestino,
        string proprietarioId,
        string corpo,
        string? idempotencyKey = null,
        string? payloadJson = null,
        DateTime? enviarApos = null);
}
