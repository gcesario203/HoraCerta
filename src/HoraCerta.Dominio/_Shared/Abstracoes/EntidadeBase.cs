using HoraCerta.Dominio;

namespace HoraCerta.Dominio;

public abstract class EntidadeBase<TEntity> where TEntity : EntidadeBase<TEntity>
{
    public IdEntidade Id { get; }

    public DateTime DataCriacao { get; }

    public DateTime? DataAlteracao { get; private set; }

    protected readonly IServicoValidacao<TEntity> _validador;

    protected EntidadeBase(IServicoValidacao<TEntity> validador = null)
    {
        Id = new IdEntidade();

        DataCriacao = DateTime.UtcNow;

        if (validador is null)
            return;

        _validador = validador;
    }

    public virtual void Atualizar()
    {
        DataAlteracao = DateTime.UtcNow;
    }
}
