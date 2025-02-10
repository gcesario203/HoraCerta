using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Agendamento;

public interface IEstadoAgendamento
{
    EstadoAgendamento EstadoAtual();

    IEstadoAgendamento AlterarEstado(AgendamentoEntidade agendamento, EstadoAgendamento novoStatus);
}
