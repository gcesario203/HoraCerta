using HoraCerta.Aplicacao.Comunicacao.Dtos;

namespace HoraCerta.Aplicacao.Comunicacao.Ports;

public interface IMensagemOutboxRepositorio
{
    void Adicionar(MensagemOutboxPendente mensagem);

    bool ExistePorIdempotencyKey(string idempotencyKey);

    IReadOnlyList<MensagemOutboxPendente> ReservarPendentes(DateTime ate, int limite);

    void MarcarEnviado(string id);

    void RegistrarFalha(string id, string erro, DateTime proximaTentativaEm, int tentativas);

    void MarcarFalhaDefinitiva(string id, string erro);

    void CancelarPorAgendamento(string agendamentoId);
}
