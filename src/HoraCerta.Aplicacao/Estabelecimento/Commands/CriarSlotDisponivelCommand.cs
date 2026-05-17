using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Estabelecimento.Commands;

public record CriarSlotDisponivelCommand(
    IdEntidade ProprietarioId,
    DateTime InicioDoHorario);
