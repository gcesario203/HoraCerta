using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao.Cliente.Queries;
using HoraCerta.Aplicacao.Comunicacao.Ports;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Cliente.Handlers;

public class BuscarClientePorTelefoneHandler : IQueryHandler<BuscarClientePorTelefoneQuery, ClienteEntidade?>
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly INormalizadorTelefone _normalizador;

    public BuscarClientePorTelefoneHandler(
        IProprietarioRepositorio proprietarioRepositorio,
        IClienteRepositorio clienteRepositorio,
        INormalizadorTelefone normalizador)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
        _clienteRepositorio = clienteRepositorio;
        _normalizador = normalizador;
    }

    public ClienteEntidade? Executar(BuscarClientePorTelefoneQuery query)
    {
        var proprietario = _proprietarioRepositorio.BuscarPorId(query.ProprietarioId)
            ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");

        var telefone = _normalizador.Normalizar(query.Telefone);

        return _clienteRepositorio
            .ListarComProprietario(proprietario)
            .FirstOrDefault(c => _normalizador.SaoEquivalentes(c.Telefone, telefone));
    }
}
