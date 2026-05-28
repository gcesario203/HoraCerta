namespace HoraCerta.Aplicacao.Comunicacao.Ports;

public interface IEnviadorWhatsApp
{
    Task EnviarAsync(string telefoneDestino, string corpo, CancellationToken cancellationToken = default);
}
