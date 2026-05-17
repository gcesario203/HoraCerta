namespace HoraCerta.Infaestrutura.Persistencia.Modelos;

public class ClienteModelo : PersistenciaModeloBase
{
    public string Nome { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public List<AgendamentoModelo> Agendamentos { get; set; } = [];

    public List<AvaliacaoModelo> Avaliacoes { get; set; } = [];
}
