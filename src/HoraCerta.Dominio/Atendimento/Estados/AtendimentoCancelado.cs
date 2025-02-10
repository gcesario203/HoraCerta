using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Atendimento;

public class AtendimentoCancelado : EstadoAtendimentoAbstracao
{
    public AtendimentoCancelado()
    {
        Estado = EstadoAtendimento.CANCELADO;
    }
    public override IEstadoAtendimento AlterarEstado(AtendimentoEntidade entidade, EstadoAtendimento estado)
    {
        throw new OperacaoInvalidaExcessao("Atendimento cancelado");
    }
}
