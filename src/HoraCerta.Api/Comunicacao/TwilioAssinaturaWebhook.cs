using HoraCerta.Aplicacao.Comunicacao;
using Microsoft.Extensions.Options;
using Twilio.Security;

namespace HoraCerta.Api.Comunicacao;

public class TwilioAssinaturaWebhook
{
    private readonly TwilioOptions _options;

    public TwilioAssinaturaWebhook(IOptions<TwilioOptions> options)
    {
        _options = options.Value;
    }

    public bool Validar(HttpRequest request, string urlCompleta, IFormCollection form)
    {
        if (!_options.ValidarAssinaturaWebhook || string.IsNullOrWhiteSpace(_options.AuthToken))
            return true;

        var assinatura = request.Headers["X-Twilio-Signature"].ToString();
        if (string.IsNullOrWhiteSpace(assinatura))
            return false;

        var parametros = form.ToDictionary(x => x.Key, x => x.Value.ToString());
        var validador = new RequestValidator(_options.AuthToken);
        return validador.Validate(urlCompleta, parametros, assinatura);
    }
}
