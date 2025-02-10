namespace HoraCerta.Dominio;

public class SlotHorarioEntidade : EntidadeBase<SlotHorarioEntidade>
{
    public DateTime DataHora { get; private set; }

    public StatusSlotAgendamento Status { get; private set; }

    public SlotHorarioEntidade(DateTime dataHora)
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
