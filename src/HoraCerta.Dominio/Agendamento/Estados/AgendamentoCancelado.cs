using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio;

public class AgendamentoCancelado : EstadoAgendamentoAbstrato
{
    public AgendamentoCancelado() : base()
    {
        Estado = EstadoAgendamento.CANCELADO;
    }

    public override IEstadoAgendamento AlterarEstado(Agendamento agendamento, EstadoAgendamento novoStatus)
    {
        if(novoStatus == EstadoAgendamento.REMARCADO)
        {
            agendamento.LiberarSlot();

            return new AgendamentoRemarcado();
        }

        throw new OperacaoInvalidaExcessao("Não é possivel realizar a nova operação em um agendamento cancelado");
    }
}
