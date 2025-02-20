using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Procedimento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Agendamento;

public class GerenciadorAgendamentos : IGerenciadorAgendamentos
{
    private readonly ClienteEntidade _cliente;
    public ICollection<AgendamentoEntidade> Agendamentos { get; private set; }

    public GerenciadorAgendamentos(ClienteEntidade cliente, ICollection<AgendamentoEntidade> agendamentos)
    {
        if (agendamentos is null || !agendamentos.Any())
            Agendamentos = new List<AgendamentoEntidade>();
        else
            Agendamentos = agendamentos;

        _cliente = cliente;
    }
    public AgendamentoEntidade IniciarAgendamento(ProcedimentoEntidade procedimento, SlotHorarioEntidade slot)
    { 
        var agendamento = new AgendamentoEntidade(slot, procedimento, _cliente);

        Agendamentos.Add(agendamento);

        return agendamento;
    }

    public void ConfirmarAgendamento(IdEntidade id)
    {
        var agendamento = BuscarAgendamentoPorId(id);

        agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);
    }

    public void CancelarAgendamento(IdEntidade id)
    {
        var agendamento = BuscarAgendamentoPorId(id);

        agendamento.AlterarEstado(EstadoAgendamento.CANCELADO);
    }

    public AgendamentoEntidade RemarcarAgendamento(IdEntidade id, SlotHorarioEntidade slot)
    {
        var agendamento = BuscarAgendamentoPorId(id);

        var remarcacao = agendamento.AlterarEstado(EstadoAgendamento.REMARCADO, slot);

        Agendamentos.Add(remarcacao);

        return remarcacao;
    }

    public AgendamentoEntidade BuscarAgendamentoPorId(IdEntidade id)
    {
        var agendamento = Agendamentos.FirstOrDefault(x => x.Id.Valor == id.Valor);

        if (agendamento is null)
            throw new OperacaoInvalidaExcessao("Agendamento não encontrado");

        return agendamento;
    }
}
