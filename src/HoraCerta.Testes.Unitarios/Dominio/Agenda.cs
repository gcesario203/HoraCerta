﻿using HoraCerta.Dominio.Agenda;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Procedimento;
using HoraCerta.Dominio.Proprietario;
using HoraCerta.Dominio;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoraCerta.Dominio.Cliente;

namespace HoraCerta.Testes.Unitarios.Dominio
{
    [TestFixture]
    public class Agenda
    {
        private GerenciadorAgenda? _gerenciadorAgenda;
        private readonly DateTime now = DateTime.Now;
        private ProprietarioEntidade? _proprietario;
        private List<SlotHorarioEntidade>? _horarios;
        private List<AtendimentoEntidade>? _atendimentos;
        private readonly ProcedimentoEntidade procedimento = new ProcedimentoEntidade("Cabelo 2", 140, TimeSpan.FromHours(4));
        [SetUp]
        public void Setup()
        {
            _proprietario = new ProprietarioEntidade("Roberatao da massa");
            _horarios = new List<SlotHorarioEntidade>();
            _atendimentos = new List<AtendimentoEntidade>();
            _gerenciadorAgenda = new GerenciadorAgenda(_horarios, _atendimentos);
        }

        [Test]
        public void CriarHorarioDisponivel_DeveAdicionarHorarioSeNaoHouverConflito()
        {
            var horario = now;
            _gerenciadorAgenda!.CriarHorarioDisponivel(horario);

            Assert.That(_gerenciadorAgenda.Agenda.Horarios, Has.Exactly(1).Matches<SlotHorarioEntidade>(h => h.Inicio == horario));
        }

        [Test]
        public void CriarHorarioDisponivel_DeveLancarExcecaoSeHouverConflito()
        {
            var horario = now;
            _gerenciadorAgenda!.CriarHorarioDisponivel(horario);

            Assert.Throws<OperacaoInvalidaExcessao>(() => _gerenciadorAgenda.CriarHorarioDisponivel(horario));
        }

        [Test]
        public void CriarAtendimento_DeveCriarAtendimentoSeAgendamentoForValido()
        {
            var agendamento = CriarAgendamentoValido();
            _gerenciadorAgenda!.CriarAtendimento(agendamento);

            Assert.That(agendamento.EstadoAtual(), Is.EqualTo(EstadoAgendamento.FINALIZADO));
        }

        [Test]
        public void CriarAtendimento_DeveLancarExcecaoSeAgendamentoForInvalido()
        {
            var slot = new SlotHorarioEntidade(DateTime.Now);
            var agendamento = new AgendamentoEntidade(slot, procedimento);
            Assert.Throws<OperacaoInvalidaExcessao>(() => _gerenciadorAgenda!.CriarAtendimento(agendamento));
        }

        [Test]
        public void AlterarStatusAtendimento_DeveAlterarEstadoDoAtendimento()
        {
            var agendamento = CriarAgendamentoValido();
            _gerenciadorAgenda!.CriarAtendimento(agendamento);
            var atendimento = _gerenciadorAgenda.BuscarAtendimentoPorHorario(agendamento!.SlotHorario!.Id!);

            _gerenciadorAgenda.AlterarStatusAtendimento(EstadoAtendimento.CANCELADO, atendimento.Id);

            Assert.That(atendimento.EstadoAtual(), Is.EqualTo(EstadoAtendimento.CANCELADO));
        }

        [Test]
        public void DeveCriarAgendaComValoresPreenchidos()
        {
            var agendamentoValido = CriarAgendamentoValido();

            agendamentoValido.AlterarEstado(EstadoAgendamento.FINALIZADO);

            var novoAtendimento = new AtendimentoEntidade(agendamentoValido, agendamentoValido.Procedimento.Valor);

            var novoSlot = new SlotHorarioEntidade(DateTime.Now);

            var novaAgenda = new GerenciadorAgenda( new List<SlotHorarioEntidade> { novoSlot }, new List<AtendimentoEntidade> { novoAtendimento });

            Assert.That(novaAgenda.Agenda.Horarios, Has.Exactly(1).Matches<SlotHorarioEntidade>(h => h.Inicio == novoSlot.Inicio));
            Assert.That(novaAgenda.Agenda.Atendimentos, Has.Exactly(1).Matches<AtendimentoEntidade>(h => h.Id.Valor == novoAtendimento.Id.Valor));
        }

