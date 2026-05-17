namespace HoraCerta.Infaestrutura.Persistencia.Modelos;

public class ProcedimentoModelo : PersistenciaModeloBase
{
    public string Nome { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public TimeSpan TempoEstimado { get; set; }
}
