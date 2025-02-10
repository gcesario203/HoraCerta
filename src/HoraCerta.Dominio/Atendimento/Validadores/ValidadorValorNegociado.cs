using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Atendimento;

public class ValidadorValorNegociado : IValidadorEspecificacao<AtendimentoEntidade>
{
    public void Valido(AtendimentoEntidade entidade)
    {
        if (entidade.ValorNegociado < 0)
            throw new EntidadeInvalidadeExcessao("Atendimento com valor inválido");
    }
}
