using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Agendamento;

public class AgendamentoFinalizado : EstadoAgendamentoAbstrato
{
    public AgendamentoFinalizado() : base()
    {
        Estado = EstadoAgendamento.FINALIZADO;
    }

    public override IEstadoAgendamento AlterarEstado(AgendamentoEntidade agendamento, EstadoAgendamento novoStatus)
    {
        throw new OperacaoInvalidaExcessao("Agendamento finalizado!");
    }
}
