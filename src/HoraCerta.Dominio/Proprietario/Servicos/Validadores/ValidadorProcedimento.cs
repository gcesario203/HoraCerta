namespace HoraCerta.Dominio.Proprietario;

public class ValidadorProprietario : ServicoValidacaoBase<ProprietarioEntidade>
{
    public ValidadorProprietario() : base(new List<IValidadorEspecificacao<ProprietarioEntidade>>()
    {
        new ValidadorNome()
    })
    {
        
    }
}
