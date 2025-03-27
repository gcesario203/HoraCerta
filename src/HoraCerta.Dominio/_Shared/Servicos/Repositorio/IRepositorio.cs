namespace HoraCerta.Dominio._Shared;

public interface IRepositorio<TEntidade> where TEntidade : EntidadeBase<TEntidade>
{
    TEntidade BuscarPorId(IdEntidade id);

    Task<TEntidade> BuscarPorIdAsync(IdEntidade id);

    Task<int> CriarAsync(TEntidade entidade);

    Task<bool> EditarAsync(IdEntidade id, TEntidade entidade);

    int Criar(TEntidade entidade);

    bool Editar(IdEntidade id, TEntidade entidade);

    bool Deletar(IdEntidade id);

    Task DeletarAsync(IdEntidade id);
}
