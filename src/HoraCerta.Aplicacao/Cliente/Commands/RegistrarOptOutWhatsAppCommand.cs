using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Cliente.Commands;

public record RegistrarOptOutWhatsAppCommand(IdEntidade ProprietarioId, string Telefone);
