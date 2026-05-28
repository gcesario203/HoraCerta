namespace HoraCerta.Aplicacao.Comunicacao.Bot;

public interface IOrquestradorBotAgendamento
{
    Task<string> ProcessarMensagemAsync(
        string telefone,
        string proprietarioId,
        string texto,
        CancellationToken cancellationToken = default);
}
