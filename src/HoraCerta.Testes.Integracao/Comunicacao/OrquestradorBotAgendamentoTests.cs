using HoraCerta.Aplicacao.Agendamento.Queries;
using HoraCerta.Aplicacao.Agendamento.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Proprietario;
using NUnit.Framework;

namespace HoraCerta.Testes.Integracao.Comunicacao;

[TestFixture]
public class OrquestradorBotAgendamentoTests : ComunicacaoIntegracaoFixture
{
    private const string TelefoneBot = "+5511988776655";

    private ProprietarioEntidade _proprietario = null!;

    [SetUp]
    public void SetUpOrquestrador()
    {
        _proprietario = new ProprietarioEntidade("Barbearia Bot");
        _proprietario.GerenciadorProcedimentos.CriarProcedimento("Corte Bot", 45m, TimeSpan.FromMinutes(30));
        ProprietarioRepositorio.Salvar(_proprietario);

        new CriarSlotDisponivelHandler(ProprietarioRepositorio, Dispatcher)
            .Executar(new CriarSlotDisponivelCommand(
                _proprietario.Id,
                DateTime.UtcNow.AddDays(1)));
    }

    [Test]
    public async Task SemCodigoEstabelecimento_DevePedirHcId()
    {
        var orquestrador = CriarOrquestrador();
        var resposta = await orquestrador.ProcessarMensagemAsync(TelefoneBot, "", "Olá");

        Assert.That(resposta, Does.Contain("HC-"));
        Assert.That(resposta, Does.Contain("código do estabelecimento"));
    }

    [Test]
    public async Task FluxoCompleto_NovoCliente_DeveCriarAgendamentoPendente()
    {
        var orquestrador = CriarOrquestrador();
        var id = _proprietario.Id.Valor;

        var r1 = await orquestrador.ProcessarMensagemAsync(TelefoneBot, "", $"HC-{id}");
        Assert.That(r1, Does.Contain("nome completo"));

        var r2 = await orquestrador.ProcessarMensagemAsync(TelefoneBot, id, "Carlos Bot");
        Assert.That(r2, Does.Contain("Confirma o nome"));

        var r3 = await orquestrador.ProcessarMensagemAsync(TelefoneBot, id, "SIM");
        Assert.That(r3, Does.Contain("Escolha o procedimento"));
        Assert.That(r3, Does.Contain("Corte Bot"));

        var r4 = await orquestrador.ProcessarMensagemAsync(TelefoneBot, id, "1");
        Assert.That(r4, Does.Contain("Escolha o horário"));

        var r5 = await orquestrador.ProcessarMensagemAsync(TelefoneBot, id, "1");
        Assert.That(r5, Does.Contain("Confirmar agendamento"));

        var r6 = await orquestrador.ProcessarMensagemAsync(TelefoneBot, id, "SIM");
        Assert.That(r6, Does.Contain("Pedido enviado"));

        var cliente = ClienteRepositorio
            .ListarComProprietario(_proprietario)
            .First(c => c.Nome == "Carlos Bot");

        var agendamentos = new ListarAgendamentosClienteHandler(ClienteRepositorio, ProprietarioRepositorio)
            .Executar(new ListarAgendamentosClienteQuery(cliente.Id, _proprietario.Id));

        Assert.That(agendamentos, Has.Count.EqualTo(1));
        Assert.That(agendamentos.First().Estado, Is.EqualTo(nameof(EstadoAgendamento.PENDENTE)));
    }

    [Test]
    public async Task ClienteJaCadastradoComMesmoTelefone_DevePularIdentificacao()
    {
        var telefoneNormalizado = Normalizador.Normalizar(TelefoneBot);
        var cliente = new ClienteEntidade("Cliente Existente", telefoneNormalizado);
        ClienteRepositorio.Salvar(cliente);

        var orquestrador = CriarOrquestrador();
        var id = _proprietario.Id.Valor;

        var resposta = await orquestrador.ProcessarMensagemAsync(TelefoneBot, "", $"HC-{id}");

        Assert.That(resposta, Does.Contain("Escolha o procedimento"));
        Assert.That(resposta, Does.Not.Contain("nome completo"));
    }
}
