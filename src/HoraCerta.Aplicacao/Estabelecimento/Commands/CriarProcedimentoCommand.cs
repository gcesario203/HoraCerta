using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Estabelecimento.Commands;

public record CriarProcedimentoCommand(
    IdEntidade ProprietarioId,
    string Nome,
    decimal Valor,
    TimeSpan TempoEstimado);
