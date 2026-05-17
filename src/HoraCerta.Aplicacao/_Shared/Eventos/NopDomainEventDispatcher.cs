using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Dominio._Shared.Interfaces;

namespace HoraCerta.Aplicacao._Shared.Eventos;

public class NopDomainEventDispatcher : IDomainEventDispatcher
{
    public void Disparar(IEnumerable<IDomainEvent> eventos) { }
}
