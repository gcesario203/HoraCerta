using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Aplicacao.Agendamento.Commands;
using HoraCerta.Aplicacao.Agendamento.Handlers;
using HoraCerta.Aplicacao.Agendamento.Queries;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Proprietario;
using HoraCerta.Infaestrutura.Persistencia;
using HoraCerta.Infaestrutura.Repositorio;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace HoraCerta.Testes.Integracao.Agendamento;

[TestFixture]
public class ListarAgendamentosPersistenciaTests
{
  private HoraCertaDbContext _context = null!;
  private EfProprietarioRepositorio _proprietarioRepositorio = null!;
  private EfClienteRepositorio _clienteRepositorio = null!;
  private ColetorDomainEventDispatcher _dispatcher = null!;

  [SetUp]
  public void SetUp()
  {
    var options = new DbContextOptionsBuilder<HoraCertaDbContext>()
      .UseInMemoryDatabase(Guid.NewGuid().ToString())
      .Options;

    _context = new HoraCertaDbContext(options);
    _proprietarioRepositorio = new EfProprietarioRepositorio(_context);
    _clienteRepositorio = new EfClienteRepositorio(_context);
    _dispatcher = new ColetorDomainEventDispatcher();
  }

  [Test]
  public void AposIniciarAgendamento_ListagensClienteEProprietario_DevemRetornarRegistro()
  {
    var proprietario = new ProprietarioEntidade("Barbearia EF");
    proprietario.GerenciadorProcedimentos.CriarProcedimento("Corte EF", 40m, TimeSpan.FromMinutes(30));
    var cliente = new ClienteEntidade("Maria EF", "(11) 98888-7777");

    _proprietarioRepositorio.Salvar(proprietario);
    _clienteRepositorio.Salvar(cliente);

    var inicio = DateTime.UtcNow.AddDays(2);
    var slot = new CriarSlotDisponivelHandler(_proprietarioRepositorio, _dispatcher)
      .Executar(new CriarSlotDisponivelCommand(proprietario.Id, inicio));

    var procedimento = proprietario.GerenciadorProcedimentos.RecuperarProcedimentos().First();

    new IniciarAgendamentoHandler(_proprietarioRepositorio, _clienteRepositorio, _dispatcher)
      .Executar(new IniciarAgendamentoCommand(
        proprietario.Id,
        cliente.Id,
        procedimento.Id,
        slot.Id));

    var listaCliente = new ListarAgendamentosClienteHandler(
        _clienteRepositorio,
        _proprietarioRepositorio)
      .Executar(new ListarAgendamentosClienteQuery(cliente.Id, proprietario.Id));

    var listaProprietario = new ListarAgendamentosProprietarioHandler(
        _proprietarioRepositorio,
        _clienteRepositorio)
      .Executar(new ListarAgendamentosProprietarioQuery(proprietario.Id));

    Assert.That(listaCliente, Has.Count.EqualTo(1));
    Assert.That(listaCliente.First().ProcedimentoNome, Is.EqualTo("Corte EF"));

    Assert.That(listaProprietario, Has.Count.EqualTo(1));
    Assert.That(listaProprietario.First().ClienteNome, Is.EqualTo("Maria EF"));
    Assert.That(listaProprietario.First().ProcedimentoNome, Is.EqualTo("Corte EF"));
  }
}
