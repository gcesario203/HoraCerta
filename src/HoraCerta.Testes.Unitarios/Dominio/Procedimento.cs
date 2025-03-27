using HoraCerta.Dominio;
using HoraCerta.Dominio.Procedimento;
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
        }

        [Test]
        public void Procedimento_DeveConverter_ParaDTO()
        {
            var procedimentoDTO = ProcedimentoEntidade.ParaDTO(_procedimento);

            Assert.That(_procedimento.Id.Valor, Is.EqualTo(procedimentoDTO.Id));
            Assert.That(_procedimento.DataAlteracao, Is.EqualTo(procedimentoDTO.DataAlteracao));
            Assert.That(_procedimento.DataCriacao, Is.EqualTo(procedimentoDTO.DataCriacao));
            Assert.That(_procedimento.EstadoEntidade, Is.EqualTo(procedimentoDTO.EstadoEntidade));
            Assert.That(_procedimento.Nome, Is.EqualTo(procedimentoDTO.Nome));
            Assert.That(_procedimento.Valor, Is.EqualTo(procedimentoDTO.Valor));
            Assert.That(_procedimento.TempoEstimado, Is.EqualTo(procedimentoDTO.TempoEstimado));
        }

        [Test]
        public void ProcedimentoDTO_DeveConverter_ParaEntidade()
        {
            var novoProcedimento = new ProcedimentoEntidade("Novo procedimento", 200, TimeSpan.FromHours(5));
            var procedimentoDTO = ProcedimentoEntidade.ParaDTO(novoProcedimento);

            var procedimentoDaDTO = ProcedimentoEntidade.ParaEntidade(procedimentoDTO);

            Assert.That(procedimentoDaDTO.Id.Valor, Is.EqualTo(procedimentoDTO.Id));
            Assert.That(procedimentoDaDTO.DataAlteracao, Is.EqualTo(procedimentoDTO.DataAlteracao));
            Assert.That(procedimentoDaDTO.DataCriacao, Is.EqualTo(procedimentoDTO.DataCriacao));
            Assert.That(procedimentoDaDTO.EstadoEntidade, Is.EqualTo(procedimentoDTO.EstadoEntidade));
            Assert.That(procedimentoDaDTO.Nome, Is.EqualTo(procedimentoDTO.Nome));
            Assert.That(procedimentoDaDTO.Valor, Is.EqualTo(procedimentoDTO.Valor));
            Assert.That(procedimentoDaDTO.TempoEstimado, Is.EqualTo(procedimentoDTO.TempoEstimado));

            
            Assert.That(procedimentoDaDTO.Id.Valor, Is.EqualTo(novoProcedimento.Id.Valor));
            Assert.That(procedimentoDaDTO.DataAlteracao, Is.EqualTo(novoProcedimento.DataAlteracao));
            Assert.That(procedimentoDaDTO.DataCriacao, Is.EqualTo(novoProcedimento.DataCriacao));
            Assert.That(procedimentoDaDTO.EstadoEntidade, Is.EqualTo(novoProcedimento.EstadoEntidade));
            Assert.That(procedimentoDaDTO.Nome, Is.EqualTo(novoProcedimento.Nome));
            Assert.That(procedimentoDaDTO.Valor, Is.EqualTo(novoProcedimento.Valor));
            Assert.That(procedimentoDaDTO.TempoEstimado, Is.EqualTo(novoProcedimento.TempoEstimado));
        }
    }

}