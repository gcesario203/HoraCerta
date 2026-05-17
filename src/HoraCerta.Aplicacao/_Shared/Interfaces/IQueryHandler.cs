namespace HoraCerta.Aplicacao._Shared.Interfaces;

public interface IQueryHandler<in TQuery, TResult>
{
    TResult Executar(TQuery query);
}
