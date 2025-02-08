using HoraCerta.Dominio.Shared.Abstracoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio._Shared.Interfaces;

public interface IValidadorEspecificacao<TEntity> where TEntity : EntidadeBase<TEntity>
{
    void Valido(TEntity entidade);
}
