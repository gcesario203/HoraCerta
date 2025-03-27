using HoraCerta.Dominio;
using HoraCerta.Dominio._Shared.Enums;

namespace HoraCerta.Dominio;

public abstract class EntidadeBase<TEntity> where TEntity : EntidadeBase<TEntity>
{
    public IdEntidade Id { get; protected set; }

    public DateTime DataCriacao { get; protected set; }

    public DateTime? DataAlteracao { get; protected set; }

    public EstadoEntidade EstadoEntidade { get; protected set; } = EstadoEntidade.ATIVO;

    protected readonly IServicoValidacao<TEntity>? _validador;

    protected EntidadeBase(IServicoValidacao<TEntity>? validador = null)
    {
        Id = new IdEntidade();

        DataCriacao = DateTime.UtcNow;

        if (validador is null)
            return;

        _validador = validador;
    }

    protected EntidadeBase(string? id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade, IServicoValidacao<TEntity>? validador = null)
    {
        Id =  string.IsNullOrEmpty(id) ? new IdEntidade() : new IdEntidade(id);
        DataCriacao = dataCriacao;
        DataAlteracao = dataAlteracao;
        EstadoEntidade = estadoEntidade;

        if (validador is null)
            return;

        _validador = validador;
    }

    public void MudarStatus(EstadoEntidade estadoEntidade)
        => EstadoEntidade = estadoEntidade;

    public virtual void Atualizar()
    {
        DataAlteracao = DateTime.UtcNow;
    }
}
