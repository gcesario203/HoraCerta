namespace HoraCerta.Dominio.Procedimento;

public class ValidadorTempoEstimado : IValidadorEspecificacao<ProcedimentoEntidade>
{
    public void Valido(ProcedimentoEntidade entidade)
    {
        if (entidade.TempoEstimado.TotalHours > 24)
            throw new EntidadeInvalidadeExcessao("Tempo estimado não pode ser maior que um dia");

        if (entidade.TempoEstimado.TotalHours <= 0)
            throw new EntidadeInvalidadeExcessao("Tempo estimado não pode ser menor ou igual a zero");
    }
}
