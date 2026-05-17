using HoraCerta.Dominio.Atendimento;
using NUnit.Framework;

namespace HoraCerta.Testes.Unitarios.Dominio;

[TestFixture]
public class AtendimentoFalhaTests
{
    [Test]
    public void UtilidadesDeEstado_DeveMontarEstadoFalha()
    {
        var estado = UtilidadesDeEstado.MontaObjetoDeEstado(EstadoAtendimento.FALHA);

        Assert.That(estado.EstadoAtual(), Is.EqualTo(EstadoAtendimento.FALHA));
    }

    [Test]
    public void AtendimentoPendente_DeveTransicionarParaFalha()
    {
        var pendente = new AtendimentoPendente();
        var falha = pendente.AlterarEstado(null!, EstadoAtendimento.FALHA);

        Assert.That(falha.EstadoAtual(), Is.EqualTo(EstadoAtendimento.FALHA));
    }
}
