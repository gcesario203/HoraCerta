using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Estabelecimento.Commands;

public record InativarProcedimentoCommand(
    IdEntidade ProprietarioId,
    IdEntidade ProcedimentoId);
