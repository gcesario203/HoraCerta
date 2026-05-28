using System.Collections.Concurrent;
using HoraCerta.Aplicacao.Comunicacao.Ports;

namespace HoraCerta.Infaestrutura.Comunicacao;

public class ConversaBotLock : IConversaBotLock
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public async Task<IDisposable> AdquirirAsync(
        string telefone,
        string proprietarioId,
        CancellationToken cancellationToken = default)
    {
        var chave = $"{telefone}:{proprietarioId}";
        var semaforo = _locks.GetOrAdd(chave, _ => new SemaphoreSlim(1, 1));
        await semaforo.WaitAsync(cancellationToken);
        return new LockHandle(semaforo);
    }

    private sealed class LockHandle(SemaphoreSlim semaforo) : IDisposable
    {
        public void Dispose() => semaforo.Release();
    }
}
