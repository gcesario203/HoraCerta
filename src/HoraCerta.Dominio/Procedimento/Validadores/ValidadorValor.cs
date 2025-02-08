using HoraCerta.Dominio._Shared.Excessoes;
using HoraCerta.Dominio._Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Procedimento;

public class ValidadorValor : IValidadorEspecificacao<Procedimento>
{
    public void Valido(Procedimento entidade)
    {
        if (entidade.Valor < 0)
            throw new EntidadeInvalidadeExcessao("Valor menor do que zero");
    }
}
