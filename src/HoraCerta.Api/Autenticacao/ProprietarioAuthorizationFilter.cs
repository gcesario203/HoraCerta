using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HoraCerta.Api.Autenticacao;

public class ProprietarioAuthorizationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var http = context.HttpContext;

        if (http.User.Identity?.IsAuthenticated != true)
            return Results.Unauthorized();

        var claimId = http.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? http.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (context.HttpContext.Request.RouteValues.TryGetValue("proprietarioId", out var routeValue)
            && routeValue?.ToString() is { } proprietarioId
            && claimId != proprietarioId)
        {
            return Results.Forbid();
        }

        return await next(context);
    }
}

public class ProprietarioBodyAuthorizationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var http = context.HttpContext;

        if (http.User.Identity?.IsAuthenticated != true)
            return Results.Unauthorized();

        var claimId = http.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? http.User.FindFirstValue(ClaimTypes.NameIdentifier);

        foreach (var arg in context.Arguments)
        {
            var prop = arg?.GetType().GetProperty("ProprietarioId");
            if (prop?.GetValue(arg) is string proprietarioId && claimId != proprietarioId)
                return Results.Forbid();
        }

        return await next(context);
    }
}
