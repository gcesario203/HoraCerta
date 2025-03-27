
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Procedimento;

namespace HoraCerta.Dominio.Agendamento;

public class AgendamentoEntidade : EntidadeBase<AgendamentoEntidade>
{
    /// <summary>
    /// Slot da agenda cujo qual este agendamento ocupa
    /// </summary>
    public SlotHorarioEntidade? SlotHorario { get; private set; }

    public IEstadoAgendamento Estado { get; private set; }

    public AgendamentoEntidade? Reagendamento { get; private set; }

    public ProcedimentoEntidade Procedimento { get; private set; }

    /// TODO:: TIPO DE COMUNICAÇÃO QUE O AGENDAMENTO FOI CRIADO

    public AgendamentoEntidade(SlotHorarioEntidade slotHorario, ProcedimentoEntidade procedimento, AgendamentoEntidade? reagendamento = null)
    {

        Estado = new AgendamentoPendente();

        Procedimento = procedimento;

        if (reagendamento != null)
            Reagendamento = reagendamento;


        AlocarSlot(slotHorario);

    }

    private AgendamentoEntidade(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade, EstadoAgendamento estadoAgendamento, SlotHorarioEntidade? slotHorario, ProcedimentoEntidade procedimento, AgendamentoEntidade? reagendamento = null)
    : base(id, dataCriacao, dataAlteracao, estadoEntidade)
    {
        Estado = UtilidadesDeEstado.MontaObjetoDeEstado(estadoAgendamento);

        SlotHorario = slotHorario;

        Procedimento = procedimento;

        Reagendamento = reagendamento;
    }

    public void MudarProcedimento(ProcedimentoEntidade procedimento)
    {
        if (EstadoAtual() == EstadoAgendamento.PENDENTE)
            Procedimento = procedimento;

        throw new OperacaoInvalidaExcessao("O procedimento deve ser alterado enquanto o agendamento está pendente");
    }

    public EstadoAgendamento EstadoAtual()
        => Estado.EstadoAtual();

    public AgendamentoEntidade AlterarEstado(EstadoAgendamento novoEstado, SlotHorarioEntidade? slot = null)
    {
        if (Estado is null)
            throw new EntidadeInvalidadeExcessao("Agendamento inicializado sem controlador de estados");


        AgendamentoEntidade @return = this;

        if (novoEstado == EstadoAgendamento.REMARCADO)
        {
            if (slot is null)
            {
                slot = new SlotHorarioEntidade(DateTime.Now);

                slot.AlterarDuracao(Procedimento.TempoEstimado);
            }


            @return = Reagendar(slot);
        }

        Estado = Estado.AlterarEstado(this, novoEstado);

        Atualizar();

        return @return;
    }

    private AgendamentoEntidade Reagendar(SlotHorarioEntidade slot)
    {
        if (AgendamentoFinalizado())
            throw new OperacaoInvalidaExcessao("Não é possivel reagendar um agendamento finalizado");

        if (!slot.VerificarDisponibilidade())
            throw new OperacaoInvalidaExcessao("Slot de agenda já reservado ou confirmado");

        return new AgendamentoEntidade(slot, Procedimento, this);
    }

    public void AlocarSlot(SlotHorarioEntidade slotHorario)
    {
        if (AgendamentoFinalizado())
            throw new OperacaoInvalidaExcessao("Agendamento finalizado!");

        if (SlotHorario != null)
            LiberarSlot();

        if (!slotHorario.VerificarDisponibilidade())
            throw new OperacaoInvalidaExcessao("Slot de agenda já reservado ou confirmado com este procedimento");

        SlotHorario = slotHorario;

        SlotHorario.AlterarDuracao(Procedimento.TempoEstimado);

        SlotHorario.AlterarStatus(StatusSlotAgendamento.RESERVADO);

        Atualizar();
    }

    public void LiberarSlot()
    {
        if (AgendamentoFinalizado())
            throw new OperacaoInvalidaExcessao("Agendamento finalizado!");

        if (SlotHorario is null)
            return;

        SlotHorario.AlterarStatus(StatusSlotAgendamento.DISPONIVEL);

        SlotHorario.AlterarDuracao(TimeSpan.Zero);

        SlotHorario = null;

        Atualizar();
    }

    public bool AgendamentoFinalizado()
        => EstadoAtual() == EstadoAgendamento.CANCELADO || EstadoAtual() == EstadoAgendamento.REMARCADO || EstadoAtual() == EstadoAgendamento.FINALIZADO;

    public static AgendamentoDTO ParaDTO(AgendamentoEntidade entidade)
        => new AgendamentoDTO(entidade.Id.Valor, entidade.DataCriacao, entidade.DataAlteracao, entidade.EstadoEntidade, entidade.SlotHorario is null ? null : SlotHorarioEntidade.ParaDTO(entidade.SlotHorario), entidade.Estado.EstadoAtual(), entidade.Reagendamento is null ? null : ParaDTO(entidade.Reagendamento), ProcedimentoEntidade.ParaDTO(entidade.Procedimento));

    public static AgendamentoEntidade ParaEntidade(AgendamentoDTO agendamento)
    => new AgendamentoEntidade(agendamento.Id, agendamento.DataCriacao, agendamento.DataAlteracao, agendamento.EstadoEntidade, agendamento.Estado, agendamento.SlotHorario is null ? null : SlotHorarioEntidade.ParaEntidade(agendamento.SlotHorario), ProcedimentoEntidade.ParaEntidade(agendamento.Procedimento), agendamento.Reagendamento is null ? null : ParaEntidade(agendamento.Reagendamento));
}
