using HoraCerta.Api.Comunicacao;
using HoraCerta.Aplicacao.Comunicacao;
using HoraCerta.Aplicacao.Comunicacao.Commands;
using HoraCerta.Aplicacao.Comunicacao.Handlers;
using Microsoft.Extensions.Options;

namespace HoraCerta.Api.Endpoints;

public static class WebhooksTwilioEndpoints
{
    public static RouteGroupBuilder MapWebhooksTwilio(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/webhooks/twilio")
            .WithTags("Webhooks Twilio")
            .AllowAnonymous();

        group.MapPost("/whatsapp", async (
            HttpRequest request,
            ProcessarWebhookTwilioHandler handler,
            TwilioAssinaturaWebhook assinatura,
            IOptions<TwilioOptions> twilioOptions) =>
        {
            var form = await request.ReadFormAsync();
            var messageSid = form["MessageSid"].ToString();
            var from = form["From"].ToString();
            var body = form["Body"].ToString();

            if (string.IsNullOrWhiteSpace(messageSid) || string.IsNullOrWhiteSpace(from))
                return Results.BadRequest();

            var baseUrl = twilioOptions.Value.WebhookBaseUrl.TrimEnd('/');
            var path = request.Path.Value ?? "/api/webhooks/twilio/whatsapp";
            var urlCompleta = string.IsNullOrWhiteSpace(baseUrl)
                ? $"{request.Scheme}://{request.Host}{path}"
                : $"{baseUrl}{path}";

            if (!assinatura.Validar(request, urlCompleta, form))
                return Results.Unauthorized();

            var proprietarioHint = ExtrairProprietarioId(body);

            await handler.ExecutarAsync(new ProcessarWebhookTwilioCommand(
                messageSid,
                from,
                body,
                proprietarioHint));

            return Results.Ok();
        });

        group.MapGet("/whatsapp", () => Results.Ok(new { status = "webhook ativo" }));

        return group;
    }

    private static string? ExtrairProprietarioId(string body)
    {
        var match = System.Text.RegularExpressions.Regex.Match(
            body,
            @"HC[-\s]?([0-9a-fA-F\-]{36})",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return match.Success ? match.Groups[1].Value : null;
    }
}
