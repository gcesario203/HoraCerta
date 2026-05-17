using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Agendamento.Queries;

public record ObterAvaliacaoAgendamentoQuery(
    IdEntidade ProprietarioId,
    IdEntidade AgendamentoId);
