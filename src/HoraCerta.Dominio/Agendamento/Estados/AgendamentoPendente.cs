using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Agendamento;

public class AgendamentoPendente : EstadoAgendamentoAbstrato
{
    public AgendamentoPendente() : base()
    {
        Estado = EstadoAgendamento.PENDENTE;
    }

    public override IEstadoAgendamento AlterarEstado(AgendamentoEntidade agendamento, EstadoAgendamento novoStatus)
    {
        if (novoStatus == EstadoAgendamento.PENDENTE)
        {
            return this;
        }

        if (novoStatus == EstadoAgendamento.CONFIRMADO)
        {
            agendamento.SlotHorario.AlterarStatus(StatusSlotAgendamento.RESERVADO);

            return new AgendamentoConfirmado();
        }

        if( novoStatus == EstadoAgendamento.CANCELADO)
        {
            agendamento.LiberarSlot();

            return new AgendamentoCancelado();
        }

        throw new OperacaoInvalidaExcessao("Operação inválida para estado atual de agendamento");
    }
}