        [Test]
        public void DeveRetornarExcessãoNasBuscarComIdInexistente()
        {
            var agendamento = CriarAgendamentoValido();
            _gerenciadorAgenda!.CriarAtendimento(agendamento);
            var atendimento = _gerenciadorAgenda.BuscarAtendimentoPorHorario(agendamento!.SlotHorario!.Id!);

            Assert.Catch<OperacaoInvalidaExcessao>(() => _gerenciadorAgenda.BuscarAtendimentoPorHorario(new IdEntidade(new Guid().ToString())));
            Assert.Catch<OperacaoInvalidaExcessao>(() => _gerenciadorAgenda.BuscarAtendimentoPorId(new IdEntidade(new Guid().ToString())));
        }

        [Test]
        public void NaoDeveCriarAtendimentoComAtendimentosQueCoincidem()
        {
            var agendamento = CriarAgendamentoValido();
            _gerenciadorAgenda!.CriarAtendimento(agendamento);

            var agendamento2 = CriarAgendamentoValido2();

            Assert.Catch<OperacaoInvalidaExcessao>(() => _gerenciadorAgenda!.CriarAtendimento(agendamento2));
        }

        [Test]
        public void DeveCriarReagendamentoCasoTiverSlotDisponivelConflitante()
        {
            var now = DateTime.Now;

            _gerenciadorAgenda!.CriarHorarioDisponivel(now);

            var agendamento = CriarAgendamentoValido3(now);
            var novoAgendamento = _gerenciadorAgenda!.CriarAtendimento(agendamento);

            Assert.That(novoAgendamento.EstadoAtual() == EstadoAgendamento.PENDENTE && novoAgendamento.Reagendamento != null);
        }

        [Test]
        public void NaoDeveCriarAtendimentoCasoHorarioJaConfirmado()
        {
            var now = DateTime.Now;

            var agendamento = CriarAgendamentoValido2();
            var novoAgendamento = _gerenciadorAgenda!.CriarAtendimento(agendamento);

            Assert.That(novoAgendamento.SlotHorario!.Status == StatusSlotAgendamento.CONFIRMADO);

            Assert.That(_gerenciadorAgenda!.Agenda.Horarios, Has.Exactly(1).Matches<SlotHorarioEntidade>(h => h.Inicio == novoAgendamento.SlotHorario.Inicio && h.Fim == novoAgendamento.SlotHorario.Fim && h.Status == novoAgendamento.SlotHorario.Status));

            var agendamentoComMesmoHorario = CriarAgendamentoValido2();

            Assert.Catch<OperacaoInvalidaExcessao>(() => _gerenciadorAgenda.CriarAtendimento(agendamentoComMesmoHorario));

            var atendimento = _gerenciadorAgenda.BuscarAtendimentoPorAgendamento(novoAgendamento.Id);

            _gerenciadorAgenda.AlterarStatusAtendimento(EstadoAtendimento.REALIZADO, atendimento.Id);

            Assert.That(atendimento.EstadoAtual() == EstadoAtendimento.REALIZADO && atendimento.Origem!.SlotHorario!.Id.Valor == novoAgendamento.SlotHorario.Id.Valor);
        }

        private AgendamentoEntidade CriarAgendamentoValido()
        {
            var procedimento = new ProcedimentoEntidade("Procedimento Teste", 100, TimeSpan.FromMinutes(30));
            var slot = new SlotHorarioEntidade(DateTime.Now.AddHours(2), procedimento.TempoEstimado);
            var agendamento = new AgendamentoEntidade(slot, procedimento);
            agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);
            return agendamento;
        }
        private AgendamentoEntidade CriarAgendamentoValido2()
        {
            var procedimento = new ProcedimentoEntidade("Procedimento Teste", 100, TimeSpan.FromMinutes(60));
            var slot = new SlotHorarioEntidade(DateTime.Now.AddHours(1), procedimento.TempoEstimado);
            var agendamento = new AgendamentoEntidade(slot, procedimento);
            agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);
            return agendamento;
        }

        private AgendamentoEntidade CriarAgendamentoValido3(DateTime now)
        {
            var procedimento = new ProcedimentoEntidade("Procedimento Teste", 100, TimeSpan.FromMinutes(30));
            var slot = new SlotHorarioEntidade(now, procedimento.TempoEstimado);
            var agendamento = new AgendamentoEntidade(slot, procedimento);
            agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);
            return agendamento;
        }
    }
}
