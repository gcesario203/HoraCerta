namespace HoraCerta.Dominio.Proprietario
{
    public class ValidadorNome : IValidadorEspecificacao<ProprietarioEntidade>
    {
        public void Valido(ProprietarioEntidade entidade)
        {
            if (string.IsNullOrEmpty(entidade.Nome))
                throw new EntidadeInvalidadeExcessao("Nome vazio");

            if (entidade.Nome.Length <= 0 || entidade.Nome.Length > 70)
                throw new EntidadeInvalidadeExcessao("Nome com tamanho inválido");
        }
    }
}
