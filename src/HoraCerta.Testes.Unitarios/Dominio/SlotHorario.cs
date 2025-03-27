using HoraCerta.Dominio;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Testes.Unitarios.Dominio;

[TestFixture]
public class SlotHorario
{
    [Test]
    public void CriarSlotHorario_DeveIniciarComStatusDisponivel()
    {
        // Arrange
        DateTime inicio = new DateTime(2025, 2, 21, 10, 0, 0);

        // Act
        var slot = new SlotHorarioEntidade(inicio);

        // Assert
        Assert.That(inicio == slot.Inicio);
        Assert.That(StatusSlotAgendamento.DISPONIVEL == slot.Status);
    }

    [Test]
    public void AlterarStatus_DeveAtualizarStatusCorretamente()
    {
        // Arrange
        var slot = new SlotHorarioEntidade(new DateTime(2025, 2, 21, 10, 0, 0));

        // Act
        slot.AlterarStatus(StatusSlotAgendamento.RESERVADO);

        // Assert
        Assert.That(StatusSlotAgendamento.RESERVADO == slot.Status);
    }

    [Test]
    public void VerificarDisponibilidade_DeveRetornarTrue_SeDisponivel()
    {
        // Arrange
        var slot = new SlotHorarioEntidade(new DateTime(2025, 2, 21, 10, 0, 0));

        // Act
        bool disponivel = slot.VerificarDisponibilidade();

        // Assert
        Assert.That(disponivel);
    }

    [Test]
    public void VerificarDisponibilidade_DeveRetornarFalse_SeNaoDisponivel()
    {
        // Arrange
        var slot = new SlotHorarioEntidade(new DateTime(2025, 2, 21, 10, 0, 0));
        slot.AlterarStatus(StatusSlotAgendamento.RESERVADO);

        // Act
        bool disponivel = slot.VerificarDisponibilidade();

        // Assert
        Assert.That(!disponivel);
    }

    [Test]
    public void ConflitaCom_DeveRetornarTrue_SeSlotsSeSobreporem()
    {
        // Arrange
        var slot1 = new SlotHorarioEntidade(new DateTime(2025, 2, 21, 10, 0, 0));
        slot1.AlterarDuracao(TimeSpan.FromMinutes(60));

        var slot2 = new SlotHorarioEntidade(new DateTime(2025, 2, 21, 10, 30, 0));
        slot2.AlterarDuracao(TimeSpan.FromMinutes(30));

        // Act
        bool conflito = slot1.ConflitaCom(slot2);

        // Assert
        Assert.That(conflito);
    }

    [Test]
    public void ConflitaCom_DeveRetornarFalse_SeSlotsNaoSeSobreporem()
    {
        // Arrange
        var slot1 = new SlotHorarioEntidade(new DateTime(2025, 2, 21, 10, 0, 0));
        slot1.AlterarDuracao(TimeSpan.FromMinutes(30));

        var slot2 = new SlotHorarioEntidade(new DateTime(2025, 2, 21, 11, 0, 0));
        slot2.AlterarDuracao(TimeSpan.FromMinutes(30));

        // Act
        bool conflito = slot1.ConflitaCom(slot2);

        // Assert
        Assert.That(!conflito);
    }

    [Test]
    public void AlterarDuracao_DeveAtualizarFimSeSlotDisponivel()
    {
        // Arrange
        var slot = new SlotHorarioEntidade(new DateTime(2025, 2, 21, 10, 0, 0));

        // Act
        slot.AlterarDuracao(TimeSpan.FromMinutes(45));

        // Assert
        Assert.That(new DateTime(2025, 2, 21, 10, 45, 0) == slot.Fim);
    }

    [Test]
    public void AlterarDuracao_DeveLancarExcecaoSeSlotNaoDisponivel()
    {
        // Arrange
        var slot = new SlotHorarioEntidade(new DateTime(2025, 2, 21, 10, 0, 0));
        slot.AlterarStatus(StatusSlotAgendamento.RESERVADO);

        // Act & Assert
        Assert.Throws<OperacaoInvalidaExcessao>(() => slot.AlterarDuracao(TimeSpan.FromMinutes(45)));
    }

    [Test]
    public void SlotHorario_DeveConverter_ParaDTO()
    {
        var slot = new SlotHorarioEntidade(new DateTime(2025, 2, 21, 10, 0, 0));
        slot.AlterarStatus(StatusSlotAgendamento.RESERVADO);

        var slotDto = SlotHorarioEntidade.ParaDTO(slot);

        Assert.That(slotDto.Id == slot.Id.Valor);
        Assert.That(slotDto.DataAlteracao == slot.DataAlteracao);
        Assert.That(slotDto.DataCriacao == slot.DataCriacao);
        Assert.That(slotDto.EstadoEntidade == slot.EstadoEntidade);
        Assert.That(slotDto.Inicio == slot.Inicio);
        Assert.That(slotDto.Fim == slot.Fim);
        Assert.That(slotDto.Status == slot.Status);

    }

    [Test]
    public void SlotHorarioDTO_DeveConverter_ParaEntidade()
    {
        var slot = new SlotHorarioEntidade(new DateTime(2025, 2, 21, 10, 0, 0));
        slot.AlterarStatus(StatusSlotAgendamento.RESERVADO);

        var slotDto = SlotHorarioEntidade.ParaDTO(slot);

        Assert.That(slotDto.Id == slot.Id.Valor);
        Assert.That(slotDto.DataAlteracao == slot.DataAlteracao);
        Assert.That(slotDto.DataCriacao == slot.DataCriacao);
        Assert.That(slotDto.EstadoEntidade == slot.EstadoEntidade);
        Assert.That(slotDto.Inicio == slot.Inicio);
        Assert.That(slotDto.Fim == slot.Fim);
        Assert.That(slotDto.Status == slot.Status);

        var slotEntidade = SlotHorarioEntidade.ParaEntidade(slotDto);

        Assert.That(slotDto.Id == slotEntidade.Id.Valor);
        Assert.That(slotDto.DataAlteracao == slotEntidade.DataAlteracao);
        Assert.That(slotDto.DataCriacao == slotEntidade.DataCriacao);
        Assert.That(slotDto.EstadoEntidade == slotEntidade.EstadoEntidade);
        Assert.That(slotDto.Inicio == slotEntidade.Inicio);
        Assert.That(slotDto.Fim == slotEntidade.Fim);
        Assert.That(slotDto.Status == slotEntidade.Status);

    }
}
