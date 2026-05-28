using System.Net.Http.Headers;
using System.Text;
using HoraCerta.Aplicacao.Comunicacao;
using HoraCerta.Aplicacao.Comunicacao.Ports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HoraCerta.Infaestrutura.Comunicacao.ProvedorTwilio;

public class TwilioWhatsAppEnviador : IEnviadorWhatsApp
{
    private readonly TwilioOptions _options;
    private readonly ILogger<TwilioWhatsAppEnviador> _logger;
    private readonly HttpClient _httpClient;

    public TwilioWhatsAppEnviador(
        IOptions<TwilioOptions> options,
        ILogger<TwilioWhatsAppEnviador> logger,
        IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("TwilioWhatsAppEnviador");
    }

    public async Task EnviarAsync(string telefoneDestino, string corpo, CancellationToken cancellationToken = default)
    {
        var destino = telefoneDestino.StartsWith("whatsapp:", StringComparison.OrdinalIgnoreCase)
            ? telefoneDestino
            : $"whatsapp:{telefoneDestino}";

        var from = _options.WhatsAppFrom.StartsWith("whatsapp:", StringComparison.OrdinalIgnoreCase)
            ? _options.WhatsAppFrom
            : $"whatsapp:{_options.WhatsAppFrom}";

        _logger.LogInformation(
            "Enviando WhatsApp via Twilio para {Telefone}",
            TelefoneLog.Sanitizar(telefoneDestino));

        var url = $"https://api.twilio.com/2010-04-01/Accounts/{_options.AccountSid}/Messages.json";
        var credenciais = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{_options.AccountSid}:{_options.AuthToken}"));

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credenciais);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["To"] = destino,
            ["From"] = from,
            ["Body"] = corpo
        });

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Twilio retornou {(int)response.StatusCode}: {erro}");
        }
    }
}
