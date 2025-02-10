using HoraCerta.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio;

public interface IValidadorEspecificacao<TEntity> where TEntity : EntidadeBase<TEntity>
{
    void Valido(TEntity entidade);
}
