using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Atendimento;

public class AtendimentoPendente : EstadoAtendimentoAbstracao
{
    public AtendimentoPendente()
    {
        Estado = EstadoAtendimento.PENDENTE;
    }

    public override IEstadoAtendimento AlterarEstado(AtendimentoEntidade entidade, EstadoAtendimento estado)
    {
        if (estado == EstadoAtendimento.REALIZADO)
            return new AtendimentoFinalizado();

        if (estado == EstadoAtendimento.CANCELADO)
        {
            return new AtendimentoCancelado();
        }

        throw new OperacaoInvalidaExcessao("Estado de alteração inválido");
    }
}
