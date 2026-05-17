using HoraCerta.Dominio;

namespace HoraCerta.Infaestrutura.Persistencia.Modelos;

public class SlotHorarioModelo : PersistenciaModeloBase
{
    public DateTime Inicio { get; set; }

    public DateTime? Fim { get; set; }

    public StatusSlotAgendamento Status { get; set; }
}
