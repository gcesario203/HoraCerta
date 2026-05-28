using HoraCerta.Aplicacao.Comunicacao.Dtos;
using HoraCerta.Aplicacao.Comunicacao.Enums;
using HoraCerta.Aplicacao.Comunicacao.Ports;
using NUnit.Framework;

namespace HoraCerta.Testes.Integracao.Comunicacao;

[TestFixture]
public class MensagemOutboxIntegracaoTests : ComunicacaoIntegracaoFixture
{
    [Test]
    public void Enfileirar_ComIdempotencyKeyDuplicada_NaoDeveCriarSegundoRegistro()
    {
        Enfileirador.Enfileirar(
            TipoMensagemOutbox.NotificacaoConfirmacao,
            "+5511987654321",
            Guid.NewGuid().ToString(),
            "Mensagem 1",
            "Confirmacao:ag-1");

        Enfileirador.Enfileirar(
            TipoMensagemOutbox.NotificacaoConfirmacao,
            "+5511987654321",
            Guid.NewGuid().ToString(),
            "Mensagem 2",
            "Confirmacao:ag-1");

        Assert.That(Context.MensagensOutbox.Count(), Is.EqualTo(1));
        Assert.That(Context.MensagensOutbox.Single().Corpo, Is.EqualTo("Mensagem 1"));
    }

    [Test]
    public void ReservarPendentes_DeveMarcarComoProcessando()
    {
        OutboxRepositorio.Adicionar(new MensagemOutboxPendente(
            Guid.NewGuid().ToString(),
            TipoMensagemOutbox.Lembrete,
            "+5511999999999",
            Guid.NewGuid().ToString(),
            "Lembrete teste",
            "Lembrete:ag-1",
            null,
            0,
            DateTime.UtcNow.AddMinutes(-1)));

        var reservados = OutboxRepositorio.ReservarPendentes(DateTime.UtcNow, 10);

        Assert.That(reservados, Has.Count.EqualTo(1));
        Assert.That(
            Context.MensagensOutbox.Single().Status,
            Is.EqualTo(nameof(StatusMensagemOutbox.Processando)));
    }

    [Test]
    public void RegistrarFalha_DeveVoltarParaPendenteComBackoff()
    {
        var id = Guid.NewGuid().ToString();
        OutboxRepositorio.Adicionar(new MensagemOutboxPendente(
            id,
            TipoMensagemOutbox.NotificacaoConfirmacao,
            "+5511888888888",
            Guid.NewGuid().ToString(),
            "Teste falha",
            null,
            null,
            0,
            DateTime.UtcNow));

        OutboxRepositorio.ReservarPendentes(DateTime.UtcNow, 1);
        var proxima = DateTime.UtcNow.AddMinutes(5);
        OutboxRepositorio.RegistrarFalha(id, "Twilio timeout", proxima, 1);

        var registro = Context.MensagensOutbox.Single();
        Assert.That(registro.Status, Is.EqualTo(nameof(StatusMensagemOutbox.Pendente)));
        Assert.That(registro.Tentativas, Is.EqualTo(1));
        Assert.That(registro.ProximaTentativaEm, Is.EqualTo(proxima));
        Assert.That(registro.UltimoErro, Is.EqualTo("Twilio timeout"));
    }

    [Test]
    public void CancelarPorAgendamento_DeveCancelarItensPendentesComAgendamentoNoPayload()
    {
        var agendamentoId = Guid.NewGuid().ToString();
        var payload = System.Text.Json.JsonSerializer.Serialize(new OutboxPayloadDto(agendamentoId));

        OutboxRepositorio.Adicionar(new MensagemOutboxPendente(
            Guid.NewGuid().ToString(),
            TipoMensagemOutbox.NotificacaoConfirmacao,
            "+5511777777777",
            Guid.NewGuid().ToString(),
            "Confirmacao",
            null,
            payload,
            0,
            DateTime.UtcNow));

        OutboxRepositorio.CancelarPorAgendamento(agendamentoId);

        Assert.That(
            Context.MensagensOutbox.Single().Status,
            Is.EqualTo(nameof(StatusMensagemOutbox.Cancelado)));
    }
}
