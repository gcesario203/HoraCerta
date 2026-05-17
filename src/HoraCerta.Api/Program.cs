using HoraCerta.Api.Endpoints;
using HoraCerta.Api.Excecoes;
using HoraCerta.Api.Extensions;
using HoraCerta.Infaestrutura.Extensions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("HoraCerta")
    ?? "Data Source=horacerta.db";

builder.Services.AddHoraCerta(connectionString);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<TratamentoExcecoesDominio>();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
    app.Services.AplicarMigrationsHoraCerta();

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
