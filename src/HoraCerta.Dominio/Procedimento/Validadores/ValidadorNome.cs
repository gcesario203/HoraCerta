using HoraCerta.Dominio._Shared.Excessoes;
using HoraCerta.Dominio._Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Procedimento.Validadores
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
