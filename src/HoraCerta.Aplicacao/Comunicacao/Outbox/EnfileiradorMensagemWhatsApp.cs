using HoraCerta.Aplicacao.Comunicacao.Dtos;
using HoraCerta.Aplicacao.Comunicacao.Enums;
using HoraCerta.Aplicacao.Comunicacao.Ports;

namespace HoraCerta.Aplicacao.Comunicacao.Outbox;

public class EnfileiradorMensagemWhatsApp : IEnfileiradorMensagemWhatsApp
{
    private readonly IMensagemOutboxRepositorio _repositorio;
    private readonly INormalizadorTelefone _normalizador;

    public EnfileiradorMensagemWhatsApp(
        IMensagemOutboxRepositorio repositorio,
        INormalizadorTelefone normalizador)
    {
        _repositorio = repositorio;
        _normalizador = normalizador;
    }

    public void Enfileirar(
        TipoMensagemOutbox tipo,
        string telefoneDestino,
        string proprietarioId,
        string corpo,
        string? idempotencyKey = null,
        string? payloadJson = null,
        DateTime? enviarApos = null)
    {
        if (!string.IsNullOrWhiteSpace(idempotencyKey) && _repositorio.ExistePorIdempotencyKey(idempotencyKey))
            return;

        var telefone = _normalizador.Normalizar(telefoneDestino);
        var agora = DateTime.UtcNow;

        _repositorio.Adicionar(new MensagemOutboxPendente(
            Guid.NewGuid().ToString(),
            tipo,
            telefone,
            proprietarioId,
            corpo,
            idempotencyKey,
            payloadJson,
            Tentativas: 0,
            ProximaTentativaEm: enviarApos ?? agora));
    }
}
