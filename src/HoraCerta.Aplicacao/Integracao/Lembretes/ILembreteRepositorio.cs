namespace HoraCerta.Aplicacao.Integracao.Lembretes;

public interface ILembreteRepositorio
{
    void Agendar(
        string proprietarioId,
        string clienteId,
        string agendamentoId,
        string telefoneCliente,
        DateTime slotInicio,
        DateTime enviarEm);

    void CancelarPorAgendamento(string agendamentoId);

    void Reagendar(
        string agendamentoAnteriorId,
        string novoAgendamentoId,
        DateTime novoSlotInicio,
        DateTime enviarEm);

    IReadOnlyList<LembretePendente> BuscarPendentesParaEnvio(DateTime ate);

    void MarcarEnviado(string id);
}
