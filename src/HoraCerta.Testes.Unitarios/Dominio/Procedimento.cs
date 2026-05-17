using HoraCerta.Dominio;
using HoraCerta.Dominio.Procedimento;
using HoraCerta.Infaestrutura.Mapeamento;
using NUnit.Framework;

namespace HoraCerta.Testes.Unitarios.Dominio
{
    [TestFixture]
    public class Procedimento
    {
        private ProcedimentoEntidade _procedimento = new ProcedimentoEntidade("Teste", 100, TimeSpan.FromHours(3));

        [Test]
        public void CriarProcedimento_DeveCriarComDadosValidos()
        {
            // Arrange
            string nome = "Limpeza de Pele";
            decimal valor = 150.00m;
            TimeSpan duracao = TimeSpan.FromHours(1);

            // Act
            var procedimento = new ProcedimentoEntidade(nome, valor, duracao);

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
            Assert.Catch<EntidadeInvalidadeExcessao>(() => new ProcedimentoEntidade(nome, valor, duracao));

            Assert.Catch<EntidadeInvalidadeExcessao>(() => _procedimento.AtualizarNome(nome));

            nome = "ABJKSBJKASBJKASBJKASBASJKBJKASASBJKBASJKASBJKASJKLJKLÇASKLHJASHJKASGHJASGHJASHJGASGHJASHJVVAHJSJAHSVHJVASAS";
            Assert.Catch<EntidadeInvalidadeExcessao>(() => new ProcedimentoEntidade(nome, valor, duracao));

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
            Assert.Catch<EntidadeInvalidadeExcessao>(() => new ProcedimentoEntidade(nome, valor, duracao));
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
            Assert.Catch<EntidadeInvalidadeExcessao>(() => new ProcedimentoEntidade(nome, valor, duracao));

            Assert.Catch<EntidadeInvalidadeExcessao>(() => _procedimento.AtualizarTempoEstimado(duracao));

            duracao = TimeSpan.FromDays(2);
            // Act & Assert
            Assert.Catch<EntidadeInvalidadeExcessao>(() => new ProcedimentoEntidade(nome, valor, duracao));

            Assert.Catch<EntidadeInvalidadeExcessao>(() => _procedimento.AtualizarTempoEstimado(duracao));
        }

        [Test]
        public void Procedimento_DeveConverter_ParaModelo()
        {
            var modelo = ProcedimentoMapper.ParaModelo(_procedimento);

            Assert.That(_procedimento.Id.Valor, Is.EqualTo(modelo.Id));
            Assert.That(_procedimento.DataAlteracao, Is.EqualTo(modelo.DataAlteracao));
            Assert.That(_procedimento.DataCriacao, Is.EqualTo(modelo.DataCriacao));
            Assert.That(_procedimento.EstadoEntidade, Is.EqualTo(modelo.EstadoEntidade));
            Assert.That(_procedimento.Nome, Is.EqualTo(modelo.Nome));
            Assert.That(_procedimento.Valor, Is.EqualTo(modelo.Valor));
            Assert.That(_procedimento.TempoEstimado, Is.EqualTo(modelo.TempoEstimado));
        }

        [Test]
        public void ProcedimentoModelo_DeveConverter_ParaEntidade()
        {
            var novoProcedimento = new ProcedimentoEntidade("Novo procedimento", 200, TimeSpan.FromHours(5));
            var modelo = ProcedimentoMapper.ParaModelo(novoProcedimento);

            var procedimentoDaModelo = ProcedimentoMapper.ParaEntidade(modelo);

            Assert.That(procedimentoDaModelo.Id.Valor, Is.EqualTo(modelo.Id));
            Assert.That(procedimentoDaModelo.DataAlteracao, Is.EqualTo(modelo.DataAlteracao));
            Assert.That(procedimentoDaModelo.DataCriacao, Is.EqualTo(modelo.DataCriacao));
            Assert.That(procedimentoDaModelo.EstadoEntidade, Is.EqualTo(modelo.EstadoEntidade));
            Assert.That(procedimentoDaModelo.Nome, Is.EqualTo(modelo.Nome));
            Assert.That(procedimentoDaModelo.Valor, Is.EqualTo(modelo.Valor));
            Assert.That(procedimentoDaModelo.TempoEstimado, Is.EqualTo(modelo.TempoEstimado));

            Assert.That(procedimentoDaModelo.Id.Valor, Is.EqualTo(novoProcedimento.Id.Valor));
            Assert.That(procedimentoDaModelo.DataAlteracao, Is.EqualTo(novoProcedimento.DataAlteracao));
            Assert.That(procedimentoDaModelo.DataCriacao, Is.EqualTo(novoProcedimento.DataCriacao));
            Assert.That(procedimentoDaModelo.EstadoEntidade, Is.EqualTo(novoProcedimento.EstadoEntidade));
            Assert.That(procedimentoDaModelo.Nome, Is.EqualTo(novoProcedimento.Nome));
            Assert.That(procedimentoDaModelo.Valor, Is.EqualTo(novoProcedimento.Valor));
            Assert.That(procedimentoDaModelo.TempoEstimado, Is.EqualTo(novoProcedimento.TempoEstimado));
        }
    }

}