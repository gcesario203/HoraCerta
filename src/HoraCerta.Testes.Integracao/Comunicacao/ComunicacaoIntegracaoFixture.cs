using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Aplicacao.Agendamento.Handlers;
using HoraCerta.Aplicacao.Cliente.Handlers;
using HoraCerta.Aplicacao.Comunicacao;
using HoraCerta.Aplicacao.Comunicacao.Bot;
using HoraCerta.Aplicacao.Comunicacao.Outbox;
using HoraCerta.Aplicacao.Comunicacao.Ports;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Infaestrutura.Comunicacao;
using HoraCerta.Infaestrutura.Comunicacao.Outbox;
using HoraCerta.Infaestrutura.Comunicacao.Repositorio;
using HoraCerta.Infaestrutura.Persistencia;
using HoraCerta.Infaestrutura.Repositorio;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace HoraCerta.Testes.Integracao.Comunicacao;

public abstract class ComunicacaoIntegracaoFixture
{
    protected HoraCertaDbContext Context = null!;
    protected EfProprietarioRepositorio ProprietarioRepositorio = null!;
    protected EfClienteRepositorio ClienteRepositorio = null!;
    protected EfMensagemOutboxRepositorio OutboxRepositorio = null!;
    protected EfSessaoConversaRepositorio SessaoRepositorio = null!;
    protected EfWebhookTwilioProcessadoRepositorio WebhookRepositorio = null!;
    protected ColetorDomainEventDispatcher Dispatcher = null!;
    protected INormalizadorTelefone Normalizador = null!;
    protected IEnfileiradorMensagemWhatsApp Enfileirador = null!;

    [SetUp]
    public void BaseSetUp()
    {
        var options = new DbContextOptionsBuilder<HoraCertaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new HoraCertaDbContext(options);
        ProprietarioRepositorio = new EfProprietarioRepositorio(Context);
        ClienteRepositorio = new EfClienteRepositorio(Context);
        OutboxRepositorio = new EfMensagemOutboxRepositorio(Context);
        SessaoRepositorio = new EfSessaoConversaRepositorio(Context);
        WebhookRepositorio = new EfWebhookTwilioProcessadoRepositorio(Context);
        Dispatcher = new ColetorDomainEventDispatcher();
        Normalizador = new NormalizadorTelefoneE164();
        Enfileirador = new EnfileiradorMensagemWhatsApp(OutboxRepositorio, Normalizador);
    }

    protected OrquestradorBotAgendamento CriarOrquestrador()
        => new(
            SessaoRepositorio,
            ProprietarioRepositorio,
            new ListarProcedimentosAtivosHandler(ProprietarioRepositorio),
            new ListarSlotsDisponiveisHandler(ProprietarioRepositorio),
            new BuscarClientePorTelefoneHandler(ProprietarioRepositorio, ClienteRepositorio, Normalizador),
            new CriarClienteHandler(ClienteRepositorio, Normalizador),
            new IniciarAgendamentoHandler(ProprietarioRepositorio, ClienteRepositorio, Dispatcher),
            Options.Create(new TwilioOptions { SessaoExpiracaoHoras = 24 }));

    protected int ContarOutboxPendentes()
        => Context.MensagensOutbox.Count(x => x.Status == "Pendente");
}
