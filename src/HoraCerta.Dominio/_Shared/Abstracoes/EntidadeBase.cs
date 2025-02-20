using HoraCerta.Dominio;
using HoraCerta.Dominio._Shared.Enums;

namespace HoraCerta.Dominio;

public abstract class EntidadeBase<TEntity> where TEntity : EntidadeBase<TEntity>
{
    public IdEntidade Id { get; }

    public DateTime DataCriacao { get; }

    public DateTime? DataAlteracao { get; private set; }

    public EstadoEntidade EstadoEntidade { get; private set; } = EstadoEntidade.ATIVO;

    protected readonly IServicoValidacao<TEntity> _validador;

    protected EntidadeBase(IServicoValidacao<TEntity> validador = null)
    {
        Id = new IdEntidade();

        DataCriacao = DateTime.UtcNow;

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
