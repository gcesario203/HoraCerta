using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Agendamento;

public abstract class EstadoAgendamentoAbstrato : IEstadoAgendamento
{
    public EstadoAgendamento Estado { get; protected set; }

    public abstract IEstadoAgendamento AlterarEstado(AgendamentoEntidade agendamento, EstadoAgendamento novoStatus);

    public EstadoAgendamento EstadoAtual()
        => Estado;
}
