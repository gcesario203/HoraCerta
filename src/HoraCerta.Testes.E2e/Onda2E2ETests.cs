using System.Net;
using System.Net.Http.Json;
using HoraCerta.Api.Contratos;
using HoraCerta.Infaestrutura.Persistencia;
using HoraCerta.Testes.E2e.Infraestrutura;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace HoraCerta.Testes.E2e;

[TestFixture]
public class Onda2E2ETests : E2ETestBase
{
    [Test]
    public async Task LoginSemCredencial_DeveRetornar400()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequisicao("inexistente@test.com", "Senha123!"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task ConfirmarAgendamento_DeveCriarLembretePendente()
    {
        var (proprietario, cliente) = await CriarProprietarioEClienteAsync();
        var procedimento = await CriarProcedimentoAsync(proprietario.Id);
        var slot = await CriarSlotAsync(proprietario.Id, DateTime.UtcNow.AddDays(2));

        var iniciar = await Client.PostAsJsonAsync(
            "/api/agendamentos/iniciar",
            new IniciarAgendamentoRequisicao(
                proprietario.Id,
                cliente.Id,
                procedimento.Id,
                slot.Id));

        iniciar.EnsureSuccessStatusCode();
        var agendamento = await iniciar.Content.ReadFromJsonAsync<AgendamentoResposta>();

        var confirmar = await Client.PostAsJsonAsync(
            $"/api/agendamentos/{agendamento!.Id}/confirmar",
            new ConfirmarAgendamentoRequisicao(proprietario.Id, cliente.Id));

        confirmar.EnsureSuccessStatusCode();

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HoraCertaDbContext>();
        var lembrete = db.Lembretes.FirstOrDefault(l => l.AgendamentoId == agendamento.Id);

        Assert.That(lembrete, Is.Not.Null);
        Assert.That(lembrete!.Status, Is.EqualTo("Pendente"));
    }

    [Test]
    public async Task FluxoAvaliacao_DeveRegistrarAvaliacao()
    {
        var (proprietario, cliente) = await CriarProprietarioEClienteAsync();
        var procedimento = await CriarProcedimentoAsync(proprietario.Id);
        var slot = await CriarSlotAsync(proprietario.Id, DateTime.UtcNow.AddDays(3));

        var iniciar = await Client.PostAsJsonAsync(
            "/api/agendamentos/iniciar",
            new IniciarAgendamentoRequisicao(
                proprietario.Id,
                cliente.Id,
                procedimento.Id,
                slot.Id));

        var agendamento = await iniciar.Content.ReadFromJsonAsync<AgendamentoResposta>();

        await Client.PostAsJsonAsync(
            $"/api/agendamentos/{agendamento!.Id}/confirmar",
            new ConfirmarAgendamentoRequisicao(proprietario.Id, cliente.Id));

        var atendimentoResponse = await Client.PostAsJsonAsync(
            $"/api/agendamentos/{agendamento.Id}/atendimento",
            new RegistrarAtendimentoRequisicao(proprietario.Id, cliente.Id, null));

        atendimentoResponse.EnsureSuccessStatusCode();
        var atendimento = await atendimentoResponse.Content.ReadFromJsonAsync<AtendimentoResposta>();

        var patchResponse = await Client.SendAsync(new HttpRequestMessage(
            HttpMethod.Patch,
            $"/api/proprietarios/{proprietario.Id}/atendimentos/{atendimento!.Id}/estado")
        {
            Content = JsonContent.Create(new AlterarEstadoAtendimentoRequisicao("REALIZADO"))
        });

        Assert.That(
            patchResponse.StatusCode,
            Is.EqualTo(HttpStatusCode.OK),
            await patchResponse.Content.ReadAsStringAsync());

        Client.DefaultRequestHeaders.Authorization = null;

        var avaliar = await Client.PostAsJsonAsync(
            $"/api/clientes/{cliente.Id}/agendamentos/{agendamento.Id}/avaliar",
            new AvaliarAgendamentoRequisicao(proprietario.Id, 5, "Ótimo atendimento"));

        Assert.That(
            avaliar.StatusCode,
            Is.EqualTo(HttpStatusCode.OK),
            await avaliar.Content.ReadAsStringAsync());
    }
}
