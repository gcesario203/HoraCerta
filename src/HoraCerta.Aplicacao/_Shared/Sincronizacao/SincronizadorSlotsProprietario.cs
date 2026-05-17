using HoraCerta.Dominio;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Proprietario;

namespace HoraCerta.Aplicacao._Shared.Sincronizacao;

public static class SincronizadorSlotsProprietario
{
    public static void AplicarStatusDoAgendamento(ProprietarioEntidade proprietario, AgendamentoEntidade agendamento)
    {
        if (agendamento.SlotHorario is null)
            return;

        var slot = proprietario.Horarios
            .FirstOrDefault(s => s.Id.Valor == agendamento.SlotHorario.Id.Valor);

        slot?.AlterarStatus(agendamento.SlotHorario.Status);
    }

    public static void LiberarSlot(ProprietarioEntidade proprietario, IdEntidade slotId)
    {
        var slot = proprietario.Horarios
            .FirstOrDefault(s => s.Id.Valor == slotId.Valor);

        if (slot is null)
            return;

        slot.AlterarStatus(StatusSlotAgendamento.DISPONIVEL);
    }
}
