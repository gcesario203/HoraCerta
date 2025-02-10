namespace HoraCerta.Dominio
{
    public class ValidadorNome : IValidadorEspecificacao<Procedimento>
    {
        public void Valido(Procedimento entidade)
        {
            if (string.IsNullOrEmpty(entidade.Nome))
                throw new EntidadeInvalidadeExcessao("Nome vazio");

            if (entidade.Nome.Length <= 0 && entidade.Nome.Length > 70)
                throw new EntidadeInvalidadeExcessao("Nome com tamanho inválido");
        }
    }
}
