using HoraCerta.Dominio;
using System.Text.Json;

namespace HoraCerta.Api.Excecoes;

public class TratamentoExcecoesDominio : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (OperacaoInvalidaExcessao ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { erro = ex.Message }));
        }
        catch (ExceptionBase ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { erro = ex.Message }));
        }
    }
}
