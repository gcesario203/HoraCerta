using HoraCerta.Api.Endpoints;
using HoraCerta.Api.Excecoes;
using HoraCerta.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHoraCerta();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<TratamentoExcecoesDominio>();

var app = builder.Build();

app.UseMiddleware<TratamentoExcecoesDominio>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapCadastro();
app.MapProcedimentos();
app.MapSlots();
app.MapAgendamentos();

app.Run();

public partial class Program;
