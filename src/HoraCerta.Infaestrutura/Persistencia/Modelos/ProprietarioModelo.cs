namespace HoraCerta.Infaestrutura.Persistencia.Modelos;

public class ProprietarioModelo : PersistenciaModeloBase
{
    public string Nome { get; set; } = string.Empty;

    public List<SlotHorarioModelo> Horarios { get; set; } = [];

    public List<AtendimentoModelo> Atendimentos { get; set; } = [];

    public List<ProcedimentoModelo> Procedimentos { get; set; } = [];
}
