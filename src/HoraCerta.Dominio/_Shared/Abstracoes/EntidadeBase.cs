using HoraCerta.Dominio._Shared.Abstracoes;
using HoraCerta.Dominio._Shared.Interfaces;
using HoraCerta.Dominio.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.@Shared.Abstracoes;

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
