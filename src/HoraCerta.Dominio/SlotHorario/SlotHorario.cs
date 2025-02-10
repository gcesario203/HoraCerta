namespace HoraCerta.Dominio;

public class SlotHorario : EntidadeBase<SlotHorario>
{
    public DateTime DataHora { get; private set; }

    public StatusSlotAgendamento Status { get; private set; }

    public SlotHorario(DateTime dataHora)
    {
        DataHora = dataHora;
        Status = StatusSlotAgendamento.DISPONIVEL;
    }

    public bool VerificarDisponibilidade()
        => Status == StatusSlotAgendamento.DISPONIVEL;

    public void AlterarStatus(StatusSlotAgendamento status)
    {
        Status = status;

        Atualizar();
    }
}
