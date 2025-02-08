using HoraCerta.Dominio._Shared.Interfaces;
using HoraCerta.Dominio.Shared.Abstracoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio._Shared.Abstracoes;

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
