using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio;

public enum EstadoAgendamento
{
    PENDENTE = 1,
    CONFIRMADO,
    CANCELADO,

    // Importante salientar que quando um agendamento é remarcado, instantaneamente é criado um novo agendamento
    REMARCADO,
    FINALIZADO
}
