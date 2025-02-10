using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio;

public class AgendamentoRemarcado : EstadoAgendamentoAbstrato
{
    public AgendamentoRemarcado() : base()
    {
        Estado = EstadoAgendamento.REMARCADO;
    }

    public override IEstadoAgendamento AlterarEstado(Agendamento agendamento, EstadoAgendamento novoStatus)
    {
        throw new OperacaoInvalidaExcessao("Agendamento remarcado não pode mudar mais de status");
    }
}
