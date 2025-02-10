
namespace HoraCerta.Dominio;

public class Agendamento : EntidadeBase<Agendamento>
{
    /// <summary>
    /// Slot da agenda cujo qual este agendamento ocupa
    /// </summary>
    public SlotHorario? SlotHorario { get; private set; }

    public IEstadoAgendamento Estado { get; private set; }

    public Agendamento? Reagendamento { get; private set; }

    public Procedimento Procedimento { get; private set; }

    /// TODO:: CLIENTE
    /// 

    /// TODO:: TIPO DE COMUNICAÇÃO QUE O AGENDAMENTO FOI CRIADO

    public Agendamento(SlotHorario slotHorario, Procedimento procedimento, Agendamento? reagendamento = null)
    {
        
        Estado = new AgendamentoPendente();

        AlocarSlot(slotHorario);
        Procedimento = procedimento;

        if(reagendamento != null)
                Reagendamento = reagendamento;

    }

    public void MudarProcedimento(Procedimento procedimento)
    {
        if(EstadoAtual() == EstadoAgendamento.PENDENTE)
            Procedimento = procedimento;

        throw new OperacaoInvalidaExcessao("O procedimento deve ser alterado enquanto o agendamento está pendente");
    }

    public EstadoAgendamento EstadoAtual()
        => Estado.EstadoAtual();

    public Agendamento AlterarEstado(EstadoAgendamento novoEstado)
    {
        if (Estado is null)
            throw new EntidadeInvalidadeExcessao("Agendamento inicializado sem controlador de estados");


        Agendamento @return = this;

        if(novoEstado == EstadoAgendamento.REMARCADO)
        {
            var slot = new SlotHorario(DateTime.Now);


            @return = Reagendar(slot);
        }

        Estado = Estado.AlterarEstado(this, novoEstado);

        Atualizar();

        return @return;
    }

    private Agendamento Reagendar(SlotHorario slot)
    {
        if (AgendamentoFinalizado())
            throw new OperacaoInvalidaExcessao("Não é possivel reagendar um agendamento finalizado");

        if (!slot.VerificarDisponibilidade())
            throw new OperacaoInvalidaExcessao("Slot de agenda já reservado ou confirmado");

        return new Agendamento(slot, Procedimento, this);
    }

    public void AlocarSlot(SlotHorario slotHorario)
    {
        if (AgendamentoFinalizado())
            throw new OperacaoInvalidaExcessao("Agendamento finalizado!");

        if (SlotHorario != null)
            LiberarSlot();

        if (!slotHorario.VerificarDisponibilidade())
            throw new OperacaoInvalidaExcessao("Slot de agenda já reservado ou confirmado com este procedimento");

        SlotHorario = slotHorario;

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

        SlotHorario = null;

        Atualizar();
    }

    public bool AgendamentoFinalizado()
        => EstadoAtual() == EstadoAgendamento.CANCELADO || EstadoAtual() == EstadoAgendamento.REMARCADO || EstadoAtual() == EstadoAgendamento.FINALIZADO;
}
