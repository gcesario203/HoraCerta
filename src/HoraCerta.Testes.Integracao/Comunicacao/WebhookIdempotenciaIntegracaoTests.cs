using NUnit.Framework;

namespace HoraCerta.Testes.Integracao.Comunicacao;

[TestFixture]
public class WebhookIdempotenciaIntegracaoTests : ComunicacaoIntegracaoFixture
{
    [Test]
    public void MarcarProcessado_MessageSidDuplicado_NaoDeveInserirNovamente()
    {
        const string messageSid = "SM1234567890abcdef";

        Assert.That(WebhookRepositorio.JaProcessado(messageSid), Is.False);

        WebhookRepositorio.MarcarProcessado(messageSid);
        WebhookRepositorio.MarcarProcessado(messageSid);

        Assert.That(WebhookRepositorio.JaProcessado(messageSid), Is.True);
        Assert.That(Context.WebhooksTwilioProcessados.Count(), Is.EqualTo(1));
    }
}
