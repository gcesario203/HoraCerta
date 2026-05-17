using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao.Agendamento.Queries;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Agendamento.Handlers;

public class ListarAgendamentosClienteHandler
    : IQueryHandler<ListarAgendamentosClienteQuery, ICollection<AgendamentoClienteListagemDto>>
{
    private readonly IClienteRepositorio _clienteRepositorio;

    public ListarAgendamentosClienteHandler(IClienteRepositorio clienteRepositorio)
    {
        _clienteRepositorio = clienteRepositorio;
    }

    public ICollection<AgendamentoClienteListagemDto> Executar(ListarAgendamentosClienteQuery query)
    {
        var cliente = _clienteRepositorio.BuscarPorId(query.ClienteId)
            ?? throw new OperacaoInvalidaExcessao("Cliente não encontrado");

        return cliente.GerenciadorAgendamentos.BuscarAgendamentos()
            .Select(a => new AgendamentoClienteListagemDto(
                a.Id.Valor,
                a.Procedimento.Nome,
                a.SlotHorario?.Inicio,
                a.EstadoAtual().ToString()))
            .ToList();
    }
}

public record AgendamentoClienteListagemDto(
    string AgendamentoId,
    string ProcedimentoNome,
    DateTime? SlotInicio,
    string Estado);
