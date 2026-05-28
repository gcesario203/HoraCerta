namespace HoraCerta.Aplicacao.Comunicacao.Ports;

public interface IHorarioSilenciosoServico
{
    bool EstaEmHorarioSilencioso(DateTime utcNow);

    DateTime ProximoHorarioPermitido(DateTime utcNow);
}
