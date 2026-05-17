using HoraCerta.Dominio._Shared.Enums;

namespace HoraCerta.Infaestrutura.Persistencia.Modelos;

public abstract class PersistenciaModeloBase
{
    public string Id { get; set; } = string.Empty;

    public DateTime DataCriacao { get; set; }

    public DateTime? DataAlteracao { get; set; }

    public EstadoEntidade EstadoEntidade { get; set; }
}
