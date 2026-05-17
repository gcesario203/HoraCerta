using HoraCerta.Aplicacao.Autenticacao.Commands;
using HoraCerta.Aplicacao.Autenticacao.Handlers;
using HoraCerta.Api.Autenticacao;
using HoraCerta.Api.Contratos;

namespace HoraCerta.Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuth(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Autenticação");

        group.MapPost("/registrar", (RegistrarCredencialRequisicao req, RegistrarCredencialHandler handler) =>
        {
            var proprietarioId = handler.Executar(new RegistrarCredencialCommand(
                req.ProprietarioId,
                req.NomeEstabelecimento,
                req.Email,
                req.Senha));

            return Results.Created($"/api/proprietarios/{proprietarioId}", new { proprietarioId });
        });

        group.MapPost("/login", (LoginRequisicao req, LoginHandler handler) =>
        {
            var (token, proprietarioId) = handler.Executar(new LoginCommand(req.Email, req.Senha));
            return Results.Ok(new LoginResposta(token, proprietarioId));
        });

        return group;
    }
}
