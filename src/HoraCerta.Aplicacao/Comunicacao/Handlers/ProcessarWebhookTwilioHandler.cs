using HoraCerta.Aplicacao.Cliente.Commands;
using HoraCerta.Aplicacao.Cliente.Handlers;
using HoraCerta.Aplicacao.Comunicacao;
using HoraCerta.Aplicacao.Comunicacao.Bot;
using HoraCerta.Aplicacao.Comunicacao.Commands;
using HoraCerta.Aplicacao.Comunicacao.Enums;
using HoraCerta.Aplicacao.Comunicacao.Ports;
using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Comunicacao.Handlers;

public class ProcessarWebhookTwilioHandler
{
    private static readonly string[] PalavrasOptOut = ["SAIR", "PARAR", "STOP"];

    private readonly IWebhookTwilioProcessadoRepositorio _processadoRepositorio;
    private readonly INormalizadorTelefone _normalizador;
    private readonly IConversaBotLock _lock;
    private readonly IOrquestradorBotAgendamento _orquestrador;
    private readonly IEnviadorWhatsApp _enviador;
    private readonly IEnfileiradorMensagemWhatsApp _enfileirador;
    private readonly RegistrarOptOutWhatsAppHandler _optOutHandler;

    public ProcessarWebhookTwilioHandler(
        IWebhookTwilioProcessadoRepositorio processadoRepositorio,
        INormalizadorTelefone normalizador,
        IConversaBotLock lockService,
        IOrquestradorBotAgendamento orquestrador,
        IEnviadorWhatsApp enviador,
        IEnfileiradorMensagemWhatsApp enfileirador,
        RegistrarOptOutWhatsAppHandler optOutHandler)
    {
        _processadoRepositorio = processadoRepositorio;
        _normalizador = normalizador;
        _lock = lockService;
        _orquestrador = orquestrador;
        _enviador = enviador;
        _enfileirador = enfileirador;
        _optOutHandler = optOutHandler;
    }

    public async Task ExecutarAsync(ProcessarWebhookTwilioCommand command, CancellationToken cancellationToken = default)
    {
        if (_processadoRepositorio.JaProcessado(command.MessageSid))
            return;

        var telefone = _normalizador.Normalizar(command.From);
        var texto = command.Body.Trim();

        if (PalavrasOptOut.Contains(texto, StringComparer.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(command.ProprietarioIdHint)
            && Guid.TryParse(command.ProprietarioIdHint, out _))
        {
            _optOutHandler.Executar(
                new RegistrarOptOutWhatsAppCommand(new IdEntidade(command.ProprietarioIdHint), telefone));

            await EnviarRespostaAsync(
                telefone,
                command.ProprietarioIdHint!,
                MensagensWhatsAppTemplates.OptOutConfirmado(),
                command.MessageSid,
                cancellationToken);
            return;
        }

        var proprietarioHint = command.ProprietarioIdHint ?? string.Empty;

        using (await _lock.AdquirirAsync(telefone, proprietarioHint, cancellationToken))
        {
            var resposta = await _orquestrador.ProcessarMensagemAsync(
                telefone,
                proprietarioHint,
                texto,
                cancellationToken);

            await EnviarRespostaAsync(telefone, proprietarioHint, resposta, command.MessageSid, cancellationToken);
        }
    }

    private async Task EnviarRespostaAsync(
        string telefone,
        string proprietarioId,
        string corpo,
        string messageSid,
        CancellationToken cancellationToken)
    {
        try
        {
            await _enviador.EnviarAsync(telefone, corpo, cancellationToken);
        }
        catch
        {
            _enfileirador.Enfileirar(
                TipoMensagemOutbox.RespostaBot,
                telefone,
                string.IsNullOrWhiteSpace(proprietarioId) ? "desconhecido" : proprietarioId,
                corpo,
                $"BotResposta:{messageSid}");
        }

        _processadoRepositorio.MarcarProcessado(messageSid);
    }
}
