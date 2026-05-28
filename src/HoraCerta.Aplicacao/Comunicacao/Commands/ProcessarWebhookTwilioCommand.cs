namespace HoraCerta.Aplicacao.Comunicacao.Commands;

public record ProcessarWebhookTwilioCommand(
    string MessageSid,
    string From,
    string Body,
    string? ProprietarioIdHint);
