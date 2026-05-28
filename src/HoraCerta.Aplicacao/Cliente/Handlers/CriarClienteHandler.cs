using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao.Cliente.Commands;
using HoraCerta.Aplicacao.Comunicacao.Ports;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Cliente.Handlers;

public class CriarClienteHandler : ICommandHandler<CriarClienteCommand, ClienteEntidade>
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly INormalizadorTelefone _normalizador;

    public CriarClienteHandler(
        IClienteRepositorio clienteRepositorio,
        INormalizadorTelefone normalizador)
    {
        _clienteRepositorio = clienteRepositorio;
        _normalizador = normalizador;
    }

    public ClienteEntidade Executar(CriarClienteCommand command)
    {
        var telefone = _normalizador.Normalizar(command.Telefone);
        var cliente = new ClienteEntidade(command.Nome, telefone);
        _clienteRepositorio.Salvar(cliente);
        return cliente;
    }
}
