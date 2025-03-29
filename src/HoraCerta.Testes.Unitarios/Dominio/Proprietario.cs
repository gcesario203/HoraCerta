using NUnit.Framework;
using HoraCerta.Dominio.Proprietario;
using HoraCerta.Dominio.Procedimento;
using HoraCerta.Dominio;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Agenda;
using HoraCerta.Dominio.Atendimento;

namespace HoraCerta.Testes.Unitarios.Dominio
{
    [TestFixture]
    public class Proprietario
    {
        private ProprietarioEntidade? _proprietario;

        [SetUp]
        public void SetUp()
        {

            _proprietario = new ProprietarioEntidade("Proprietário Teste");
        }

        [Test]
        public void ProprietarioEntidade_DeveDefinirNomeCorretamente()
        {
            // Arrange
            string nomeEsperado = "João da Silva";

            // Act
            var proprietario = new ProprietarioEntidade(nomeEsperado);

            // Assert
            Assert.That(nomeEsperado == proprietario.Nome);
        }

        [Test]
        public void AtualizarNome_DeveAlterarONomeCorretamente()
        {
            // Arrange
            var proprietario = new ProprietarioEntidade("Nome Antigo");
            string novoNome = "Novo Nome";

            // Act
            proprietario.AtualizarNome(novoNome);

            // Assert
            Assert.That(novoNome == proprietario.Nome);
        }

        [Test]
        public void ProprietarioEntidade_NomeVazio_DeveLancarEntidadeInvalidaExcecao()
        {
            // Arrange & Act & Assert
            Assert.Throws<EntidadeInvalidadeExcessao>(() => new ProprietarioEntidade(""));
        }

        [Test]
        public void ProprietarioEntidade_NomeMaiorQue70Caracteres_DeveLancarEntidadeInvalidaExcecao()
        {
            // Arrange
            string nomeInvalido = new string('A', 71);

            // Act & Assert
            Assert.Throws<EntidadeInvalidadeExcessao>(() => new ProprietarioEntidade(nomeInvalido));
        }

        [Test]
        public void GerenciadorProcedimentos_DeveInicializarComListaVazia()
        {
            // Assert
            Assert.That(_proprietario!.GerenciadorProcedimentos!.RecuperarProcedimentos() != null);
            Assert.That(!_proprietario.GerenciadorProcedimentos!.RecuperarProcedimentos()!.Any());
        }

        [Test]
        public void CriarProcedimento_DeveAdicionarNovoProcedimento()
        {
            // Act
            _proprietario!.GerenciadorProcedimentos!.CriarProcedimento("Procedimento Teste", 100m, TimeSpan.FromMinutes(30));

            // Assert
            Assert.That(1 == _proprietario.GerenciadorProcedimentos.RecuperarProcedimentos().Count);
            Assert.That("Procedimento Teste" == _proprietario.GerenciadorProcedimentos.RecuperarProcedimentos().First().Nome);
        }

        [Test]
        public void AlterarProcedimento_DeveAtualizarProcedimentoCorretamente()
        {
            // Arrange
            _proprietario!.GerenciadorProcedimentos!.CriarProcedimento("Procedimento Antigo", 100m, TimeSpan.FromMinutes(30));
            var procedimento = _proprietario.GerenciadorProcedimentos.RecuperarProcedimentos().First();

            // Act
            _proprietario.GerenciadorProcedimentos.AlterarProcedimento(procedimento.Id, "Procedimento Novo", 150m, TimeSpan.FromMinutes(45));

            // Assert
            Assert.That("Procedimento Novo" == procedimento.Nome);
            Assert.That(150m == procedimento.Valor);
            Assert.That(TimeSpan.FromMinutes(45) == procedimento.TempoEstimado);

            procedimento = _proprietario.GerenciadorProcedimentos.BuscarProcedimentoPorId(procedimento.Id);

            Assert.That("Procedimento Novo" == procedimento.Nome);
            Assert.That(150m == procedimento.Valor);
            Assert.That(TimeSpan.FromMinutes(45) == procedimento.TempoEstimado);
        }

