using HoraCerta.Dominio;
using HoraCerta.Dominio.Atendimento;

namespace HoraCerta.Aplicacao.Estabelecimento.Commands;

public record AlterarEstadoAtendimentoCommand(
    IdEntidade ProprietarioId,
    IdEntidade AtendimentoId,
    EstadoAtendimento NovoEstado);
