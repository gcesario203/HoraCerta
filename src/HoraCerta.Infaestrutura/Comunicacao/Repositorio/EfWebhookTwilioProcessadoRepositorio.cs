using HoraCerta.Aplicacao.Comunicacao.Ports;
using HoraCerta.Infaestrutura.Persistencia;
using HoraCerta.Infaestrutura.Persistencia.Registros;

namespace HoraCerta.Infaestrutura.Comunicacao.Repositorio;

public class EfWebhookTwilioProcessadoRepositorio : IWebhookTwilioProcessadoRepositorio
{
    private readonly HoraCertaDbContext _context;

    public EfWebhookTwilioProcessadoRepositorio(HoraCertaDbContext context)
    {
        _context = context;
    }

    public bool JaProcessado(string messageSid)
        => _context.WebhooksTwilioProcessados.Any(x => x.MessageSid == messageSid);

    public void MarcarProcessado(string messageSid)
    {
        if (JaProcessado(messageSid))
            return;

        _context.WebhooksTwilioProcessados.Add(new WebhookTwilioProcessadoRegistro
        {
            MessageSid = messageSid,
            ProcessadoEm = DateTime.UtcNow
        });
        _context.SaveChanges();
    }
}