        [Test]
        public void InativarProcedimento_DeveAlterarStatusParaInativo()
        {
            // Arrange
            _proprietario!.GerenciadorProcedimentos!.CriarProcedimento("Procedimento Teste", 100m, TimeSpan.FromMinutes(30));
            var procedimento = _proprietario.GerenciadorProcedimentos.RecuperarProcedimentos().First();

            // Act
            _proprietario.GerenciadorProcedimentos.InativarProcedimento(procedimento);

            // Assert
            Assert.That(procedimento!.EstadoEntidade == EstadoEntidade.INATIVO);
        }

        [Test]
        public void AtivarProcedimento_DeveAlterarStatusParaAtivo()
        {
            // Arrange
            _proprietario!.GerenciadorProcedimentos!.CriarProcedimento("Procedimento Teste", 100m, TimeSpan.FromMinutes(30));
            var procedimento = _proprietario.GerenciadorProcedimentos.RecuperarProcedimentos().First();
            _proprietario.GerenciadorProcedimentos.InativarProcedimento(procedimento);

            // Act
            _proprietario.GerenciadorProcedimentos!.AtivarProcedimento(procedimento);

            // Assert
            Assert.That(procedimento!.EstadoEntidade == EstadoEntidade.ATIVO);
        }

        [Test]
        public void DeveCriar_proprietario_GerenciadorProcedimentos_ComProcedimentos()
        {
            _proprietario!.GerenciadorProcedimentos!.CriarProcedimento("Procedimento Teste", 100m, TimeSpan.FromMinutes(30));
            var procedimento = _proprietario.GerenciadorProcedimentos.RecuperarProcedimentos().First();

            var novoGerenciador = new GerenciadorProcedimentos(new List<ProcedimentoEntidade>(){
                procedimento
            });

            Assert.That(novoGerenciador.Procedimentos, Has.Exactly(1).Matches<ProcedimentoEntidade>(h => h.Id.Valor == procedimento.Id.Valor));
        }

        [Test]
        public void NaoDeve_BuscarProcedimento_Inexistente()
        {
            Assert.Catch<OperacaoInvalidaExcessao>(() => _proprietario!.GerenciadorProcedimentos!.BuscarProcedimentoPorId(new IdEntidade(new Guid().ToString())));
        }

        [Test]
        public void CriarHorarioDisponivel_DeveAdicionarHorarioSeNaoHouverConflito()
        {
            var horario = DateTime.Now;
            _proprietario!.GerenciadorAgenda!.CriarHorarioDisponivel(horario);

            Assert.That(_proprietario.GerenciadorAgenda.RecuperarAgenda().Horarios, Has.Exactly(1).Matches<SlotHorarioEntidade>(h => h.Inicio == horario));
        }

        [Test]
        public void CriarHorarioDisponivel_DeveLancarExcecaoSeHouverConflito()
        {
            var horario = DateTime.Now;
            _proprietario!.GerenciadorAgenda!.CriarHorarioDisponivel(horario);

            Assert.Throws<OperacaoInvalidaExcessao>(() => _proprietario.GerenciadorAgenda.CriarHorarioDisponivel(horario));
        }

        [Test]
        public void CriarAtendimento_DeveCriarAtendimentoSeAgendamentoForValido()
        {
            var agendamento = CriarAgendamentoValido();
            _proprietario!.GerenciadorAgenda!.CriarAtendimento(agendamento);

            Assert.That(agendamento.EstadoAtual(), Is.EqualTo(EstadoAgendamento.FINALIZADO));
        }

        [Test]
        public void AlterarStatusAtendimento_DeveAlterarEstadoDoAtendimento()
        {
            var agendamento = CriarAgendamentoValido();
            _proprietario!.GerenciadorAgenda!.CriarAtendimento(agendamento);
            var atendimento = _proprietario.GerenciadorAgenda.BuscarAtendimentoPorHorario(agendamento!.SlotHorario!.Id!);

            _proprietario.GerenciadorAgenda.AlterarStatusAtendimento(EstadoAtendimento.CANCELADO, atendimento.Id);

            Assert.That(atendimento.EstadoAtual(), Is.EqualTo(EstadoAtendimento.CANCELADO));
        }

