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
    public void Construtor_DeveIniciarComStatusDisponivel()
    {
        // Arrange
        DateTime dataHora = DateTime.Now;

        // Act
        var slot = new SlotHorarioEntidade(dataHora);

        // Assert
        Assert.That(StatusSlotAgendamento.DISPONIVEL, Is.EqualTo(slot.Status));
        Assert.That(dataHora, Is.EqualTo(slot.DataHora));
    }

    [Test]
    public void VerificarDisponibilidade_DeveRetornarTrueQuandoStatusDisponivel()
    {
        // Arrange
        DateTime dataHora = DateTime.Now;
        var slot = new SlotHorarioEntidade(dataHora);

        // Assert
        Assert.That(slot.VerificarDisponibilidade());
    }

    [Test]
    public void VerificarDisponibilidade_DeveRetornarFalseQuandoStatusIndisponivel()
    {
        // Arrange
        DateTime dataHora = DateTime.Now;
        var slot = new SlotHorarioEntidade(dataHora);

        // Mudando o status para indisponível
        slot.AlterarStatus(StatusSlotAgendamento.RESERVADO);

        // Assert
        Assert.That(!slot.VerificarDisponibilidade());
    }

    [Test]
    public void AlterarStatus_DeveAlterarStatusParaReservado()
    {
        // Arrange
        DateTime dataHora = DateTime.Now;
        var slot = new SlotHorarioEntidade(dataHora);

        // Act
        slot.AlterarStatus(StatusSlotAgendamento.RESERVADO);

        // Assert
        Assert.That(StatusSlotAgendamento.RESERVADO, Is.EqualTo(slot.Status));
    }

    [Test]
    public void AlterarStatus_DeveAlterarStatusParaConfirmado()
    {
        // Arrange
        DateTime dataHora = DateTime.Now;
        var slot = new SlotHorarioEntidade(dataHora);

        // Act
        slot.AlterarStatus(StatusSlotAgendamento.CONFIRMADO);

        // Assert
        Assert.That(StatusSlotAgendamento.CONFIRMADO, Is.EqualTo(slot.Status));
    }
}
