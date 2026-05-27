using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Agendamento.Queries;

public record ListarAgendamentosClienteQuery(IdEntidade ClienteId, IdEntidade? ProprietarioId = null);
