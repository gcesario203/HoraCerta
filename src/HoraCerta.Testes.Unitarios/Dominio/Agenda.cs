using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Procedimento;
using HoraCerta.Dominio.Proprietario;
using HoraCerta.Dominio;
using NUnit.Framework;

namespace HoraCerta.Testes.Unitarios.Dominio
{
    [TestFixture]
    public class Agenda
    {
        private ProprietarioEntidade? _proprietario;
        private readonly DateTime now = DateTime.Now;
        private readonly ProcedimentoEntidade procedimento = new ProcedimentoEntidade("Cabelo 2", 140, TimeSpan.FromHours(4));
        private readonly IdEntidade clienteId = new();

        [SetUp]
        public void Setup()
        {
            _proprietario = new ProprietarioEntidade("Roberatao da massa");
        }

        [Test]
        public void CriarHorarioDisponivel_DeveAdicionarHorarioSeNaoHouverConflito()
        {
            var horario = now;
            _proprietario!.GerenciadorAgenda.CriarHorarioDisponivel(horario);

            Assert.That(_proprietario.Horarios, Has.Exactly(1).Matches<SlotHorarioEntidade>(h => h.Inicio == horario));
        }

        [Test]
        public void CriarHorarioDisponivel_DeveLancarExcecaoSeHouverConflito()
        {
            var horario = now;
            _proprietario!.GerenciadorAgenda.CriarHorarioDisponivel(horario);

            Assert.Throws<OperacaoInvalidaExcessao>(() => _proprietario.GerenciadorAgenda.CriarHorarioDisponivel(horario));
        }

        [Test]
        public void CriarAtendimento_DeveCriarAtendimentoSeAgendamentoForValido()
        {
            var agendamento = CriarAgendamentoValido();
            _proprietario!.GerenciadorAgenda.CriarAtendimento(agendamento, clienteId);

            Assert.That(agendamento.EstadoAtual(), Is.EqualTo(EstadoAgendamento.FINALIZADO));
        }

        [Test]
        public void CriarAtendimento_DeveLancarExcecaoSeAgendamentoForInvalido()
        {
            var slot = new SlotHorarioEntidade(DateTime.Now);
            var agendamento = new AgendamentoEntidade(slot, procedimento);
            Assert.Throws<OperacaoInvalidaExcessao>(() => _proprietario!.GerenciadorAgenda.CriarAtendimento(agendamento, clienteId));
        }

        [Test]
        public void AlterarStatusAtendimento_DeveAlterarEstadoDoAtendimento()
        {
            var agendamento = CriarAgendamentoValido();
            _proprietario!.GerenciadorAgenda.CriarAtendimento(agendamento, clienteId);
            var atendimento = _proprietario.GerenciadorAgenda.BuscarAtendimentoPorHorario(agendamento!.SlotHorario!.Id!);

            _proprietario.GerenciadorAgenda.AlterarStatusAtendimento(EstadoAtendimento.CANCELADO, atendimento.Id);

            Assert.That(atendimento.EstadoAtual(), Is.EqualTo(EstadoAtendimento.CANCELADO));
        }

        [Test]
        public void DeveCriarProprietarioComHorariosEAtendimentosPreenchidos()
        {
            var agendamentoValido = CriarAgendamentoValido();

            agendamentoValido.AlterarEstado(EstadoAgendamento.FINALIZADO);

            var novoAtendimento = new AtendimentoEntidade(agendamentoValido, agendamentoValido.Procedimento.Valor);
            var novoSlot = new SlotHorarioEntidade(DateTime.Now);

            var proprietario = new ProprietarioEntidade(
                "Teste",
                horarios: new List<SlotHorarioEntidade> { novoSlot },
                atendimentos: new List<AtendimentoEntidade> { novoAtendimento });

            Assert.That(proprietario.Horarios, Has.Exactly(1).Matches<SlotHorarioEntidade>(h => h.Inicio == novoSlot.Inicio));
            Assert.That(proprietario.Atendimentos, Has.Exactly(1).Matches<AtendimentoEntidade>(h => h.Id.Valor == novoAtendimento.Id.Valor));
        }

