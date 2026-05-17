namespace HoraCerta.Aplicacao._Shared.Interfaces;

public interface ICommandHandler<in TCommand, TResult>
{
    TResult Executar(TCommand command);
}
