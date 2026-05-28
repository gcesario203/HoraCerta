using HoraCerta.Api.Contratos;
using HoraCerta.Api.Mapeamento;
using HoraCerta.Aplicacao.Cliente.Commands;
using HoraCerta.Aplicacao.Cliente.Handlers;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;
using HoraCerta.Dominio.Proprietario;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Api.Endpoints;

public static class CadastroEndpoints
{
    public static RouteGroupBuilder MapCadastro(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api").WithTags("Cadastro");

        group.MapPost("/proprietarios", (CriarProprietarioRequisicao req, IProprietarioRepositorio repo) =>
        {
            var proprietario = new ProprietarioEntidade(req.Nome);
            repo.Salvar(proprietario);
            return Results.Created($"/api/proprietarios/{proprietario.Id.Valor}", RespostaMapeamento.ParaResposta(proprietario));
        });

        group.MapGet("/proprietarios/{id}", (string id, IProprietarioRepositorio repo) =>
        {
            var proprietario = repo.BuscarPorId(RespostaMapeamento.Id(id));
            return proprietario is null
                ? Results.NotFound()
                : Results.Ok(RespostaMapeamento.ParaResposta(proprietario));
        });

        group.MapPost("/clientes", (CriarClienteRequisicao req, CriarClienteHandler handler) =>
        {
            var cliente = handler.Executar(new CriarClienteCommand(req.Nome, req.Telefone));
            return Results.Created($"/api/clientes/{cliente.Id.Valor}", RespostaMapeamento.ParaResposta(cliente));
        });

        group.MapGet("/clientes/{id}", (string id, IClienteRepositorio repo) =>
        {
            var cliente = repo.BuscarPorId(RespostaMapeamento.Id(id));
            return cliente is null
                ? Results.NotFound()
                : Results.Ok(RespostaMapeamento.ParaResposta(cliente));
        });

        return group;
    }
}
