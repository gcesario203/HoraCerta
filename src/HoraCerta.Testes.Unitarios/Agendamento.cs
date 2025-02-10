using HoraCerta.Dominio;
using NUnit.Framework;
using System;

namespace HoraCerta.Testes.Unitarios
{
    [TestFixture]
    public class AgendamentoTestes
    {
        private readonly Dominio.SlotHorario slotHorario1 = new Dominio.SlotHorario(DateTime.Now);
        private readonly Dominio.SlotHorario slotHorario2 = new Dominio.SlotHorario(DateTime.Now.AddDays(-2));
        private readonly Procedimento procedimento1 = new Procedimento("Cabelo", 120, TimeSpan.FromHours(2));
        private readonly Procedimento procedimento2 = new Procedimento("Cabelo 2", 140, TimeSpan.FromHours(4));

        [Test]
        public void Construtor_DeveIniciarAgendamento_Pendente()
        {
            var agendamento = new Agendamento(slotHorario2, procedimento1);

            Assert.That(agendamento.EstadoAtual(), Is.EqualTo(EstadoAgendamento.PENDENTE));
            Assert.That(agendamento.SlotHorario, Is.Not.Null);
            Assert.That(agendamento.Procedimento, Is.Not.Null);
            Assert.That(agendamento.Reagendamento, Is.Null);
            Assert.That(agendamento.Estado, Is.TypeOf<AgendamentoPendente>());
        }

        [Test]
        public void AlterarSlot_DeveAlterarSlot()
        {
            var agendamento = new Agendamento(slotHorario2, procedimento1);

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
            Assert.Catch<OperacaoInvalidaExcessao>(() => new Agendamento(slotHorario2, procedimento2));
        }

        [Test]
        public void LiberarSlotDeveRemoverSlotDoAgendamento()
        {
            slotHorario2.AlterarStatus(StatusSlotAgendamento.DISPONIVEL);
            var agendamento = new Agendamento(slotHorario2, procedimento2);

            agendamento.LiberarSlot();

            Assert.That(agendamento.SlotHorario, Is.Null);
            Assert.That(slotHorario2.Status, Is.EqualTo(StatusSlotAgendamento.DISPONIVEL));
        }

        [Test]
        public void AlterarStatus_AgendamentoConfirmado()
        {
            var slot = new Dominio.SlotHorario(DateTime.Now);
            var agendamento = new Agendamento(slot, procedimento1);

            agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);

            Assert.That(agendamento.SlotHorario.Status, Is.EqualTo(StatusSlotAgendamento.RESERVADO));
            Assert.That(agendamento.EstadoAtual(), Is.EqualTo(EstadoAgendamento.CONFIRMADO));
            Assert.That(agendamento.Estado, Is.TypeOf<AgendamentoConfirmado>());
        }

        [Test]
        public void AlterarStatus_CancelarAgendamento()
        {
            var slot = new Dominio.SlotHorario(DateTime.Now);
            var agendamento = new Agendamento(slot, procedimento1);

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
            var slot = new Dominio.SlotHorario(DateTime.Now);
            var agendamento = new Agendamento(slot, procedimento1);

            agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);
            var remarcado = agendamento.AlterarEstado(EstadoAgendamento.REMARCADO);

            Assert.That(agendamento.EstadoAtual(), Is.EqualTo(EstadoAgendamento.REMARCADO));
            Assert.That(agendamento.Estado, Is.TypeOf<AgendamentoRemarcado>());
            Assert.That(remarcado.EstadoAtual(), Is.EqualTo(EstadoAgendamento.PENDENTE));

            Assert.Catch<OperacaoInvalidaExcessao>(() => agendamento.AlterarEstado(EstadoAgendamento.PENDENTE));
        }

        [Test]
        public void AlterarStatus_FinalizarAgendamento()
        {
            var slot = new Dominio.SlotHorario(DateTime.Now);
            var agendamento = new Agendamento(slot, procedimento1);

            agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);
            agendamento.AlterarEstado(EstadoAgendamento.FINALIZADO);

            Assert.That(agendamento.EstadoAtual(), Is.EqualTo(EstadoAgendamento.FINALIZADO));
            Assert.That(agendamento.Estado, Is.TypeOf<AgendamentoFinalizado>());

            Assert.Catch<OperacaoInvalidaExcessao>(() => agendamento.AlterarEstado(EstadoAgendamento.PENDENTE));
        }
    }
}
