using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;
using HoraCerta.Infaestrutura.Repositorio;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HoraCerta.Testes.E2e.Infraestrutura;

public class HoraCertaApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IProprietarioRepositorio>();
            services.RemoveAll<IClienteRepositorio>();
            services.RemoveAll<IDomainEventDispatcher>();

            services.AddSingleton<IProprietarioRepositorio, InMemoryProprietarioRepositorio>();
            services.AddSingleton<IClienteRepositorio, InMemoryClienteRepositorio>();
            services.AddSingleton<IDomainEventDispatcher, NopDomainEventDispatcher>();
        });
    }
}
