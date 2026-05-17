using HoraCerta.Aplicacao.Autenticacao.Commands;
using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Autenticacao.Handlers;

public class LoginHandler
{
    private readonly ICredencialProprietarioRepositorio _credencialRepositorio;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly Func<string, string, bool> _verificarSenha;

    public LoginHandler(
        ICredencialProprietarioRepositorio credencialRepositorio,
        IJwtTokenService jwtTokenService,
        Func<string, string, bool> verificarSenha)
    {
        _credencialRepositorio = credencialRepositorio;
        _jwtTokenService = jwtTokenService;
        _verificarSenha = verificarSenha;
    }

    public (string Token, string ProprietarioId) Executar(LoginCommand command)
    {
        var credencial = _credencialRepositorio.BuscarPorEmail(command.Email)
            ?? throw new OperacaoInvalidaExcessao("Credenciais inválidas");

        if (!_verificarSenha(command.Senha, credencial.PasswordHash))
            throw new OperacaoInvalidaExcessao("Credenciais inválidas");

        var token = _jwtTokenService.GerarToken(credencial.ProprietarioId, credencial.Email);

        return (token, credencial.ProprietarioId);
    }
}
