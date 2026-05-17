using HoraCerta.Dominio._Shared.Interfaces;

namespace HoraCerta.Aplicacao._Shared.Interfaces;

public interface IDomainEventDispatcher
{
    void Disparar(IEnumerable<IDomainEvent> eventos);
}
