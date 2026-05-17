using System.Text;
using HoraCerta.Api.Autenticacao;
using HoraCerta.Api.Endpoints;
using HoraCerta.Api.Excecoes;
using HoraCerta.Api.Extensions;
using HoraCerta.Aplicacao.Autenticacao;
using HoraCerta.Infaestrutura.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("HoraCerta")
    ?? "Data Source=horacerta.db";

var incluirBackground = !builder.Environment.IsEnvironment("Testing");

builder.Services.AddHoraCerta(builder.Configuration, connectionString, incluirBackground);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtOptions = builder.Configuration.GetSection(JwtOptions.Secao).Get<JwtOptions>() ?? new JwtOptions();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
        };
    });

builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapAuth();
app.MapCadastro();
app.MapProcedimentos();
app.MapSlots();
app.MapAgendamentos();
app.MapConsultas();

app.Run();

public partial class Program;
