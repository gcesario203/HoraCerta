
using HoraCerta.Dominio.Cliente;
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

    public ClienteEntidade Cliente { get; set; }

    /// TODO:: TIPO DE COMUNICAÇÃO QUE O AGENDAMENTO FOI CRIADO

    public AgendamentoEntidade(SlotHorarioEntidade slotHorario, ProcedimentoEntidade procedimento, ClienteEntidade cliente, AgendamentoEntidade? reagendamento = null)
    {
        
        Estado = new AgendamentoPendente();

        AlocarSlot(slotHorario);
        Procedimento = procedimento;

        Cliente = cliente;

        if(reagendamento != null)
                Reagendamento = reagendamento;

    }

    public void MudarProcedimento(ProcedimentoEntidade procedimento)
    {
        if(EstadoAtual() == EstadoAgendamento.PENDENTE)
            Procedimento = procedimento;

        throw new OperacaoInvalidaExcessao("O procedimento deve ser alterado enquanto o agendamento está pendente");
    }

    public EstadoAgendamento EstadoAtual()
        => Estado.EstadoAtual();

    public AgendamentoEntidade AlterarEstado(EstadoAgendamento novoEstado, SlotHorarioEntidade slot = null)
    {
        if (Estado is null)
            throw new EntidadeInvalidadeExcessao("Agendamento inicializado sem controlador de estados");


        AgendamentoEntidade @return = this;

        if(novoEstado == EstadoAgendamento.REMARCADO)
        {
            if(slot is null)
                slot = new SlotHorarioEntidade(DateTime.Now);


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

        return new AgendamentoEntidade(slot, Procedimento, Cliente, this);
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
