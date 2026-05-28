using HoraCerta.Aplicacao.Comunicacao;
using HoraCerta.Aplicacao.Comunicacao.Ports;
using Microsoft.Extensions.Options;

namespace HoraCerta.Infaestrutura.Comunicacao;

public class HorarioSilenciosoServico : IHorarioSilenciosoServico
{
    private readonly WhatsAppOptions _options;

    public HorarioSilenciosoServico(IOptions<WhatsAppOptions> options)
    {
        _options = options.Value;
    }

    public bool EstaEmHorarioSilencioso(DateTime utcNow)
    {
        var fuso = ObterFuso();
        var local = TimeZoneInfo.ConvertTimeFromUtc(utcNow, fuso);
        var inicio = TimeOnly.Parse(_options.HorarioSilenciosoInicio);
        var fim = TimeOnly.Parse(_options.HorarioSilenciosoFim);
        var hora = TimeOnly.FromDateTime(local);

        if (inicio < fim)
            return hora >= inicio && hora < fim;

        return hora >= inicio || hora < fim;
    }

    public DateTime ProximoHorarioPermitido(DateTime utcNow)
    {
        if (!EstaEmHorarioSilencioso(utcNow))
            return utcNow;

        var fuso = ObterFuso();
        var local = TimeZoneInfo.ConvertTimeFromUtc(utcNow, fuso);
        var fim = TimeOnly.Parse(_options.HorarioSilenciosoFim);
        var proximoLocal = local.Date.Add(fim.ToTimeSpan());

        if (proximoLocal <= local)
            proximoLocal = proximoLocal.AddDays(1);

        return TimeZoneInfo.ConvertTimeToUtc(proximoLocal, fuso);
    }

    private TimeZoneInfo ObterFuso()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(_options.FusoHorario);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        }
    }
}
