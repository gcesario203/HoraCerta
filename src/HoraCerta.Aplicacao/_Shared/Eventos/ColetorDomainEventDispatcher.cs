using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Dominio._Shared.Interfaces;

namespace HoraCerta.Aplicacao._Shared.Eventos;

public class ColetorDomainEventDispatcher : IDomainEventDispatcher
{
    public IList<IDomainEvent> EventosDisparados { get; } = new List<IDomainEvent>();

    public void Disparar(IEnumerable<IDomainEvent> eventos)
    {
        foreach (var evento in eventos)
            EventosDisparados.Add(evento);
    }

    public void Limpar()
        => EventosDisparados.Clear();
}
