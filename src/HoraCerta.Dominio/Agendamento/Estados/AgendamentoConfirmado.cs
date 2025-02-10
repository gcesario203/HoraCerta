using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio;

public class AgendamentoConfirmado : EstadoAgendamentoAbstrato
{
    public AgendamentoConfirmado() :base()
    {
        Estado = EstadoAgendamento.CONFIRMADO;
    }
    public override IEstadoAgendamento AlterarEstado(Agendamento agendamento, EstadoAgendamento novoStatus)
    {
        if(novoStatus == EstadoAgendamento.CANCELADO)
        {
            agendamento.LiberarSlot();

            return new AgendamentoCancelado();
        }

        if(novoStatus == EstadoAgendamento.FINALIZADO)
        {
            agendamento.SlotHorario.AlterarStatus(StatusSlotAgendamento.CONFIRMADO);

            return new AgendamentoFinalizado();
        }

        if(novoStatus == EstadoAgendamento.REMARCADO)
        {
            agendamento.LiberarSlot();

            return new AgendamentoRemarcado();
        }

        throw new OperacaoInvalidaExcessao("Novo estado inválido para confirmação");
    }
}
