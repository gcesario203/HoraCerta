using HoraCerta.Dominio._Shared.Excessoes;
using HoraCerta.Dominio._Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Procedimento;

public class ValidadorTempoEstimado : IValidadorEspecificacao<Procedimento>
{
    public void Valido(Procedimento entidade)
    {
        if (entidade.TempoEstimado.TotalHours > 24)
            throw new EntidadeInvalidadeExcessao("Tempo estimado não pode ser maior que um dia");

        if (entidade.TempoEstimado.TotalHours <= 0)
            throw new EntidadeInvalidadeExcessao("Tempo estimado não pode ser menor ou igual a zero");
    }
}
