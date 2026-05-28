using HoraCerta.Aplicacao.Agendamento.Commands;
using HoraCerta.Aplicacao.Agendamento.Handlers;
using HoraCerta.Aplicacao.Comunicacao.Eventos;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Cliente.Eventos;
using HoraCerta.Dominio.Proprietario;
using NUnit.Framework;

namespace HoraCerta.Testes.Integracao.Comunicacao;

[TestFixture]
public class ConfirmacaoOutboxIntegracaoTests : ComunicacaoIntegracaoFixture
{
    [Test]
    public void ConfirmarAgendamento_DeveEnfileirarNotificacaoWhatsAppNaOutbox()
    {
        var proprietario = new ProprietarioEntidade("Salão Outbox");
        proprietario.GerenciadorProcedimentos.CriarProcedimento("Corte", 50m, TimeSpan.FromMinutes(30));
        var cliente = new ClienteEntidade("Ana", "(11) 97777-6666");

        ProprietarioRepositorio.Salvar(proprietario);
        ClienteRepositorio.Salvar(cliente);

        var slot = new CriarSlotDisponivelHandler(ProprietarioRepositorio, Dispatcher)
            .Executar(new CriarSlotDisponivelCommand(proprietario.Id, DateTime.UtcNow.AddDays(1)));

        var procedimento = proprietario.GerenciadorProcedimentos.RecuperarProcedimentos().First();

        var agendamento = new IniciarAgendamentoHandler(ProprietarioRepositorio, ClienteRepositorio, Dispatcher)
            .Executar(new IniciarAgendamentoCommand(
                proprietario.Id,
                cliente.Id,
                procedimento.Id,
                slot.Id));

        new ConfirmarAgendamentoHandler(ProprietarioRepositorio, ClienteRepositorio, Dispatcher)
            .Executar(new ConfirmarAgendamentoCommand(proprietario.Id, cliente.Id, agendamento.Id));

        var evento = Dispatcher.EventosDisparados.OfType<AgendamentoConfirmadoEvent>().Single();

        new EnviarNotificacaoConfirmacaoWhatsAppHandler(
                Enfileirador,
                ClienteRepositorio,
                ProprietarioRepositorio)
            .Handle(evento);

        Assert.That(Context.MensagensOutbox.Count(), Is.EqualTo(1));
        var mensagem = Context.MensagensOutbox.Single();
        Assert.That(mensagem.Tipo, Is.EqualTo("NotificacaoConfirmacao"));
        Assert.That(mensagem.IdempotencyKey, Is.EqualTo($"Confirmacao:{agendamento.Id.Valor}"));
        Assert.That(mensagem.Status, Is.EqualTo("Pendente"));
        Assert.That(mensagem.Corpo, Does.Contain("confirmado"));
    }

    [Test]
    public void ConfirmarAgendamento_ComOptOut_NaoDeveEnfileirarNaOutbox()
    {
        var proprietario = new ProprietarioEntidade("Salão OptOut");
        proprietario.GerenciadorProcedimentos.CriarProcedimento("Barba", 30m, TimeSpan.FromMinutes(20));
        var cliente = new ClienteEntidade("Pedro", "(11) 96666-5555", optOutWhatsApp: true);

        ProprietarioRepositorio.Salvar(proprietario);
        ClienteRepositorio.Salvar(cliente);

        var slot = new CriarSlotDisponivelHandler(ProprietarioRepositorio, Dispatcher)
            .Executar(new CriarSlotDisponivelCommand(proprietario.Id, DateTime.UtcNow.AddDays(2)));

        var procedimento = proprietario.GerenciadorProcedimentos.RecuperarProcedimentos().First();

        var agendamento = new IniciarAgendamentoHandler(ProprietarioRepositorio, ClienteRepositorio, Dispatcher)
            .Executar(new IniciarAgendamentoCommand(
                proprietario.Id,
                cliente.Id,
                procedimento.Id,
                slot.Id));

        new ConfirmarAgendamentoHandler(ProprietarioRepositorio, ClienteRepositorio, Dispatcher)
            .Executar(new ConfirmarAgendamentoCommand(proprietario.Id, cliente.Id, agendamento.Id));

        var evento = Dispatcher.EventosDisparados.OfType<AgendamentoConfirmadoEvent>().Single();

        new EnviarNotificacaoConfirmacaoWhatsAppHandler(
                Enfileirador,
                ClienteRepositorio,
                ProprietarioRepositorio)
            .Handle(evento);

        Assert.That(Context.MensagensOutbox, Is.Empty);
    }
}
