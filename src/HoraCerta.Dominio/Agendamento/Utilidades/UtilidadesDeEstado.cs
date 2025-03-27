
namespace HoraCerta.Dominio.Agendamento;

public static class UtilidadesDeEstado
{
    public static IEstadoAgendamento MontaObjetoDeEstado(EstadoAgendamento estado)
    {
        switch (estado)
        {
            case EstadoAgendamento.PENDENTE:
                return new AgendamentoPendente();

            case EstadoAgendamento.CONFIRMADO:
                return new AgendamentoConfirmado();

            case EstadoAgendamento.CANCELADO:
                return new AgendamentoCancelado();

            case EstadoAgendamento.REMARCADO:
                return new AgendamentoRemarcado();

            case EstadoAgendamento.FINALIZADO:
                return new AgendamentoFinalizado();

            default:
                throw new OperacaoInvalidaExcessao("Estado de objeto inválido");
        }
    }
}