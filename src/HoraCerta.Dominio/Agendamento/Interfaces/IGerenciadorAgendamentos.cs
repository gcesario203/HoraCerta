using HoraCerta.Dominio.Procedimento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Agendamento;

public interface IGerenciadorAgendamentos
{
    AgendamentoEntidade IniciarAgendamento(ProcedimentoEntidade procedimento, SlotHorarioEntidade slot);
    void ConfirmarAgendamento(IdEntidade id);

    void CancelarAgendamento(IdEntidade id);

    AgendamentoEntidade RemarcarAgendamento(IdEntidade id, SlotHorarioEntidade slot);
}
