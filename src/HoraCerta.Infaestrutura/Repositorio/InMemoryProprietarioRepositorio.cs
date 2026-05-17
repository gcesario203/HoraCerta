using HoraCerta.Dominio;
using HoraCerta.Dominio.Proprietario;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Infaestrutura.Repositorio;

public class InMemoryProprietarioRepositorio : IProprietarioRepositorio
{
    private readonly Dictionary<string, ProprietarioEntidade> _armazenamento = new();

    public ProprietarioEntidade? BuscarPorId(IdEntidade id)
        => _armazenamento.GetValueOrDefault(id.Valor);

    public Task<ProprietarioEntidade?> BuscarPorIdAsync(IdEntidade id)
        => Task.FromResult(BuscarPorId(id));

    public int Criar(ProprietarioEntidade entidade)
    {
        Salvar(entidade);
        return 1;
    }

    public Task<int> CriarAsync(ProprietarioEntidade entidade)
    {
        Salvar(entidade);
        return Task.FromResult(1);
    }

    public bool Editar(IdEntidade id, ProprietarioEntidade entidade)
    {
        Salvar(entidade);
        return true;
    }

    public Task<bool> EditarAsync(IdEntidade id, ProprietarioEntidade entidade)
    {
        Salvar(entidade);
        return Task.FromResult(true);
    }

    public bool Deletar(IdEntidade id)
    {
        return _armazenamento.Remove(id.Valor);
    }

    public Task DeletarAsync(IdEntidade id)
    {
        _armazenamento.Remove(id.Valor);
        return Task.CompletedTask;
    }

    public void Salvar(ProprietarioEntidade entidade)
        => _armazenamento[entidade.Id.Valor] = entidade;

    public IReadOnlyList<ProprietarioEntidade> ListarTodos()
        => _armazenamento.Values.OrderBy(p => p.Nome, StringComparer.OrdinalIgnoreCase).ToList();
}
