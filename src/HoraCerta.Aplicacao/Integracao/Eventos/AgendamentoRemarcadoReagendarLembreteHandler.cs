using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Aplicacao.Integracao.Lembretes;
using HoraCerta.Dominio.Cliente.Eventos;
using Microsoft.Extensions.Options;

namespace HoraCerta.Aplicacao.Integracao.Eventos;

public class AgendamentoRemarcadoReagendarLembreteHandler : IDomainEventHandler<AgendamentoRemarcadoEvent>
{
    private readonly ILembreteRepositorio _lembreteRepositorio;
    private readonly LembreteOptions _options;

    public AgendamentoRemarcadoReagendarLembreteHandler(
        ILembreteRepositorio lembreteRepositorio,
        IOptions<LembreteOptions> options)
    {
        _lembreteRepositorio = lembreteRepositorio;
        _options = options.Value;
    }

    public void Handle(AgendamentoRemarcadoEvent evento)
    {
        var enviarEm = evento.NovoSlotInicio.AddHours(-_options.HorasAntecedencia);

        if (enviarEm < DateTime.UtcNow)
            enviarEm = DateTime.UtcNow;

        _lembreteRepositorio.Reagendar(
            evento.AgendamentoAnteriorId,
            evento.NovoAgendamentoId,
            evento.NovoSlotInicio,
            enviarEm);
    }
}
