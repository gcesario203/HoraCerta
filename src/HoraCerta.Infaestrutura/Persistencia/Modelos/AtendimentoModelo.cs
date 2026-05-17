using HoraCerta.Dominio.Atendimento;

namespace HoraCerta.Infaestrutura.Persistencia.Modelos;

public class AtendimentoModelo : PersistenciaModeloBase
{
    public AgendamentoModelo Origem { get; set; } = null!;

    public decimal ValorNegociado { get; set; }

    public EstadoAtendimento Estado { get; set; }
}
