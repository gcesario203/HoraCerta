using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio;

public class AgendamentoFinalizado : EstadoAgendamentoAbstrato
{
    public AgendamentoFinalizado() : base()
    {
        Estado = EstadoAgendamento.FINALIZADO;
    }

    public override IEstadoAgendamento AlterarEstado(Agendamento agendamento, EstadoAgendamento novoStatus)
    {
        throw new OperacaoInvalidaExcessao("Agendamento finalizado!");
    }
}
