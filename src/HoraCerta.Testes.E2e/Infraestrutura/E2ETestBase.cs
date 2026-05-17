using System.Net.Http.Headers;
using System.Net.Http.Json;
using HoraCerta.Api.Contratos;
using HoraCerta.Infaestrutura.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace HoraCerta.Testes.E2e.Infraestrutura;

public abstract class E2ETestBase
{
    protected HoraCertaApiFactory Factory = null!;
    protected HttpClient Client = null!;

    [SetUp]
    public void ConfigurarClienteHttp()
    {
        Factory?.Dispose();
        Factory = new HoraCertaApiFactory();
        Client = Factory.CreateClient();

        Factory.Services.AplicarMigrationsHoraCerta();
    }

    [TearDown]
    public void EncerrarClienteHttp()
    {
        Client?.Dispose();
        Factory?.Dispose();
    }

    protected async Task<(ProprietarioResposta Proprietario, ClienteResposta Cliente, string Token)> CriarProprietarioClienteEAutenticarAsync(
        string nomeEstabelecimento = "Barbearia E2E",
        string email = "barbearia@e2e.test",
        string senha = "Senha123!")
    {
        var proprietarioResponse = await Client.PostAsJsonAsync(
            "/api/auth/registrar",
            new RegistrarCredencialRequisicao(null, nomeEstabelecimento, email, senha));

        proprietarioResponse.EnsureSuccessStatusCode();

        var loginResponse = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequisicao(email, senha));

        loginResponse.EnsureSuccessStatusCode();
        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResposta>()
            ?? throw new InvalidOperationException("Resposta de login inválida");

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.Token);

        var proprietario = await Client.GetFromJsonAsync<ProprietarioResposta>(
            $"/api/proprietarios/{login.ProprietarioId}")
            ?? throw new InvalidOperationException("Proprietário não encontrado");

        var clienteResponse = await Client.PostAsJsonAsync(
            "/api/clientes",
            new CriarClienteRequisicao("Maria", "(11) 99999-9999"));

        clienteResponse.EnsureSuccessStatusCode();
        var cliente = await clienteResponse.Content.ReadFromJsonAsync<ClienteResposta>()
            ?? throw new InvalidOperationException("Resposta de cliente inválida");

        return (proprietario, cliente, login.Token);
    }

    protected async Task<(ProprietarioResposta Proprietario, ClienteResposta Cliente)> CriarProprietarioEClienteAsync()
    {
        var (proprietario, cliente, _) = await CriarProprietarioClienteEAutenticarAsync();
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
