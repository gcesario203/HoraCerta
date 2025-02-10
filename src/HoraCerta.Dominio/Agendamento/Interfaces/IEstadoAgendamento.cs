using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio;

public interface IEstadoAgendamento
{
    EstadoAgendamento EstadoAtual();

    IEstadoAgendamento AlterarEstado(Agendamento agendamento, EstadoAgendamento novoStatus);
}