        [Test]
        public void DeveCriarAgendaComValoresPreenchidos()
        {
            var agendamentoValido = CriarAgendamentoValido();

            agendamentoValido.AlterarEstado(EstadoAgendamento.FINALIZADO);

            var novoAtendimento = new AtendimentoEntidade(agendamentoValido, agendamentoValido.Procedimento.Valor);

            var novoSlot = new SlotHorarioEntidade(DateTime.Now);

            var novaAgenda = new GerenciadorAgenda(new List<SlotHorarioEntidade> { novoSlot }, new List<AtendimentoEntidade> { novoAtendimento });

            Assert.That(novaAgenda.Agenda.Horarios, Has.Exactly(1).Matches<SlotHorarioEntidade>(h => h.Inicio == novoSlot.Inicio));
            Assert.That(novaAgenda.Agenda.Atendimentos, Has.Exactly(1).Matches<AtendimentoEntidade>(h => h.Id.Valor == novoAtendimento.Id.Valor));
        }

        [Test]
        public void DeveRetornarExcessãoNasBuscarComIdInexistente()
        {
            var agendamento = CriarAgendamentoValido();
            _proprietario!.GerenciadorAgenda!.CriarAtendimento(agendamento);
            var atendimento = _proprietario.GerenciadorAgenda.BuscarAtendimentoPorHorario(agendamento!.SlotHorario!.Id!);

            Assert.Catch<OperacaoInvalidaExcessao>(() => _proprietario.GerenciadorAgenda.BuscarAtendimentoPorHorario(new IdEntidade(new Guid().ToString())));
            Assert.Catch<OperacaoInvalidaExcessao>(() => _proprietario.GerenciadorAgenda.BuscarAtendimentoPorId(new IdEntidade(new Guid().ToString())));
        }

        [Test]
        public void NaoDeveCriarAtendimentoComAtendimentosQueCoincidem()
        {
            var agendamento = CriarAgendamentoValido();
            _proprietario!.GerenciadorAgenda!.CriarAtendimento(agendamento);

            var agendamento2 = CriarAgendamentoValido2();

            Assert.Catch<OperacaoInvalidaExcessao>(() => _proprietario.GerenciadorAgenda!.CriarAtendimento(agendamento2));
        }

        [Test]
        public void DeveCriarReagendamentoCasoTiverSlotDisponivelConflitante()
        {
            var now = DateTime.Now;

            _proprietario!.GerenciadorAgenda!.CriarHorarioDisponivel(now);

            var agendamento = CriarAgendamentoValido3(now);
            var novoAgendamento = _proprietario.GerenciadorAgenda!.CriarAtendimento(agendamento);

            Assert.That(novoAgendamento.EstadoAtual() == EstadoAgendamento.PENDENTE && novoAgendamento.Reagendamento != null);
        }

        [Test]
        public void NaoDeveCriarAtendimentoCasoHorarioJaConfirmado()
        {
            var now = DateTime.Now;

            var agendamento = CriarAgendamentoValido2();
            var novoAgendamento = _proprietario!.GerenciadorAgenda!.CriarAtendimento(agendamento);

            Assert.That(novoAgendamento.SlotHorario!.Status == StatusSlotAgendamento.CONFIRMADO);

            Assert.That(_proprietario.GerenciadorAgenda!.RecuperarAgenda().Horarios, Has.Exactly(1).Matches<SlotHorarioEntidade>(h => h.Inicio == novoAgendamento.SlotHorario.Inicio && h.Fim == novoAgendamento.SlotHorario.Fim && h.Status == novoAgendamento.SlotHorario.Status));

            var agendamentoComMesmoHorario = CriarAgendamentoValido2();

            Assert.Catch<OperacaoInvalidaExcessao>(() => _proprietario.GerenciadorAgenda.CriarAtendimento(agendamentoComMesmoHorario));

            var atendimento = _proprietario.GerenciadorAgenda.BuscarAtendimentoPorAgendamento(novoAgendamento.Id);

            _proprietario.GerenciadorAgenda.AlterarStatusAtendimento(EstadoAtendimento.REALIZADO, atendimento.Id);

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
