using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Dominio._Shared.Interfaces;

namespace HoraCerta.Aplicacao._Shared.Persistencia;

public static class UnidadeTrabalhoDominio
{
    public static void SalvarEDispararEventos<TAgregado>(
        Action<TAgregado> salvar,
        TAgregado agregado,
        IDomainEventDispatcher dispatcher)
        where TAgregado : IAggregateRoot
    {
        salvar(agregado);
        dispatcher.Disparar(agregado.EventosDominio);
        agregado.LimparEventosDominio();
    }

    public static void SalvarEDispararEventos(
        Action salvar,
        IEnumerable<IAggregateRoot> agregados,
        IDomainEventDispatcher dispatcher)
    {
        salvar();

        foreach (var agregado in agregados)
        {
            dispatcher.Disparar(agregado.EventosDominio);
            agregado.LimparEventosDominio();
        }
    }
}
