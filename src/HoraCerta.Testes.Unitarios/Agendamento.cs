using HoraCerta.Dominio;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Procedimento;
using NUnit.Framework;
using System;

namespace HoraCerta.Testes.Unitarios.Dominio
{
    [TestFixture]
    public class Agendamento
    {
        private readonly SlotHorarioEntidade slotHorario1 = new SlotHorarioEntidade(DateTime.Now);
        private readonly SlotHorarioEntidade slotHorario2 = new SlotHorarioEntidade(DateTime.Now.AddDays(-2));
        private readonly ProcedimentoEntidade procedimento1 = new ProcedimentoEntidade("Cabelo", 120, TimeSpan.FromHours(2));
        private readonly ProcedimentoEntidade procedimento2 = new ProcedimentoEntidade("Cabelo 2", 140, TimeSpan.FromHours(4));
        private readonly ClienteEntidade client = new ClienteEntidade("Alberto", "(11) 91234-5678");

        [Test]
        public void Construtor_DeveIniciarAgendamento_Pendente()
        {
            var agendamento = new AgendamentoEntidade(slotHorario2, procedimento1, client);

            Assert.That(agendamento.EstadoAtual(), Is.EqualTo(EstadoAgendamento.PENDENTE));
            Assert.That(agendamento.SlotHorario, Is.Not.Null);
            Assert.That(agendamento.Procedimento, Is.Not.Null);
            Assert.That(agendamento.Reagendamento, Is.Null);
            Assert.That(agendamento.Estado, Is.TypeOf<AgendamentoPendente>());
            Assert.That(agendamento.Cliente != null);
        }

        [Test]
        public void AlterarSlot_DeveAlterarSlot()
        {
            var agendamento = new AgendamentoEntidade(slotHorario2, procedimento1, client);

            Assert.That(agendamento.SlotHorario.Status, Is.EqualTo(StatusSlotAgendamento.RESERVADO));

            agendamento.AlocarSlot(slotHorario1);

            Assert.That(agendamento.SlotHorario.Id.Valor, Is.EqualTo(slotHorario1.Id.Valor));
            Assert.That(slotHorario1.Status, Is.EqualTo(StatusSlotAgendamento.RESERVADO));
            Assert.That(slotHorario2.Status, Is.EqualTo(StatusSlotAgendamento.DISPONIVEL));
        }

        [Test]
        public void NaoDeveIniciarAgendamentoComSlotIndisponivel()
        {
            slotHorario2.AlterarStatus(StatusSlotAgendamento.RESERVADO);
            Assert.Catch<OperacaoInvalidaExcessao>(() => new AgendamentoEntidade(slotHorario2, procedimento2, client));
        }

        [Test]
        public void LiberarSlotDeveRemoverSlotDoAgendamento()
        {
            slotHorario2.AlterarStatus(StatusSlotAgendamento.DISPONIVEL);
            var agendamento = new AgendamentoEntidade(slotHorario2, procedimento2, client);

            agendamento.LiberarSlot();

            Assert.That(agendamento.SlotHorario, Is.Null);
            Assert.That(slotHorario2.Status, Is.EqualTo(StatusSlotAgendamento.DISPONIVEL));
        }

        [Test]
        public void AlterarStatus_AgendamentoConfirmado()
        {
            var slot = new SlotHorarioEntidade(DateTime.Now);
            var agendamento = new AgendamentoEntidade(slot, procedimento1, client);

            agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);

            Assert.That(agendamento.SlotHorario.Status, Is.EqualTo(StatusSlotAgendamento.RESERVADO));
            Assert.That(agendamento.EstadoAtual(), Is.EqualTo(EstadoAgendamento.CONFIRMADO));
            Assert.That(agendamento.Estado, Is.TypeOf<AgendamentoConfirmado>());
        }

        [Test]
        public void AlterarStatus_CancelarAgendamento()
        {
            var slot = new SlotHorarioEntidade(DateTime.Now);
            var agendamento = new AgendamentoEntidade(slot, procedimento1, client);

            agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);
            agendamento.AlterarEstado(EstadoAgendamento.CANCELADO);

            Assert.That(agendamento.EstadoAtual(), Is.EqualTo(EstadoAgendamento.CANCELADO));
            Assert.That(agendamento.Estado, Is.TypeOf<AgendamentoCancelado>());
            Assert.That(agendamento.SlotHorario, Is.Null);

            Assert.Catch<OperacaoInvalidaExcessao>(() => agendamento.AlterarEstado(EstadoAgendamento.PENDENTE));
            Assert.Catch<OperacaoInvalidaExcessao>(() => agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO));
        }

        [Test]
        public void AlterarStatus_Reagendar()
        {
            var slot = new  SlotHorarioEntidade(DateTime.Now);
            var agendamento = new AgendamentoEntidade(slot, procedimento1, client);

            agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);
            var remarcado = agendamento.AlterarEstado(EstadoAgendamento.REMARCADO);

            Assert.That(agendamento.EstadoAtual(), Is.EqualTo(EstadoAgendamento.REMARCADO));
            Assert.That(agendamento.Estado, Is.TypeOf<AgendamentoRemarcado>());
            Assert.That(remarcado.EstadoAtual(), Is.EqualTo(EstadoAgendamento.PENDENTE));
            Assert.That(remarcado.Cliente.Id.Valor == agendamento.Cliente.Id.Valor);

            Assert.Catch<OperacaoInvalidaExcessao>(() => agendamento.AlterarEstado(EstadoAgendamento.PENDENTE));
        }

        [Test]
        public void AlterarStatus_FinalizarAgendamento()
        {
            var slot = new SlotHorarioEntidade(DateTime.Now);
            var agendamento = new AgendamentoEntidade(slot, procedimento1, client);

            agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);
            agendamento.AlterarEstado(EstadoAgendamento.FINALIZADO);

            Assert.That(agendamento.EstadoAtual(), Is.EqualTo(EstadoAgendamento.FINALIZADO));
            Assert.That(agendamento.Estado, Is.TypeOf<AgendamentoFinalizado>());

            Assert.Catch<OperacaoInvalidaExcessao>(() => agendamento.AlterarEstado(EstadoAgendamento.PENDENTE));
        }
    }
}
