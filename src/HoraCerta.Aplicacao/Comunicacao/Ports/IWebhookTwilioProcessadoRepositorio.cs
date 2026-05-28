namespace HoraCerta.Aplicacao.Comunicacao.Ports;

public interface IWebhookTwilioProcessadoRepositorio
{
    bool JaProcessado(string messageSid);

    void MarcarProcessado(string messageSid);
}
