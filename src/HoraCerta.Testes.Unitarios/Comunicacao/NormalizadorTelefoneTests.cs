using HoraCerta.Infaestrutura.Comunicacao;
using NUnit.Framework;

namespace HoraCerta.Testes.Unitarios.Comunicacao;

[TestFixture]
public class NormalizadorTelefoneTests
{
    private readonly NormalizadorTelefoneE164 _normalizador = new();

    [Test]
    public void Normalizar_CelularBrasil11Digitos_DeveRetornarE164()
    {
        var resultado = _normalizador.Normalizar("(11) 98765-4321");
        Assert.That(resultado, Is.EqualTo("+5511987654321"));
    }

    [Test]
    public void Normalizar_WhatsAppPrefix_DeveRemoverPrefixo()
    {
        var resultado = _normalizador.Normalizar("whatsapp:+5511987654321");
        Assert.That(resultado, Is.EqualTo("+5511987654321"));
    }

    [Test]
    public void SaoEquivalentes_FormatacoesDiferentes_DeveSerTrue()
    {
        Assert.That(
            _normalizador.SaoEquivalentes("(11) 98765-4321", "+5511987654321"),
            Is.True);
    }
}
