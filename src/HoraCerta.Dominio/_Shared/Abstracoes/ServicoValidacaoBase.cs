namespace HoraCerta.Dominio;

public abstract class ServicoValidacaoBase<TEntity> : IServicoValidacao<TEntity> where TEntity : EntidadeBase<TEntity>
{
    private readonly IEnumerable<IValidadorEspecificacao<TEntity>> _validacoes;

    protected ServicoValidacaoBase(IEnumerable<IValidadorEspecificacao<TEntity>> validacoes)
    {
        _validacoes = validacoes;
    }
    public virtual void Validar(TEntity entidade)
        => _validacoes.ToList().ForEach(validacao => validacao.Valido(entidade));
}
