namespace HoraCerta.Api.Contratos;

public record CriarProprietarioRequisicao(string Nome);

public record CriarClienteRequisicao(string Nome, string Telefone);

public record CriarProcedimentoRequisicao(string Nome, decimal Valor, int TempoEstimadoMinutos);

public record CriarSlotRequisicao(DateTime Inicio);

public record IniciarAgendamentoRequisicao(
    string ProprietarioId,
    string ClienteId,
    string ProcedimentoId,
    string SlotHorarioId);

public record ConfirmarAgendamentoRequisicao(string ProprietarioId, string ClienteId);

public record CancelarAgendamentoRequisicao(string ProprietarioId, string ClienteId);

public record RemarcarAgendamentoRequisicao(
    string ProprietarioId,
    string ClienteId,
    string NovoSlotHorarioId);

public record RegistrarAtendimentoRequisicao(
    string ProprietarioId,
    string ClienteId,
    decimal? ValorNegociado);

public record RegistrarCredencialRequisicao(
    string? ProprietarioId,
    string? NomeEstabelecimento,
    string Email,
    string Senha);

public record LoginRequisicao(string Email, string Senha);

public record AvaliarAgendamentoRequisicao(
    string ProprietarioId,
    int Nota,
    string? Comentario);

public record AlterarEstadoAtendimentoRequisicao(string Estado);
