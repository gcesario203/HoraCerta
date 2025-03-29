
namespace HoraCerta.Dominio.Atendimento;

public static class UtilidadesDeEstado
{
    public static IEstadoAtendimento MontaObjetoDeEstado(EstadoAtendimento estado)
    {
        switch (estado)
        {
            case EstadoAtendimento.PENDENTE:
                return new AtendimentoPendente();

            case EstadoAtendimento.REALIZADO:
                return new AtendimentoFinalizado();

            case EstadoAtendimento.CANCELADO:
                return new AtendimentoCancelado();

            default:
                throw new OperacaoInvalidaExcessao("Estado de objeto inválido");
        }
    }
}