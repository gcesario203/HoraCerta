namespace HoraCerta.Dominio._Shared.Interfaces;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> EventosDominio { get; }

    void LimparEventosDominio();
}
