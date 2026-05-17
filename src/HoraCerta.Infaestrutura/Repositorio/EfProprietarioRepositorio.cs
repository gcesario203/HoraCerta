using System.Text.Json;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Proprietario;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;
using HoraCerta.Infaestrutura.Mapeamento;
using HoraCerta.Infaestrutura.Persistencia;
using HoraCerta.Infaestrutura.Persistencia.Modelos;
using HoraCerta.Infaestrutura.Persistencia.Registros;
using HoraCerta.Infaestrutura.Persistencia.Serializacao;

namespace HoraCerta.Infaestrutura.Repositorio;

public class EfProprietarioRepositorio : IProprietarioRepositorio
{
    private readonly HoraCertaDbContext _context;
    private readonly JsonSerializerOptions _jsonOptions = PersistenciaJsonOptions.Criar();

    public EfProprietarioRepositorio(HoraCertaDbContext context)
    {
        _context = context;
    }

    public ProprietarioEntidade? BuscarPorId(IdEntidade id)
    {
        var registro = _context.Proprietarios.Find(id.Valor);
        return registro is null ? null : Deserializar(registro);
    }

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
        var registro = _context.Proprietarios.Find(id.Valor);
        if (registro is null)
            return false;

        _context.Proprietarios.Remove(registro);
        _context.SaveChanges();
        return true;
    }

    public Task DeletarAsync(IdEntidade id)
    {
        Deletar(id);
        return Task.CompletedTask;
    }

    public IReadOnlyList<ProprietarioEntidade> ListarTodos()
    {
        var lista = new List<ProprietarioEntidade>();

        foreach (var registro in _context.Proprietarios)
        {
            try
            {
                lista.Add(Deserializar(registro));
            }
            catch (InvalidOperationException)
            {
                // ignora registros corrompidos
            }
        }

        return lista.OrderBy(p => p.Nome, StringComparer.OrdinalIgnoreCase).ToList();
    }

    public void Salvar(ProprietarioEntidade entidade)
    {
        var modelo = ProprietarioMapper.ParaModelo(entidade);
        var conteudo = JsonSerializer.Serialize(modelo, _jsonOptions);
        var registro = _context.Proprietarios.Find(entidade.Id.Valor);

        if (registro is null)
        {
            _context.Proprietarios.Add(new ProprietarioRegistro
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

    private ProprietarioEntidade Deserializar(ProprietarioRegistro registro)
    {
        var modelo = JsonSerializer.Deserialize<ProprietarioModelo>(registro.Conteudo, _jsonOptions)
            ?? throw new InvalidOperationException("Conteúdo do proprietário inválido");

        return ProprietarioMapper.ParaEntidade(modelo);
    }
}
