namespace HoraCerta.Aplicacao.Autenticacao.Commands;

public record RegistrarCredencialCommand(
    string? ProprietarioId,
    string? NomeEstabelecimento,
    string Email,
    string Senha);
