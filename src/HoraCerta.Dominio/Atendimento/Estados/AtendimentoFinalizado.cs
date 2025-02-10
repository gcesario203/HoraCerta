using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Atendimento;

public class AtendimentoFinalizado : EstadoAtendimentoAbstracao
{
    public AtendimentoFinalizado()
    {
        Estado = EstadoAtendimento.REALIZADO;
    }
    public override IEstadoAtendimento AlterarEstado(AtendimentoEntidade entidade, EstadoAtendimento estado)
    {
        throw new OperacaoInvalidaExcessao("Atendimento já finalizado");
    }
}
