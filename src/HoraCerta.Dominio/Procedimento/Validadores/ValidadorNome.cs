namespace HoraCerta.Dominio.Procedimento
{
    public class ValidadorNome : IValidadorEspecificacao<ProcedimentoEntidade>
    {
        public void Valido(ProcedimentoEntidade entidade)
        {
            if (string.IsNullOrEmpty(entidade.Nome))
                throw new EntidadeInvalidadeExcessao("Nome vazio");

            if (entidade.Nome.Length <= 0 && entidade.Nome.Length > 70)
                throw new EntidadeInvalidadeExcessao("Nome com tamanho inválido");
        }
    }
}
