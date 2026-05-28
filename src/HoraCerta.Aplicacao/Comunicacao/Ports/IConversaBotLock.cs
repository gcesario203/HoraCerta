namespace HoraCerta.Aplicacao.Comunicacao.Ports;

public interface IConversaBotLock
{
    Task<IDisposable> AdquirirAsync(string telefone, string proprietarioId, CancellationToken cancellationToken = default);
}
