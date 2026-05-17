using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Dominio._Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HoraCerta.Aplicacao._Shared.Eventos;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Disparar(IEnumerable<IDomainEvent> eventos)
    {
        foreach (var evento in eventos)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(evento.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.Handle))!;
                method.Invoke(handler, [evento]);
            }
        }
    }
}
