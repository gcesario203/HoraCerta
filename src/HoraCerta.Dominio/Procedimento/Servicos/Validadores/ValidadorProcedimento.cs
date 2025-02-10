namespace HoraCerta.Dominio.Procedimento;

public class ValidadorProcedimento : ServicoValidacaoBase<ProcedimentoEntidade>
{
    public ValidadorProcedimento() : base(new List<IValidadorEspecificacao<ProcedimentoEntidade>>()
    {
        new ValidadorNome(),
        new ValidadorValor(),
        new ValidadorTempoEstimado()
    })
    {
        
    }
}
