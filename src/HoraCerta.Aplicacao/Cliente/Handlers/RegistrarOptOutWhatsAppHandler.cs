using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao.Cliente.Commands;
using HoraCerta.Aplicacao.Cliente.Queries;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Cliente.Handlers;

public class RegistrarOptOutWhatsAppHandler : ICommandHandler<RegistrarOptOutWhatsAppCommand, ClienteEntidade?>
{
    private readonly BuscarClientePorTelefoneHandler _buscarHandler;
    private readonly IClienteRepositorio _clienteRepositorio;

    public RegistrarOptOutWhatsAppHandler(
        BuscarClientePorTelefoneHandler buscarHandler,
        IClienteRepositorio clienteRepositorio)
    {
        _buscarHandler = buscarHandler;
        _clienteRepositorio = clienteRepositorio;
    }

    public ClienteEntidade? Executar(RegistrarOptOutWhatsAppCommand command)
    {
        var cliente = _buscarHandler.Executar(
            new BuscarClientePorTelefoneQuery(command.ProprietarioId, command.Telefone));

        if (cliente is null)
            return null;

        cliente.RegistrarOptOutWhatsApp();
        _clienteRepositorio.Salvar(cliente);
        return cliente;
    }
}
