using System.Text.Json;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;
using HoraCerta.Infaestrutura.Mapeamento;
using HoraCerta.Infaestrutura.Persistencia;
using HoraCerta.Infaestrutura.Persistencia.Modelos;
using HoraCerta.Infaestrutura.Persistencia.Registros;
using HoraCerta.Infaestrutura.Persistencia.Serializacao;

namespace HoraCerta.Infaestrutura.Repositorio;

public class EfClienteRepositorio : IClienteRepositorio
{
    private readonly HoraCertaDbContext _context;
    private readonly JsonSerializerOptions _jsonOptions = PersistenciaJsonOptions.Criar();

    public EfClienteRepositorio(HoraCertaDbContext context)
    {
        _context = context;
    }

    public ClienteEntidade? BuscarPorId(IdEntidade id)
    {
        var registro = _context.Clientes.Find(id.Valor);
        return registro is null ? null : Deserializar(registro);
    }

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
        var registro = _context.Clientes.Find(id.Valor);
        if (registro is null)
            return false;

        _context.Clientes.Remove(registro);
        _context.SaveChanges();
        return true;
    }

    public Task DeletarAsync(IdEntidade id)
    {
        Deletar(id);
        return Task.CompletedTask;
    }

    public void Salvar(ClienteEntidade entidade)
    {
        var modelo = ClienteMapper.ParaModelo(entidade);
        var conteudo = JsonSerializer.Serialize(modelo, _jsonOptions);
        var registro = _context.Clientes.Find(entidade.Id.Valor);

        if (registro is null)
        {
            _context.Clientes.Add(new ClienteRegistro
            {
                Id = entidade.Id.Valor,
                Conteudo = conteudo
            });
        }
        else
        {
            registro.Conteudo = conteudo;
        }

        _context.SaveChanges();
    }

    private ClienteEntidade Deserializar(ClienteRegistro registro)
    {
        var modelo = JsonSerializer.Deserialize<ClienteModelo>(registro.Conteudo, _jsonOptions)
            ?? throw new InvalidOperationException("Conteúdo do cliente inválido");

        return ClienteMapper.ParaEntidade(modelo);
    }
}