        [Test]
        public void DeveRetornarExcessãoNasBuscarComIdInexistente()
        {
            var agendamento = CriarAgendamentoValido();
            _proprietario!.GerenciadorAgenda.CriarAtendimento(agendamento, clienteId);

            Assert.Catch<OperacaoInvalidaExcessao>(() => _proprietario.GerenciadorAgenda.BuscarAtendimentoPorHorario(new IdEntidade(new Guid().ToString())));
            Assert.Catch<OperacaoInvalidaExcessao>(() => _proprietario.GerenciadorAgenda.BuscarAtendimentoPorId(new IdEntidade(new Guid().ToString())));
        }

        [Test]
        public void NaoDeveCriarAtendimentoComAtendimentosQueCoincidem()
        {
            var agendamento = CriarAgendamentoValido();
            _proprietario!.GerenciadorAgenda.CriarAtendimento(agendamento, clienteId);

            var agendamento2 = CriarAgendamentoValido2();

            Assert.Catch<OperacaoInvalidaExcessao>(() => _proprietario.GerenciadorAgenda.CriarAtendimento(agendamento2, clienteId));
        }

        [Test]
        public void DeveCriarReagendamentoCasoTiverSlotDisponivelConflitante()
        {
            var inicio = DateTime.Now;

            _proprietario!.GerenciadorAgenda.CriarHorarioDisponivel(inicio);

            var agendamento = CriarAgendamentoValido3(inicio);
            var novoAgendamento = _proprietario.GerenciadorAgenda.CriarAtendimento(agendamento, clienteId);

            Assert.That(novoAgendamento.EstadoAtual() == EstadoAgendamento.PENDENTE && novoAgendamento.Reagendamento != null);
        }

        [Test]
        public void NaoDeveCriarAtendimentoCasoHorarioJaConfirmado()
        {
            var agendamento = CriarAgendamentoValido2();
            var novoAgendamento = _proprietario!.GerenciadorAgenda.CriarAtendimento(agendamento, clienteId);

            Assert.That(novoAgendamento.SlotHorario!.Status == StatusSlotAgendamento.CONFIRMADO);

            Assert.That(_proprietario.Horarios, Has.Exactly(1).Matches<SlotHorarioEntidade>(h =>
                h.Inicio == novoAgendamento.SlotHorario.Inicio &&
                h.Fim == novoAgendamento.SlotHorario.Fim &&
                h.Status == novoAgendamento.SlotHorario.Status));

            var agendamentoComMesmoHorario = CriarAgendamentoValido2();

            Assert.Catch<OperacaoInvalidaExcessao>(() => _proprietario.GerenciadorAgenda.CriarAtendimento(agendamentoComMesmoHorario, clienteId));

            var atendimento = _proprietario.GerenciadorAgenda.BuscarAtendimentoPorAgendamento(novoAgendamento.Id);

            _proprietario.GerenciadorAgenda.AlterarStatusAtendimento(EstadoAtendimento.REALIZADO, atendimento.Id);

            Assert.That(atendimento.EstadoAtual() == EstadoAtendimento.REALIZADO && atendimento.Origem!.SlotHorario!.Id.Valor == novoAgendamento.SlotHorario.Id.Valor);
        }

        private AgendamentoEntidade CriarAgendamentoValido()
        {
            var proc = new ProcedimentoEntidade("Procedimento Teste", 100, TimeSpan.FromMinutes(30));
            var slot = new SlotHorarioEntidade(DateTime.Now.AddHours(2), proc.TempoEstimado);
            var agendamento = new AgendamentoEntidade(slot, proc);
            agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);
            return agendamento;
        }

        private AgendamentoEntidade CriarAgendamentoValido2()
        {
            var proc = new ProcedimentoEntidade("Procedimento Teste", 100, TimeSpan.FromMinutes(60));
            var slot = new SlotHorarioEntidade(DateTime.Now.AddHours(1), proc.TempoEstimado);
            var agendamento = new AgendamentoEntidade(slot, proc);
            agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);
            return agendamento;
        }

        private AgendamentoEntidade CriarAgendamentoValido3(DateTime inicio)
        {
            var proc = new ProcedimentoEntidade("Procedimento Teste", 100, TimeSpan.FromMinutes(30));
            var slot = new SlotHorarioEntidade(inicio, proc.TempoEstimado);
            var agendamento = new AgendamentoEntidade(slot, proc);
            agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);
            return agendamento;
        }
    }
}
