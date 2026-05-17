using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Aplicacao.Agendamento.Commands;
using HoraCerta.Aplicacao.Agendamento.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Queries;
using HoraCerta.Dominio;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Cliente.Eventos;
using HoraCerta.Dominio.Proprietario;
using HoraCerta.Infaestrutura.Repositorio;
using NUnit.Framework;

namespace HoraCerta.Testes.Integracao.Estabelecimento;

[TestFixture]
public class FluxoOnda1Tests
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
        _cliente = new ClienteEntidade("Maria", "(11) 99999-9999");

        _proprietarioRepositorio.Salvar(_proprietario);
        _clienteRepositorio.Salvar(_cliente);
    }

    [Test]
    public void FluxoOnda1_CadastrarProcedimentoListarCriarSlotIniciarEConfirmar()
    {
        var procedimento = new CriarProcedimentoHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarProcedimentoCommand(_proprietario.Id, "Corte", 50m, TimeSpan.FromMinutes(30)));

        var procedimentosAtivos = new ListarProcedimentosAtivosHandler(_proprietarioRepositorio)
            .Executar(new ListarProcedimentosAtivosQuery(_proprietario.Id));

        Assert.That(procedimentosAtivos, Has.Count.EqualTo(1));
        Assert.That(procedimentosAtivos.First().Id.Valor, Is.EqualTo(procedimento.Id.Valor));

        var inicio = DateTime.Now.AddDays(1);

        new CriarSlotDisponivelHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarSlotDisponivelCommand(_proprietario.Id, inicio));

        var slotsDisponiveis = new ListarSlotsDisponiveisHandler(_proprietarioRepositorio)
            .Executar(new ListarSlotsDisponiveisQuery(_proprietario.Id));

        Assert.That(slotsDisponiveis, Has.Count.EqualTo(1));

        var slot = slotsDisponiveis.First();

        _dispatcher.Limpar();

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
    }

    [Test]
    public void InativarProcedimento_NaoDeveAparecerNaListagemNemPermitirAgendamento()
    {
        var procedimento = new CriarProcedimentoHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarProcedimentoCommand(_proprietario.Id, "Barba", 30m, TimeSpan.FromMinutes(20)));

        new InativarProcedimentoHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new InativarProcedimentoCommand(_proprietario.Id, procedimento.Id));

        var procedimentosAtivos = new ListarProcedimentosAtivosHandler(_proprietarioRepositorio)
            .Executar(new ListarProcedimentosAtivosQuery(_proprietario.Id));

        Assert.That(procedimentosAtivos, Is.Empty);

        var inicio = DateTime.Now.AddDays(2);

        var slot = new CriarSlotDisponivelHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarSlotDisponivelCommand(_proprietario.Id, inicio));

        Assert.Throws<OperacaoInvalidaExcessao>(() =>
            new IniciarAgendamentoHandler(_proprietarioRepositorio, _clienteRepositorio, _dispatcher)
                .Executar(new IniciarAgendamentoCommand(
                    _proprietario.Id,
                    _cliente.Id,
                    procedimento.Id,
                    slot.Id)));
    }

    [Test]
    public void ListarSlotsDisponiveis_NaoDeveIncluirSlotReservado()
    {
        var procedimento = new CriarProcedimentoHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarProcedimentoCommand(_proprietario.Id, "Corte", 50m, TimeSpan.FromMinutes(30)));

        var inicio = DateTime.Now.AddDays(3);

        var slot = new CriarSlotDisponivelHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarSlotDisponivelCommand(_proprietario.Id, inicio));

        new IniciarAgendamentoHandler(_proprietarioRepositorio, _clienteRepositorio, _dispatcher)
            .Executar(new IniciarAgendamentoCommand(
                _proprietario.Id,
                _cliente.Id,
                procedimento.Id,
                slot.Id));

        var slotsDisponiveis = new ListarSlotsDisponiveisHandler(_proprietarioRepositorio)
            .Executar(new ListarSlotsDisponiveisQuery(_proprietario.Id));

        Assert.That(slotsDisponiveis, Is.Empty);
    }
}
