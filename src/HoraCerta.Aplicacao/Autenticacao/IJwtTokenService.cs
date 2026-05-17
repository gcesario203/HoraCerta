namespace HoraCerta.Aplicacao.Autenticacao;

public interface IJwtTokenService
{
    string GerarToken(string proprietarioId, string email);
}
