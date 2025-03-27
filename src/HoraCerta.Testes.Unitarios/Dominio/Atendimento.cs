using HoraCerta.Dominio;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Procedimento;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Testes.Unitarios.Dominio;

[TestFixture]
public class Atendimento
{
    private readonly ProcedimentoEntidade procedimento1 = new ProcedimentoEntidade("Cabelo", 120, TimeSpan.FromHours(2));
    private readonly ProcedimentoEntidade procedimento2 = new ProcedimentoEntidade("Cabelo 2", 140, TimeSpan.FromHours(4));
    private readonly ClienteEntidade client = new ClienteEntidade("Alberto", "(11) 91234-5678");
    //private readonly AgendamentoEntidade agendamento = new AgendamentoEntidade(slotho)

    [Test]
    public void DeveCriarAtendimentoAPartirDeUmAgendamento()
    {
        var slot = new SlotHorarioEntidade(DateTime.Now);
        var agendamento = new AgendamentoEntidade(slot, procedimento2);

        agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);

        agendamento.AlterarEstado(EstadoAgendamento.FINALIZADO);

        var atendimento = new AtendimentoEntidade(agendamento, 50);

        Assert.That(atendimento.EstadoAtual() == EstadoAtendimento.PENDENTE);

        Assert.That(atendimento.ValorNegociado == 50);

        Assert.That(atendimento!.Origem.SlotHorario!.Status == StatusSlotAgendamento.CONFIRMADO);
    }

    [Test]
    public void NaoDeveCriarComAgendamentoDiferenteDeFinalizado()
    {
        var slot = new SlotHorarioEntidade(DateTime.Now);
        var agendamento = new AgendamentoEntidade(slot, procedimento2);

        Assert.Catch<EntidadeInvalidadeExcessao>(() => new AtendimentoEntidade(agendamento, 50));

        agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);

        Assert.Catch<EntidadeInvalidadeExcessao>(() => new AtendimentoEntidade(agendamento, 50));

        var remarcado = agendamento.AlterarEstado(EstadoAgendamento.REMARCADO);

        Assert.Catch<EntidadeInvalidadeExcessao>(() => new AtendimentoEntidade(agendamento, 50));

        remarcado.AlterarEstado(EstadoAgendamento.CANCELADO);

        Assert.Catch<EntidadeInvalidadeExcessao>(() => new AtendimentoEntidade(agendamento, 50));
    }

    [Test]
    public void NaoDeveCriarComValorNegociadoInvalido()
    {
        var slot = new SlotHorarioEntidade(DateTime.Now);
        var agendamento = new AgendamentoEntidade(slot, procedimento2);

        agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);

        agendamento.AlterarEstado(EstadoAgendamento.FINALIZADO);

        Assert.Catch<EntidadeInvalidadeExcessao>(() => new AtendimentoEntidade(agendamento, -50));
    }

    [Test]
    public void CancelarDeveAlterarStatus()
    {
        var slot = new SlotHorarioEntidade(DateTime.Now);

        var agendamento = new AgendamentoEntidade(slot, procedimento2);

        agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);

        agendamento.AlterarEstado(EstadoAgendamento.FINALIZADO);

        var atendimento = new AtendimentoEntidade(agendamento, 50);

        atendimento.AlterarEstado(EstadoAtendimento.CANCELADO);

        Assert.That(atendimento.EstadoAtual() == EstadoAtendimento.CANCELADO);
    }


    [Test]
    public void FinalizarAtendimentoDeveAlterarStatus()
    {
        var slot = new SlotHorarioEntidade(DateTime.Now);

        var agendamento = new AgendamentoEntidade(slot, procedimento2);

        agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);

        agendamento.AlterarEstado(EstadoAgendamento.FINALIZADO);

        var atendimento = new AtendimentoEntidade(agendamento, 50);

        atendimento.AlterarEstado(EstadoAtendimento.REALIZADO);

        Assert.That(atendimento.EstadoAtual() == EstadoAtendimento.REALIZADO);
    }

    [Test]
    public void DeveAlterarValorNegociadoEnquantoPendente()
    {
        var slot = new SlotHorarioEntidade(DateTime.Now);

        var agendamento = new AgendamentoEntidade(slot, procedimento2);

        agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);

        agendamento.AlterarEstado(EstadoAgendamento.FINALIZADO);

        var atendimento = new AtendimentoEntidade(agendamento, 50);

        atendimento.AlterarValorNegociado(76);

        Assert.That(atendimento.ValorNegociado == 76);
    }

    [Test]
    public void NaoDeveAlterarValorNegociadoInvalido()
    {
        var slot = new SlotHorarioEntidade(DateTime.Now);

        var agendamento = new AgendamentoEntidade(slot, procedimento2);

        agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);

        agendamento.AlterarEstado(EstadoAgendamento.FINALIZADO);

        var atendimento = new AtendimentoEntidade(agendamento, 50);

        Assert.Catch<EntidadeInvalidadeExcessao>(() => atendimento.AlterarValorNegociado(-76));
    }

    [Test]
    public void NaoDeveAlterarValorNegociadoComAtendimentoCancelado()
    {
        var slot = new SlotHorarioEntidade(DateTime.Now);

        var agendamento = new AgendamentoEntidade(slot, procedimento2);

        agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);

        agendamento.AlterarEstado(EstadoAgendamento.FINALIZADO);

        var atendimento = new AtendimentoEntidade(agendamento, 50);

        atendimento.AlterarEstado(EstadoAtendimento.CANCELADO);

        Assert.Catch<OperacaoInvalidaExcessao>(() => atendimento.AlterarValorNegociado(-76));
    }

    [Test]
    public void NaoDeveAlterarValorNegociadoComAtendimentoFinalizado()
    {
        var slot = new SlotHorarioEntidade(DateTime.Now);

        var agendamento = new AgendamentoEntidade(slot, procedimento2);

        agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);

        agendamento.AlterarEstado(EstadoAgendamento.FINALIZADO);

        var atendimento = new AtendimentoEntidade(agendamento, 50);

        atendimento.AlterarEstado(EstadoAtendimento.CANCELADO);

        Assert.Catch<OperacaoInvalidaExcessao>(() => atendimento.AlterarValorNegociado(-76));
    }
}
