using HoraCerta.Dominio._Shared;
using HoraCerta.Dominio.Proprietario;

namespace HoraCerta.Dominio.Cliente.Servicos.Repositorio;

public interface IClienteRepositorio : IRepositorio<ClienteEntidade>
{
  /// <summary>
  /// Reidrata o agregado cliente vinculando slots e procedimentos ao estabelecimento informado.
  /// </summary>
  ClienteEntidade? BuscarPorId(IdEntidade clienteId, ProprietarioEntidade proprietario);

  Task<ClienteEntidade?> BuscarPorIdAsync(IdEntidade clienteId, ProprietarioEntidade proprietario);

  IReadOnlyList<ClienteEntidade> ListarComProprietario(ProprietarioEntidade proprietario);
}
