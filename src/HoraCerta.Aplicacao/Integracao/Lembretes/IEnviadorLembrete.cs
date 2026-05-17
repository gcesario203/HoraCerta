namespace HoraCerta.Aplicacao.Integracao.Lembretes;

public interface IEnviadorLembrete
{
    Task EnviarAsync(LembretePendente lembrete, CancellationToken cancellationToken = default);
}
