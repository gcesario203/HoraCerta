namespace HoraCerta.Dominio;

public class SlotHorarioEntidade : EntidadeBase<SlotHorarioEntidade>
{
    public DateTime Inicio { get; private set; }

    public DateTime? Fim { get; private set; }

    public StatusSlotAgendamento Status { get; private set; }

    public SlotHorarioEntidade(DateTime dataHora, TimeSpan? duracao = null)
    {
        Inicio = dataHora;

        Status = StatusSlotAgendamento.DISPONIVEL;

        if (duracao != null)
            AlterarDuracao(duracao.Value);
    }

    public bool VerificarDisponibilidade()
        => Status == StatusSlotAgendamento.DISPONIVEL;

    public void AlterarStatus(StatusSlotAgendamento status)
    {
        Status = status;

        Atualizar();
    }

    public bool ConflitaCom(SlotHorarioEntidade outro)
    {
        var fimAtual = Fim ?? Inicio;
        var fimOutro = outro.Fim ?? outro.Inicio;

        return Inicio <= fimOutro && outro.Inicio <= fimAtual;
    }

    public void AlterarDuracao(TimeSpan duracao)
    {
        if (!VerificarDisponibilidade())
            throw new OperacaoInvalidaExcessao("Não é possivel alterar o tempo de duração de um slot de tempo nao disponivel");

        Fim = Inicio.Add(duracao);
    }
}
