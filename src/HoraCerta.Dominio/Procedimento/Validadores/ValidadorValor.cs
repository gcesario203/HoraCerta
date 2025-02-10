namespace HoraCerta.Dominio.Procedimento;

public class ValidadorValor : IValidadorEspecificacao<ProcedimentoEntidade>
{
    public void Valido(ProcedimentoEntidade entidade)
    {
        if (entidade.Valor < 0)
            throw new EntidadeInvalidadeExcessao("Valor menor do que zero");
    }
}
