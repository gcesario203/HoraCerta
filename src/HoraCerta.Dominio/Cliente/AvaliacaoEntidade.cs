namespace HoraCerta.Dominio.Cliente;

public class AvaliacaoEntidade
{
    public IdEntidade AgendamentoId { get; }

    public IdEntidade ProprietarioId { get; }

    public int Nota { get; }

    public string? Comentario { get; }

    public DateTime DataAvaliacao { get; }

    public AvaliacaoEntidade(IdEntidade agendamentoId, IdEntidade proprietarioId, int nota, string? comentario)
    {
        if (nota is < 1 or > 5)
            throw new OperacaoInvalidaExcessao("A nota deve estar entre 1 e 5");

        AgendamentoId = agendamentoId;
        ProprietarioId = proprietarioId;
        Nota = nota;
        Comentario = comentario;
        DataAvaliacao = DateTime.UtcNow;
    }

    internal AvaliacaoEntidade(
        string agendamentoId,
        string proprietarioId,
        int nota,
        string? comentario,
        DateTime dataAvaliacao)
    {
        AgendamentoId = new IdEntidade(agendamentoId);
        ProprietarioId = new IdEntidade(proprietarioId);
        Nota = nota;
        Comentario = comentario;
        DataAvaliacao = dataAvaliacao;
    }
}
