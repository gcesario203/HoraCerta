using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.SlotHorario;

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

    private SlotHorarioEntidade(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade, DateTime inicio, DateTime? fim, StatusSlotAgendamento statusSlotAgendamento)
    : base(id, dataCriacao, dataAlteracao, estadoEntidade)
    {
        Inicio = inicio;

        Fim = fim;

        Status = statusSlotAgendamento;
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

        Atualizar();
    }

    public static SlotHorarioDTO ParaDTO(SlotHorarioEntidade entidade)
    => new SlotHorarioDTO(entidade.Id.Valor, entidade.DataCriacao, entidade.DataAlteracao, entidade.EstadoEntidade, entidade.Inicio, entidade.Fim, entidade.Status);

    public static SlotHorarioEntidade ParaEntidade(SlotHorarioDTO slotHorario)
    => new SlotHorarioEntidade(slotHorario.Id, slotHorario.DataCriacao, slotHorario.DataAlteracao, slotHorario.EstadoEntidade, slotHorario.Inicio, slotHorario.Fim, slotHorario.Status);
}
