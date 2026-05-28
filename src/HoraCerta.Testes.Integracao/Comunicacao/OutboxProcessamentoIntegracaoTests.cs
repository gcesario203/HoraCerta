using HoraCerta.Aplicacao.Comunicacao.Dtos;
using HoraCerta.Aplicacao.Comunicacao.Enums;
using HoraCerta.Aplicacao.Comunicacao.Ports;
using NUnit.Framework;

namespace HoraCerta.Testes.Integracao.Comunicacao;

[TestFixture]
public class OutboxProcessamentoIntegracaoTests : ComunicacaoIntegracaoFixture
{
    [Test]
    public async Task ProcessarPendentes_ComEnviadorFake_DeveMarcarComoEnviado()
    {
        OutboxRepositorio.Adicionar(new MensagemOutboxPendente(
            Guid.NewGuid().ToString(),
            TipoMensagemOutbox.RespostaBot,
            "+5511966666666",
            Guid.NewGuid().ToString(),
            "Resposta do bot",
            null,
            null,
            0,
            DateTime.UtcNow));

        var enviador = new EnviadorWhatsAppFake();
        var pendentes = OutboxRepositorio.ReservarPendentes(DateTime.UtcNow, 10);

        foreach (var mensagem in pendentes)
        {
            await enviador.EnviarAsync(mensagem.TelefoneDestino, mensagem.Corpo);
            OutboxRepositorio.MarcarEnviado(mensagem.Id);
        }

        Assert.That(enviador.Enviados, Has.Count.EqualTo(1));
        Assert.That(enviador.Enviados[0].Corpo, Is.EqualTo("Resposta do bot"));
        Assert.That(
            Context.MensagensOutbox.Single().Status,
            Is.EqualTo(nameof(StatusMensagemOutbox.Enviado)));
    }

    [Test]
    public void MarcarFalhaDefinitiva_AposMaxTentativas_DeveFicarEmFalha()
    {
        var id = Guid.NewGuid().ToString();
        OutboxRepositorio.Adicionar(new MensagemOutboxPendente(
            id,
            TipoMensagemOutbox.Lembrete,
            "+5511955555555",
            Guid.NewGuid().ToString(),
            "Lembrete",
            null,
            null,
            4,
            DateTime.UtcNow));

        OutboxRepositorio.MarcarFalhaDefinitiva(id, "Twilio indisponível");

        var registro = Context.MensagensOutbox.Single();
        Assert.That(registro.Status, Is.EqualTo(nameof(StatusMensagemOutbox.Falha)));
        Assert.That(registro.UltimoErro, Is.EqualTo("Twilio indisponível"));
    }

    private sealed class EnviadorWhatsAppFake : IEnviadorWhatsApp
    {
        public List<(string Telefone, string Corpo)> Enviados { get; } = [];

        public Task EnviarAsync(string telefoneDestino, string corpo, CancellationToken cancellationToken = default)
        {
            Enviados.Add((telefoneDestino, corpo));
            return Task.CompletedTask;
        }
    }
}
