using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Estabelecimento.Commands;

public record RegistrarAtendimentoCommand(
    IdEntidade ProprietarioId,
    IdEntidade ClienteId,
    IdEntidade AgendamentoId,
    decimal? ValorNegociado = null);
