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

    protected ServicoValidacaoBase()
    {
        _validacoes = BuscarValidacoes();
    }
    public virtual void Validar(TEntity entidade)
        => _validacoes.ToList().ForEach(validacao => validacao.Valido(entidade));

    private IEnumerable<IValidadorEspecificacao<TEntity>> BuscarValidacoes()
    {
        // Nome da interface que queremos buscar
        var interfaceType = typeof(IValidadorEspecificacao<TEntity>);

        // Obter o assembly atual (pode ser ajustado para um assembly específico)
        var assembly = Assembly.GetExecutingAssembly();

        // Encontrar todas as classes que implementam a interface
        var implementacoes = assembly.GetTypes()
            .Where(t => interfaceType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

        foreach (var tipo in implementacoes)
            yield return (IValidadorEspecificacao<TEntity>)Activator.CreateInstance(tipo);
    }
}
