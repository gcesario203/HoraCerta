using HoraCerta.Dominio._Shared;

namespace HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

public interface IProprietarioRepositorio : IRepositorio<ProprietarioEntidade>
{
    IReadOnlyList<ProprietarioEntidade> ListarTodos();
}
