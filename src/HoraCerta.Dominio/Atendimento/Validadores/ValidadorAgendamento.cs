using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Atendimento;

public class ValidadorAgendamento : IValidadorEspecificacao<AtendimentoEntidade>
{
    public void Valido(AtendimentoEntidade entidade)
    {
        if (entidade.Origem is null)
            throw new EntidadeInvalidadeExcessao("Atendimento precisa ser originado de um agendamento");

        if (entidade.Origem.EstadoAtual() != Agendamento.EstadoAgendamento.FINALIZADO)
            throw new EntidadeInvalidadeExcessao("Atendimento não deve ser criado a partir de Agendamentos não finalizados");

        if (entidade.Origem.Procedimento is null)
            throw new EntidadeInvalidadeExcessao("Atendimetno deve conter um procedimento");

        if (entidade.Origem.SlotHorario?.Status != StatusSlotAgendamento.CONFIRMADO)
            throw new EntidadeInvalidadeExcessao("Atendimento deve ter um slot de horario confirmado");
    }
}
