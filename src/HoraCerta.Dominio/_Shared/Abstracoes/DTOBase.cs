using HoraCerta.Dominio._Shared.Enums;

namespace HoraCerta.Dominio._Shared;

public abstract class DTOBase : IDTO
{
    public string Id { get; protected set; }

    public DateTime DataCriacao { get; protected set; }

    public DateTime? DataAlteracao { get; protected set; }

    public EstadoEntidade EstadoEntidade { get; protected set; }

    protected DTOBase(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade)
    {
        Id = id;
        DataCriacao = dataCriacao;
        DataAlteracao = dataAlteracao;
        EstadoEntidade = estadoEntidade;
    }
}
