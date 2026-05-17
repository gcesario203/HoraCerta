using HoraCerta.Dominio._Shared.Interfaces;

namespace HoraCerta.Aplicacao._Shared.Eventos;

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    void Handle(TEvent evento);
}
