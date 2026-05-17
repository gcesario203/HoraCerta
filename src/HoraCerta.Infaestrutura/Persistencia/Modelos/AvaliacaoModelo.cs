namespace HoraCerta.Infaestrutura.Persistencia.Modelos;

public class AvaliacaoModelo
{
    public string AgendamentoId { get; set; } = string.Empty;

    public string ProprietarioId { get; set; } = string.Empty;

    public int Nota { get; set; }

    public string? Comentario { get; set; }

    public DateTime DataAvaliacao { get; set; }
}
