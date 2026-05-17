using HoraCerta.Aplicacao.Autenticacao.Commands;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Proprietario;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Autenticacao.Handlers;

public class RegistrarCredencialHandler
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;
    private readonly ICredencialProprietarioRepositorio _credencialRepositorio;
    private readonly Func<string, string> _hashSenha;

    public RegistrarCredencialHandler(
        IProprietarioRepositorio proprietarioRepositorio,
        ICredencialProprietarioRepositorio credencialRepositorio,
        Func<string, string> hashSenha)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
        _credencialRepositorio = credencialRepositorio;
        _hashSenha = hashSenha;
    }

    public string Executar(RegistrarCredencialCommand command)
    {
        ProprietarioEntidade proprietario;

        if (!string.IsNullOrWhiteSpace(command.ProprietarioId))
        {
            proprietario = _proprietarioRepositorio.BuscarPorId(new IdEntidade(command.ProprietarioId))
                ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");
        }
        else if (!string.IsNullOrWhiteSpace(command.NomeEstabelecimento))
        {
            proprietario = new ProprietarioEntidade(command.NomeEstabelecimento);
            _proprietarioRepositorio.Salvar(proprietario);
        }
        else
        {
            throw new OperacaoInvalidaExcessao("Informe proprietarioId ou nomeEstabelecimento");
        }

        if (_credencialRepositorio.Existe(proprietario.Id.Valor))
            throw new OperacaoInvalidaExcessao("Credenciais já registradas para este estabelecimento");

        _credencialRepositorio.Salvar(
            proprietario.Id.Valor,
            command.Email,
            _hashSenha(command.Senha));

        return proprietario.Id.Valor;
    }
}
