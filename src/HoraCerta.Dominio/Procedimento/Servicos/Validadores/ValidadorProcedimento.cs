namespace HoraCerta.Dominio;

public class ValidadorProcedimento : ServicoValidacaoBase<Procedimento>
{
    public ValidadorProcedimento() : base(new List<IValidadorEspecificacao<Procedimento>>()
    {
        new ValidadorNome(),
        new ValidadorValor(),
        new ValidadorTempoEstimado()
    })
    {
        
    }
}
