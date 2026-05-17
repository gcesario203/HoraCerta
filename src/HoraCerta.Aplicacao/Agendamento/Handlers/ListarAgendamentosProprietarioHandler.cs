using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao.Agendamento.Queries;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Agendamento.Handlers;

public class ListarAgendamentosProprietarioHandler
    : IQueryHandler<ListarAgendamentosProprietarioQuery, ICollection<AgendamentoListagemDto>>
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;
    private readonly IClienteRepositorio _clienteRepositorio;

    public ListarAgendamentosProprietarioHandler(
        IProprietarioRepositorio proprietarioRepositorio,
        IClienteRepositorio clienteRepositorio)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
        _clienteRepositorio = clienteRepositorio;
    }

    public ICollection<AgendamentoListagemDto> Executar(ListarAgendamentosProprietarioQuery query)
    {
        var proprietario = _proprietarioRepositorio.BuscarPorId(query.ProprietarioId)
            ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");

        var slotIds = proprietario.Horarios.Select(h => h.Id.Valor).ToHashSet();
        var resultado = new List<AgendamentoListagemDto>();

        foreach (var cliente in _clienteRepositorio.ListarComProprietario(proprietario))
        {
            foreach (var agendamento in cliente.GerenciadorAgendamentos.BuscarAgendamentos())
            {
                var slotId = agendamento.SlotHorario?.Id.Valor;
                if (slotId is null || !slotIds.Contains(slotId))
                    continue;

                resultado.Add(new AgendamentoListagemDto(
                    agendamento.Id.Valor,
                    cliente.Id.Valor,
                    cliente.Nome,
                    agendamento.Procedimento.Nome,
                    agendamento.SlotHorario!.Inicio,
                    agendamento.EstadoAtual().ToString()));
            }
        }

        return resultado;
    }
}

public record AgendamentoListagemDto(
    string AgendamentoId,
    string ClienteId,
    string ClienteNome,
    string ProcedimentoNome,
    DateTime SlotInicio,
    string Estado);
