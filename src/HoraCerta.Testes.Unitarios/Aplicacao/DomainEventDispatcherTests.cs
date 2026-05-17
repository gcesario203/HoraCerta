using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Dominio._Shared.Interfaces;
using HoraCerta.Dominio.Cliente.Eventos;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace HoraCerta.Testes.Unitarios.Aplicacao;

[TestFixture]
public class DomainEventDispatcherTests
{
    private record EventoTeste(string Id, DateTime OcorreuEm) : IDomainEvent;

    private class HandlerTeste : IDomainEventHandler<EventoTeste>
    {
        public List<EventoTeste> Eventos { get; } = [];

        public void Handle(EventoTeste evento) => Eventos.Add(evento);
    }

    [Test]
    public void Disparar_DeveInvocarHandlerRegistrado()
    {
        var services = new ServiceCollection();
        var handler = new HandlerTeste();
        services.AddSingleton<IDomainEventHandler<EventoTeste>>(handler);
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();

        dispatcher.Disparar([
            new EventoTeste("1", DateTime.UtcNow),
            new EventoTeste("2", DateTime.UtcNow)]);

        Assert.That(handler.Eventos, Has.Count.EqualTo(2));
    }
}
