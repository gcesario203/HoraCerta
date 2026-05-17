using System.Net.Http.Json;
using HoraCerta.Api.Contratos;
using HoraCerta.Infaestrutura.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace HoraCerta.Testes.E2e.Infraestrutura;

public abstract class E2ETestBase
{
    private HoraCertaApiFactory _factory = null!;
    protected HttpClient Client = null!;

    [SetUp]
    public void ConfigurarClienteHttp()
    {
        _factory?.Dispose();
        _factory = new HoraCertaApiFactory();
        Client = _factory.CreateClient();

        _factory.Services.AplicarMigrationsHoraCerta();
    }

    [TearDown]
    public void EncerrarClienteHttp()
    {
        Client?.Dispose();
        _factory?.Dispose();
    }

    protected async Task<(ProprietarioResposta Proprietario, ClienteResposta Cliente)> CriarProprietarioEClienteAsync()
    {
        var proprietarioResponse = await Client.PostAsJsonAsync(
            "/api/proprietarios",
            new CriarProprietarioRequisicao("Barbearia E2E"));

        proprietarioResponse.EnsureSuccessStatusCode();
        var proprietario = await proprietarioResponse.Content.ReadFromJsonAsync<ProprietarioResposta>()
            ?? throw new InvalidOperationException("Resposta de proprietário inválida");

        var clienteResponse = await Client.PostAsJsonAsync(
            "/api/clientes",
            new CriarClienteRequisicao("Maria", "(11) 99999-9999"));

        clienteResponse.EnsureSuccessStatusCode();
        var cliente = await clienteResponse.Content.ReadFromJsonAsync<ClienteResposta>()
            ?? throw new InvalidOperationException("Resposta de cliente inválida");

        return (proprietario, cliente);
    }

    protected async Task<ProcedimentoResposta> CriarProcedimentoAsync(
        string proprietarioId,
        string nome = "Corte",
        decimal valor = 50m,
        int tempoMinutos = 30)
    {
        var response = await Client.PostAsJsonAsync(
            $"/api/proprietarios/{proprietarioId}/procedimentos",
            new CriarProcedimentoRequisicao(nome, valor, tempoMinutos));

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProcedimentoResposta>()
            ?? throw new InvalidOperationException("Resposta de procedimento inválida");
    }

    protected async Task<SlotHorarioResposta> CriarSlotAsync(string proprietarioId, DateTime inicio)
    {
        var response = await Client.PostAsJsonAsync(
            $"/api/proprietarios/{proprietarioId}/slots",
            new CriarSlotRequisicao(inicio));

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SlotHorarioResposta>()
            ?? throw new InvalidOperationException("Resposta de slot inválida");
    }
}
