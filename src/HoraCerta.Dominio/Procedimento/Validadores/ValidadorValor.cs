namespace HoraCerta.Dominio;

public class ValidadorValor : IValidadorEspecificacao<Procedimento>
{
    public void Valido(Procedimento entidade)
    {
        if (entidade.Valor < 0)
            throw new EntidadeInvalidadeExcessao("Valor menor do que zero");
    }
}
