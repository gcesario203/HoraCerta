using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao.Agendamento.Queries;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Agendamento.Handlers;

public class ListarAgendamentosClienteHandler : IQueryHandler<ListarAgendamentosClienteQuery, ICollection<AgendamentoEntidade>>
{
    private readonly IClienteRepositorio _clienteRepositorio;

    public ListarAgendamentosClienteHandler(IClienteRepositorio clienteRepositorio)
    {
        _clienteRepositorio = clienteRepositorio;
    }

    public ICollection<AgendamentoEntidade> Executar(ListarAgendamentosClienteQuery query)
    {
        var cliente = _clienteRepositorio.BuscarPorId(query.ClienteId)
            ?? throw new OperacaoInvalidaExcessao("Cliente não encontrado");

        return cliente.GerenciadorAgendamentos.BuscarAgendamentos();
    }
}
