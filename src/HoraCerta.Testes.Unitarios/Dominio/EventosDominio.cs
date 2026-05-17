using HoraCerta.Dominio;
using HoraCerta.Dominio.Cliente.Eventos;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Procedimento;
using NUnit.Framework;

namespace HoraCerta.Testes.Unitarios.Dominio;

[TestFixture]
public class EventosDominio
{
    [Test]
    public void ConfirmarAgendamento_DeveRegistrarAgendamentoConfirmadoEvent()
    {
        var cliente = new ClienteEntidade("João", "(11) 98888-7777");
        var slot = new SlotHorarioEntidade(DateTime.Now.AddHours(1));
        var procedimento = new ProcedimentoEntidade("Corte", 40m, TimeSpan.FromMinutes(30));

        var agendamento = cliente.GerenciadorAgendamentos.IniciarAgendamento(procedimento, slot);
        cliente.LimparEventosDominio();

        cliente.GerenciadorAgendamentos.ConfirmarAgendamento(agendamento.Id);

        Assert.That(cliente.EventosDominio, Has.Exactly(1).InstanceOf<AgendamentoConfirmadoEvent>());
        var evento = (AgendamentoConfirmadoEvent)cliente.EventosDominio.Single();
        Assert.That(evento.AgendamentoId, Is.EqualTo(agendamento.Id.Valor));
        Assert.That(evento.ClienteId, Is.EqualTo(cliente.Id.Valor));
        Assert.That(evento.TelefoneCliente, Is.EqualTo(cliente.Telefone));
    }
}
