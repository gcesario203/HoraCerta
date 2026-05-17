using System.Net;
using System.Net.Http.Json;
using HoraCerta.Api.Contratos;
using HoraCerta.Testes.E2e.Infraestrutura;

namespace HoraCerta.Testes.E2e;

[TestFixture]
public class FluxoOnda1E2ETests : E2ETestBase
{
    [Test]
    public async Task FluxoCompleto_DeveCadastrarProcedimentoSlotIniciarEConfirmarAgendamento()
    {
        var (proprietario, cliente) = await CriarProprietarioEClienteAsync();
        var procedimento = await CriarProcedimentoAsync(proprietario.Id);
        var slot = await CriarSlotAsync(proprietario.Id, DateTime.UtcNow.AddDays(1));

        var listagemProcedimentos = await Client.GetFromJsonAsync<List<ProcedimentoResposta>>(
            $"/api/proprietarios/{proprietario.Id}/procedimentos");

        Assert.That(listagemProcedimentos, Has.Count.EqualTo(1));
        Assert.That(listagemProcedimentos![0].Id, Is.EqualTo(procedimento.Id));

        var slotsDisponiveis = await Client.GetFromJsonAsync<List<SlotHorarioResposta>>(
            $"/api/proprietarios/{proprietario.Id}/slots/disponiveis");

        Assert.That(slotsDisponiveis, Has.Count.EqualTo(1));
        Assert.That(slotsDisponiveis![0].Id, Is.EqualTo(slot.Id));

        var iniciarResponse = await Client.PostAsJsonAsync(
            "/api/agendamentos/iniciar",
            new IniciarAgendamentoRequisicao(
                proprietario.Id,
                cliente.Id,
                procedimento.Id,
                slot.Id));

        Assert.That(iniciarResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var agendamento = await iniciarResponse.Content.ReadFromJsonAsync<AgendamentoResposta>();
        Assert.That(agendamento!.Estado, Is.EqualTo("PENDENTE"));

        var confirmarResponse = await Client.PostAsJsonAsync(
            $"/api/agendamentos/{agendamento.Id}/confirmar",
            new ConfirmarAgendamentoRequisicao(cliente.Id));

        confirmarResponse.EnsureSuccessStatusCode();
        var confirmado = await confirmarResponse.Content.ReadFromJsonAsync<AgendamentoResposta>();
        Assert.That(confirmado!.Estado, Is.EqualTo("CONFIRMADO"));
    }

    [Test]
    public async Task ProcedimentoInativado_NaoDeveAparecerNaListagem()
    {
        var (proprietario, _) = await CriarProprietarioEClienteAsync();
        var procedimento = await CriarProcedimentoAsync(proprietario.Id);

        var inativarResponse = await Client.PostAsync(
            $"/api/proprietarios/{proprietario.Id}/procedimentos/{procedimento.Id}/inativar",
            null);

        inativarResponse.EnsureSuccessStatusCode();

        var listagem = await Client.GetFromJsonAsync<List<ProcedimentoResposta>>(
            $"/api/proprietarios/{proprietario.Id}/procedimentos");

        Assert.That(listagem, Is.Empty);
    }

    [Test]
    public async Task AgendamentoComProcedimentoInativo_DeveRetornar400()
    {
        var (proprietario, cliente) = await CriarProprietarioEClienteAsync();
        var procedimento = await CriarProcedimentoAsync(proprietario.Id);
        var slot = await CriarSlotAsync(proprietario.Id, DateTime.UtcNow.AddDays(2));

        await Client.PostAsync(
            $"/api/proprietarios/{proprietario.Id}/procedimentos/{procedimento.Id}/inativar",
            null);

        var response = await Client.PostAsJsonAsync(
            "/api/agendamentos/iniciar",
            new IniciarAgendamentoRequisicao(
                proprietario.Id,
                cliente.Id,
                procedimento.Id,
                slot.Id));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task SlotReservado_NaoDeveAparecerEmSlotsDisponiveis()
    {
        var (proprietario, cliente) = await CriarProprietarioEClienteAsync();
        var procedimento = await CriarProcedimentoAsync(proprietario.Id);
        var slot = await CriarSlotAsync(proprietario.Id, DateTime.UtcNow.AddDays(3));

        await Client.PostAsJsonAsync(
            "/api/agendamentos/iniciar",
            new IniciarAgendamentoRequisicao(
                proprietario.Id,
                cliente.Id,
                procedimento.Id,
                slot.Id));

        var slotsDisponiveis = await Client.GetFromJsonAsync<List<SlotHorarioResposta>>(
            $"/api/proprietarios/{proprietario.Id}/slots/disponiveis");

        Assert.That(slotsDisponiveis, Is.Empty);
    }
}
