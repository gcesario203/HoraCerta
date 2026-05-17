using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao.Agendamento.Commands;
using HoraCerta.Aplicacao.Agendamento.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Cliente.Eventos;
using HoraCerta.Dominio.Proprietario.Eventos;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Proprietario;
using HoraCerta.Infaestrutura.Repositorio;
using NUnit.Framework;

namespace HoraCerta.Testes.Integracao.Agendamento;

[TestFixture]
public class FluxoAgendamentoTests
{
    private InMemoryProprietarioRepositorio _proprietarioRepositorio = null!;
    private InMemoryClienteRepositorio _clienteRepositorio = null!;
    private ColetorDomainEventDispatcher _dispatcher = null!;
    private ProprietarioEntidade _proprietario = null!;
    private ClienteEntidade _cliente = null!;

    [SetUp]
    public void SetUp()
    {
        _proprietarioRepositorio = new InMemoryProprietarioRepositorio();
        _clienteRepositorio = new InMemoryClienteRepositorio();
        _dispatcher = new ColetorDomainEventDispatcher();

        _proprietario = new ProprietarioEntidade("Barbearia Teste");
        _proprietario.GerenciadorProcedimentos.CriarProcedimento("Corte", 50m, TimeSpan.FromMinutes(30));

        _cliente = new ClienteEntidade("Maria", "(11) 99999-9999");

        _proprietarioRepositorio.Salvar(_proprietario);
        _clienteRepositorio.Salvar(_cliente);
    }

    [Test]
    public void FluxoCompleto_IniciarConfirmarRegistrarAtendimento()
    {
        var inicio = DateTime.Now.AddDays(1);

        var slot = new CriarSlotDisponivelHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarSlotDisponivelCommand(_proprietario.Id, inicio));

        var procedimento = _proprietario.GerenciadorProcedimentos.RecuperarProcedimentos().First();

        var agendamento = new IniciarAgendamentoHandler(_proprietarioRepositorio, _clienteRepositorio, _dispatcher)
            .Executar(new IniciarAgendamentoCommand(
                _proprietario.Id,
                _cliente.Id,
                procedimento.Id,
                slot.Id));

        Assert.That(agendamento.EstadoAtual(), Is.EqualTo(EstadoAgendamento.PENDENTE));
        Assert.That(_dispatcher.EventosDisparados.OfType<AgendamentoIniciadoEvent>(), Is.Not.Empty);

        _dispatcher.Limpar();

        var confirmado = new ConfirmarAgendamentoHandler(_clienteRepositorio, _dispatcher)
            .Executar(new ConfirmarAgendamentoCommand(_cliente.Id, agendamento.Id));

        Assert.That(confirmado.EstadoAtual(), Is.EqualTo(EstadoAgendamento.CONFIRMADO));
        Assert.That(_dispatcher.EventosDisparados.OfType<AgendamentoConfirmadoEvent>(), Is.Not.Empty);

        _dispatcher.Limpar();

        var atendimento = new RegistrarAtendimentoHandler(_proprietarioRepositorio, _clienteRepositorio, _dispatcher)
            .Executar(new RegistrarAtendimentoCommand(_proprietario.Id, _cliente.Id, agendamento.Id));

        Assert.That(atendimento.EstadoAtual(), Is.EqualTo(EstadoAtendimento.PENDENTE));
        Assert.That(confirmado.EstadoAtual(), Is.EqualTo(EstadoAgendamento.FINALIZADO));
        Assert.That(_dispatcher.EventosDisparados.OfType<AtendimentoRegistradoEvent>(), Is.Not.Empty);
    }

    [Test]
    public void CancelarAgendamento_DeveLiberarSlotEDispararEvento()
    {
        var inicio = DateTime.Now.AddDays(2);

        var slot = new CriarSlotDisponivelHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarSlotDisponivelCommand(_proprietario.Id, inicio));

        var procedimento = _proprietario.GerenciadorProcedimentos.RecuperarProcedimentos().First();

        var agendamento = new IniciarAgendamentoHandler(_proprietarioRepositorio, _clienteRepositorio, _dispatcher)
            .Executar(new IniciarAgendamentoCommand(
                _proprietario.Id,
                _cliente.Id,
                procedimento.Id,
                slot.Id));

        new ConfirmarAgendamentoHandler(_clienteRepositorio, _dispatcher)
            .Executar(new ConfirmarAgendamentoCommand(_cliente.Id, agendamento.Id));

        _dispatcher.Limpar();

        new CancelarAgendamentoHandler(_proprietarioRepositorio, _clienteRepositorio, _dispatcher)
            .Executar(new CancelarAgendamentoCommand(_proprietario.Id, _cliente.Id, agendamento.Id));

        var proprietarioPersistido = _proprietarioRepositorio.BuscarPorId(_proprietario.Id)!;
        var slotPersistido = proprietarioPersistido.Horarios.First(s => s.Id.Valor == slot.Id.Valor);

        Assert.That(slotPersistido.Status, Is.EqualTo(StatusSlotAgendamento.DISPONIVEL));
        Assert.That(_dispatcher.EventosDisparados.OfType<AgendamentoCanceladoEvent>(), Is.Not.Empty);
    }

    [Test]
    public void RemarcarAgendamento_DeveCriarNovoAgendamentoPendenteEDispararEvento()
    {
        var inicio1 = DateTime.Now.AddDays(3);
        var inicio2 = DateTime.Now.AddDays(3).AddHours(2);

        var slot1 = new CriarSlotDisponivelHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarSlotDisponivelCommand(_proprietario.Id, inicio1));

        var slot2 = new CriarSlotDisponivelHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarSlotDisponivelCommand(_proprietario.Id, inicio2));

        var procedimento = _proprietario.GerenciadorProcedimentos.RecuperarProcedimentos().First();

        var agendamento = new IniciarAgendamentoHandler(_proprietarioRepositorio, _clienteRepositorio, _dispatcher)
            .Executar(new IniciarAgendamentoCommand(
                _proprietario.Id,
                _cliente.Id,
                procedimento.Id,
                slot1.Id));

        new ConfirmarAgendamentoHandler(_clienteRepositorio, _dispatcher)
            .Executar(new ConfirmarAgendamentoCommand(_cliente.Id, agendamento.Id));

        _dispatcher.Limpar();

        var remarcado = new RemarcarAgendamentoHandler(_proprietarioRepositorio, _clienteRepositorio, _dispatcher)
            .Executar(new RemarcarAgendamentoCommand(
                _proprietario.Id,
                _cliente.Id,
                agendamento.Id,
                slot2.Id));

        Assert.That(agendamento.EstadoAtual(), Is.EqualTo(EstadoAgendamento.REMARCADO));
        Assert.That(remarcado.EstadoAtual(), Is.EqualTo(EstadoAgendamento.PENDENTE));
        Assert.That(remarcado.Reagendamento, Is.Not.Null);
        Assert.That(_dispatcher.EventosDisparados.OfType<AgendamentoRemarcadoEvent>(), Is.Not.Empty);
    }
}
