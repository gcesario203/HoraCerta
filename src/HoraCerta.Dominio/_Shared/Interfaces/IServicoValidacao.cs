using HoraCerta.Dominio.Shared.Abstracoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio._Shared.Interfaces;

public interface IServicoValidacao<TEntity> where TEntity : EntidadeBase<TEntity>
{
    public void Validar(TEntity entidade);
}
