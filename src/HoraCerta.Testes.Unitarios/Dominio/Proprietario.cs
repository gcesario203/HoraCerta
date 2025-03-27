using NUnit.Framework;
using HoraCerta.Dominio.Proprietario;
using HoraCerta.Dominio.Procedimento;
using System.Collections.Generic;
using HoraCerta.Dominio;
using HoraCerta.Dominio._Shared.Enums;

namespace HoraCerta.Testes.Unitarios.Dominio
{
    [TestFixture]
    public class Proprietario
    {
        private ProprietarioEntidade? _proprietario;
        private GerenciadorProcedimentos? _gerenciador;

        [SetUp]
        public void SetUp()
        {
            _proprietario = new ProprietarioEntidade("Proprietário Teste");
            _gerenciador = new GerenciadorProcedimentos(_proprietario, null);
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
            Assert.That(_gerenciador!.Procedimentos != null);
            Assert.That(!_gerenciador!.Procedimentos!.Any());
        }

        [Test]
        public void CriarProcedimento_DeveAdicionarNovoProcedimento()
        {
            // Act
            _gerenciador!.CriarProcedimento("Procedimento Teste", 100m, TimeSpan.FromMinutes(30));

            // Assert
            Assert.That(1 == _gerenciador.Procedimentos.Count);
            Assert.That("Procedimento Teste" == _gerenciador.Procedimentos.First().Nome);
        }

        [Test]
        public void AlterarProcedimento_DeveAtualizarProcedimentoCorretamente()
        {
            // Arrange
            _gerenciador!.CriarProcedimento("Procedimento Antigo", 100m, TimeSpan.FromMinutes(30));
            var procedimento = _gerenciador.Procedimentos.First();

            // Act
            _gerenciador.AlterarProcedimento(procedimento.Id, "Procedimento Novo", 150m, TimeSpan.FromMinutes(45));

            // Assert
            Assert.That("Procedimento Novo" == procedimento.Nome);
            Assert.That(150m == procedimento.Valor);
            Assert.That(TimeSpan.FromMinutes(45) == procedimento.TempoEstimado);

            procedimento = _gerenciador.BuscarProcedimentoPorId(procedimento.Id);

            Assert.That("Procedimento Novo" == procedimento.Nome);
            Assert.That(150m == procedimento.Valor);
            Assert.That(TimeSpan.FromMinutes(45) == procedimento.TempoEstimado);
        }

        [Test]
        public void InativarProcedimento_DeveAlterarStatusParaInativo()
        {
            // Arrange
            _gerenciador!.CriarProcedimento("Procedimento Teste", 100m, TimeSpan.FromMinutes(30));
            var procedimento = _gerenciador.Procedimentos.First();

            // Act
            _gerenciador.InativarProcedimento(procedimento);

            // Assert
            Assert.That(procedimento!.EstadoEntidade == EstadoEntidade.INATIVO);
        }

        [Test]
        public void AtivarProcedimento_DeveAlterarStatusParaAtivo()
        {
            // Arrange
            _gerenciador!.CriarProcedimento("Procedimento Teste", 100m, TimeSpan.FromMinutes(30));
            var procedimento = _gerenciador.Procedimentos.First();
            _gerenciador.InativarProcedimento(procedimento);

            // Act
            _gerenciador!.AtivarProcedimento(procedimento);

            // Assert
            Assert.That(procedimento!.EstadoEntidade == EstadoEntidade.ATIVO);
        }
    }
}
