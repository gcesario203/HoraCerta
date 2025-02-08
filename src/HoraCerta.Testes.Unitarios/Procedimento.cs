using HoraCerta.Dominio._Shared.Excessoes;
using HoraCerta.Dominio.Procedimento;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Testes.Unitarios
{
    [TestFixture]
    public class ProcedimentoTests
    {
        private Procedimento _procedimento = new Procedimento("Teste", 100, TimeSpan.FromHours(3));

        [Test]
        public void CriarProcedimento_DeveCriarComDadosValidos()
        {
            // Arrange
            string nome = "Limpeza de Pele";
            decimal valor = 150.00m;
            TimeSpan duracao = TimeSpan.FromHours(1);

            // Act
            var procedimento = new Procedimento(nome, valor, duracao);

            // Assert
            Assert.That(nome, Is.EqualTo(procedimento.Nome));
            Assert.That(valor, Is.EqualTo(procedimento.Valor));
            Assert.That(duracao, Is.EqualTo(procedimento.TempoEstimado));
        }

        [Test]
        public void CriarProcedimento_DeveLancarExcecao_ParaNomeInvalido()
        {
            // Arrange
            string nome = ""; // Nome vazio
            decimal valor = 150.00m;
            TimeSpan duracao = TimeSpan.FromHours(1);

            // Act & Assert
            Assert.Catch<EntidadeInvalidadeExcessao>(() => new Procedimento(nome, valor, duracao));

            Assert.Catch<EntidadeInvalidadeExcessao>(() => _procedimento.AtualizarNome(nome));
        }

        [Test]
        public void CriarProcedimento_DeveLancarExcecao_ParaValorInvalido()
        {
            // Arrange
            string nome = "Massagem";
            decimal valor = -10.00m; // Valor negativo
            TimeSpan duracao = TimeSpan.FromMinutes(30);

            // Act & Assert
            Assert.Catch<EntidadeInvalidadeExcessao>(() => new Procedimento(nome, valor, duracao));
            Assert.Catch<EntidadeInvalidadeExcessao>(() => _procedimento.AtualizarValor(valor));
        }

        [Test]
        public void CriarProcedimento_DeveLancarExcecao_ParaTempoEstimadoInvalido()
        {
            // Arrange
            string nome = "Massagem";
            decimal valor = 100.00m;
            TimeSpan duracao = TimeSpan.Zero; // Duração inválida

            // Act & Assert
            Assert.Catch<EntidadeInvalidadeExcessao>(() => new Procedimento(nome, valor, duracao));

            Assert.Catch<EntidadeInvalidadeExcessao>(() => _procedimento.AtualizarTempoEstimado(duracao));
        }
    }

}