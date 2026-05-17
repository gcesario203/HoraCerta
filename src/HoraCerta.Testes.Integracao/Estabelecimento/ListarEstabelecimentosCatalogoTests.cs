using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Queries;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Proprietario;
using HoraCerta.Infaestrutura.Repositorio;
using NUnit.Framework;

namespace HoraCerta.Testes.Integracao.Estabelecimento;

[TestFixture]
public class ListarEstabelecimentosCatalogoTests
{
    private InMemoryProprietarioRepositorio _proprietarioRepositorio = null!;
    private ColetorDomainEventDispatcher _dispatcher = null!;

    [SetUp]
    public void SetUp()
    {
        _proprietarioRepositorio = new InMemoryProprietarioRepositorio();
        _dispatcher = new ColetorDomainEventDispatcher();
    }

    [Test]
    public void ListarCatalogo_RetornaApenasComProcedimentoAtivoEHorarioFuturo()
    {
        var comOferta = new ProprietarioEntidade("Barbearia Centro");
        _proprietarioRepositorio.Salvar(comOferta);

        new CriarProcedimentoHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarProcedimentoCommand(comOferta.Id, "Corte", 50m, TimeSpan.FromMinutes(30)));

        new CriarSlotDisponivelHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarSlotDisponivelCommand(comOferta.Id, DateTime.Now.AddDays(1)));

        var semHorario = new ProprietarioEntidade("Salão Vazio");
        _proprietarioRepositorio.Salvar(semHorario);

        new CriarProcedimentoHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarProcedimentoCommand(semHorario.Id, "Escova", 80m, TimeSpan.FromMinutes(45)));

        var semProcedimento = new ProprietarioEntidade("Estúdio Novo");
        _proprietarioRepositorio.Salvar(semProcedimento);

        new CriarSlotDisponivelHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarSlotDisponivelCommand(semProcedimento.Id, DateTime.Now.AddDays(2)));

        var catalogo = new ListarEstabelecimentosCatalogoHandler(_proprietarioRepositorio)
            .Executar(new ListarEstabelecimentosCatalogoQuery());

        Assert.That(catalogo, Has.Count.EqualTo(1));
        Assert.That(catalogo[0].Nome, Is.EqualTo("Barbearia Centro"));
        Assert.That(catalogo[0].QuantidadeProcedimentos, Is.EqualTo(1));
        Assert.That(catalogo[0].QuantidadeHorariosDisponiveis, Is.EqualTo(1));
        Assert.That(catalogo[0].PrecoMinimo, Is.EqualTo(50m));
    }

    [Test]
    public void ListarCatalogo_FiltraPorNome()
    {
        var barbearia = new ProprietarioEntidade("Barbearia Norte");
        _proprietarioRepositorio.Salvar(barbearia);

        new CriarProcedimentoHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarProcedimentoCommand(barbearia.Id, "Barba", 30m, TimeSpan.FromMinutes(20)));

        new CriarSlotDisponivelHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarSlotDisponivelCommand(barbearia.Id, DateTime.Now.AddHours(3)));

        var spa = new ProprietarioEntidade("Spa Relax");
        _proprietarioRepositorio.Salvar(spa);

        new CriarProcedimentoHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarProcedimentoCommand(spa.Id, "Massagem", 120m, TimeSpan.FromMinutes(60)));

        new CriarSlotDisponivelHandler(_proprietarioRepositorio, _dispatcher)
            .Executar(new CriarSlotDisponivelCommand(spa.Id, DateTime.Now.AddHours(5)));

        var catalogo = new ListarEstabelecimentosCatalogoHandler(_proprietarioRepositorio)
            .Executar(new ListarEstabelecimentosCatalogoQuery("barbearia"));

        Assert.That(catalogo, Has.Count.EqualTo(1));
        Assert.That(catalogo[0].Nome, Does.Contain("Barbearia"));
    }
}
