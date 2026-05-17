using HoraCerta.Dominio;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;

namespace HoraCerta.Infaestrutura.Repositorio;

public class InMemoryClienteRepositorio : IClienteRepositorio
{
    private readonly Dictionary<string, ClienteEntidade> _armazenamento = new();

    public ClienteEntidade? BuscarPorId(IdEntidade id)
        => _armazenamento.GetValueOrDefault(id.Valor);

    public Task<ClienteEntidade?> BuscarPorIdAsync(IdEntidade id)
        => Task.FromResult(BuscarPorId(id));

    public int Criar(ClienteEntidade entidade)
    {
        Salvar(entidade);
        return 1;
    }

    public Task<int> CriarAsync(ClienteEntidade entidade)
    {
        Salvar(entidade);
        return Task.FromResult(1);
    }

    public bool Editar(IdEntidade id, ClienteEntidade entidade)
    {
        Salvar(entidade);
        return true;
    }

    public Task<bool> EditarAsync(IdEntidade id, ClienteEntidade entidade)
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

    public void Salvar(ClienteEntidade entidade)
        => _armazenamento[entidade.Id.Valor] = entidade;
}
