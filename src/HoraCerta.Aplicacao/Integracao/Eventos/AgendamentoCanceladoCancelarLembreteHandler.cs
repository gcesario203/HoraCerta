using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Aplicacao.Integracao.Lembretes;
using HoraCerta.Dominio.Cliente.Eventos;

namespace HoraCerta.Aplicacao.Integracao.Eventos;

public class AgendamentoCanceladoCancelarLembreteHandler : IDomainEventHandler<AgendamentoCanceladoEvent>
{
    private readonly ILembreteRepositorio _lembreteRepositorio;

    public AgendamentoCanceladoCancelarLembreteHandler(ILembreteRepositorio lembreteRepositorio)
    {
        _lembreteRepositorio = lembreteRepositorio;
    }

    public void Handle(AgendamentoCanceladoEvent evento)
        => _lembreteRepositorio.CancelarPorAgendamento(evento.AgendamentoId);
}
