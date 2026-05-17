using System.Net;
using System.Net.Http.Json;
using HoraCerta.Api.Contratos;
using HoraCerta.Testes.E2e.Infraestrutura;

namespace HoraCerta.Testes.E2e;

[TestFixture]
public class AgendamentoE2ETests : E2ETestBase
{
    [Test]
    public async Task CancelarAgendamentoConfirmado_DeveLiberarSlot()
    {
        var (proprietario, cliente) = await CriarProprietarioEClienteAsync();
        var procedimento = await CriarProcedimentoAsync(proprietario.Id);
        var slot = await CriarSlotAsync(proprietario.Id, DateTime.UtcNow.AddDays(4));

        var agendamento = await IniciarEConfirmarAsync(proprietario.Id, cliente.Id, procedimento.Id, slot.Id);

        var cancelarResponse = await Client.PostAsJsonAsync(
            $"/api/agendamentos/{agendamento.Id}/cancelar",
            new CancelarAgendamentoRequisicao(proprietario.Id, cliente.Id));

        cancelarResponse.EnsureSuccessStatusCode();
        var cancelado = await cancelarResponse.Content.ReadFromJsonAsync<AgendamentoResposta>();
        Assert.That(cancelado!.Estado, Is.EqualTo("CANCELADO"));

        var slotsDisponiveis = await Client.GetFromJsonAsync<List<SlotHorarioResposta>>(
            $"/api/proprietarios/{proprietario.Id}/slots/disponiveis");

        Assert.That(slotsDisponiveis, Has.Count.EqualTo(1));
        Assert.That(slotsDisponiveis![0].Id, Is.EqualTo(slot.Id));
        Assert.That(slotsDisponiveis[0].Status, Is.EqualTo("DISPONIVEL"));
    }

    [Test]
    public async Task RemarcarAgendamentoConfirmado_DeveCriarNovoAgendamentoPendente()
    {
        var (proprietario, cliente) = await CriarProprietarioEClienteAsync();
        var procedimento = await CriarProcedimentoAsync(proprietario.Id);
        var slot1 = await CriarSlotAsync(proprietario.Id, DateTime.UtcNow.AddDays(5));
        var slot2 = await CriarSlotAsync(proprietario.Id, DateTime.UtcNow.AddDays(5).AddHours(2));

        var agendamento = await IniciarEConfirmarAsync(proprietario.Id, cliente.Id, procedimento.Id, slot1.Id);

        var remarcarResponse = await Client.PostAsJsonAsync(
            $"/api/agendamentos/{agendamento.Id}/remarcar",
            new RemarcarAgendamentoRequisicao(proprietario.Id, cliente.Id, slot2.Id));

        Assert.That(remarcarResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var remarcado = await remarcarResponse.Content.ReadFromJsonAsync<AgendamentoResposta>();
        Assert.That(remarcado!.Estado, Is.EqualTo("PENDENTE"));
        Assert.That(remarcado.ReagendamentoId, Is.EqualTo(agendamento.Id));
        Assert.That(remarcado.Id, Is.Not.EqualTo(agendamento.Id));
    }

    [Test]
    public async Task RegistrarAtendimento_DeveFinalizarAgendamento()
    {
        var (proprietario, cliente) = await CriarProprietarioEClienteAsync();
        var procedimento = await CriarProcedimentoAsync(proprietario.Id);
        var slot = await CriarSlotAsync(proprietario.Id, DateTime.UtcNow.AddDays(6));

        var agendamento = await IniciarEConfirmarAsync(proprietario.Id, cliente.Id, procedimento.Id, slot.Id);

        var atendimentoResponse = await Client.PostAsJsonAsync(
            $"/api/agendamentos/{agendamento.Id}/atendimento",
            new RegistrarAtendimentoRequisicao(proprietario.Id, cliente.Id, 45m));

        Assert.That(atendimentoResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var atendimento = await atendimentoResponse.Content.ReadFromJsonAsync<AtendimentoResposta>();
        Assert.That(atendimento!.Estado, Is.EqualTo("PENDENTE"));
        Assert.That(atendimento.AgendamentoId, Is.EqualTo(agendamento.Id));
        Assert.That(atendimento.ValorNegociado, Is.EqualTo(45m));
    }

    [Test]
    public async Task ProprietarioInexistente_DeveRetornar400()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/proprietarios/inexistente/procedimentos",
            new CriarProcedimentoRequisicao("Corte", 50m, 30));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    private async Task<AgendamentoResposta> IniciarEConfirmarAsync(
        string proprietarioId,
        string clienteId,
        string procedimentoId,
        string slotId)
    {
        var iniciarResponse = await Client.PostAsJsonAsync(
            "/api/agendamentos/iniciar",
            new IniciarAgendamentoRequisicao(proprietarioId, clienteId, procedimentoId, slotId));

        iniciarResponse.EnsureSuccessStatusCode();
        var agendamento = await iniciarResponse.Content.ReadFromJsonAsync<AgendamentoResposta>()
            ?? throw new InvalidOperationException("Agendamento não retornado");

        var confirmarResponse = await Client.PostAsJsonAsync(
            $"/api/agendamentos/{agendamento.Id}/confirmar",
            new ConfirmarAgendamentoRequisicao(proprietarioId, clienteId));

        confirmarResponse.EnsureSuccessStatusCode();
        return await confirmarResponse.Content.ReadFromJsonAsync<AgendamentoResposta>()
            ?? throw new InvalidOperationException("Confirmação não retornada");
    }
}
