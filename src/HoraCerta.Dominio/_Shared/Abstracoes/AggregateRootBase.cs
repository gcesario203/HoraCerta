using HoraCerta.Dominio._Shared.Interfaces;

namespace HoraCerta.Dominio._Shared.Abstracoes;

public abstract class AggregateRootBase<TEntity> : EntidadeBase<TEntity>, IAggregateRoot
    where TEntity : AggregateRootBase<TEntity>
{
    private readonly List<IDomainEvent> _eventosDominio = new();

    public IReadOnlyCollection<IDomainEvent> EventosDominio => _eventosDominio.AsReadOnly();

    protected AggregateRootBase(IServicoValidacao<TEntity>? validador = null)
        : base(validador)
    {
    }

    protected AggregateRootBase(
        string? id,
        DateTime dataCriacao,
        DateTime? dataAlteracao,
        _Shared.Enums.EstadoEntidade estadoEntidade,
        IServicoValidacao<TEntity>? validador = null)
        : base(id, dataCriacao, dataAlteracao, estadoEntidade, validador)
    {
    }

    public void AdicionarEventoDominio(IDomainEvent evento)
        => _eventosDominio.Add(evento);

    public void LimparEventosDominio()
        => _eventosDominio.Clear();
}
