namespace HoraCerta.Dominio.Atendimento;

public class AtendimentoFalha : EstadoAtendimentoAbstracao
{
    public AtendimentoFalha()
    {
        Estado = EstadoAtendimento.FALHA;
    }

    public override IEstadoAtendimento AlterarEstado(AtendimentoEntidade entidade, EstadoAtendimento estado)
        => throw new OperacaoInvalidaExcessao("Atendimento com falha registrada");
}
