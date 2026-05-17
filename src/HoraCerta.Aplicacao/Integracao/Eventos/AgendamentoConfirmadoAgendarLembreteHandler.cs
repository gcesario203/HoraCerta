using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Aplicacao.Integracao.Lembretes;
using HoraCerta.Dominio.Cliente.Eventos;
using Microsoft.Extensions.Options;

namespace HoraCerta.Aplicacao.Integracao.Eventos;

public class AgendamentoConfirmadoAgendarLembreteHandler : IDomainEventHandler<AgendamentoConfirmadoEvent>
{
    private readonly ILembreteRepositorio _lembreteRepositorio;
    private readonly LembreteOptions _options;

    public AgendamentoConfirmadoAgendarLembreteHandler(
        ILembreteRepositorio lembreteRepositorio,
        IOptions<LembreteOptions> options)
    {
        _lembreteRepositorio = lembreteRepositorio;
        _options = options.Value;
    }

    public void Handle(AgendamentoConfirmadoEvent evento)
    {
        var enviarEm = evento.SlotInicio.AddHours(-_options.HorasAntecedencia);

        if (enviarEm < DateTime.UtcNow)
            enviarEm = DateTime.UtcNow;

        _lembreteRepositorio.Agendar(
            evento.ProprietarioId,
            evento.ClienteId,
            evento.AgendamentoId,
            evento.TelefoneCliente,
            evento.SlotInicio,
            enviarEm);
    }
}
