using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao.Agendamento.Queries;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Agendamento.Handlers;

public class ListarAgendamentosClienteHandler
    : IQueryHandler<ListarAgendamentosClienteQuery, ICollection<AgendamentoClienteListagemDto>>
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IProprietarioRepositorio _proprietarioRepositorio;

    public ListarAgendamentosClienteHandler(
        IClienteRepositorio clienteRepositorio,
        IProprietarioRepositorio proprietarioRepositorio)
    {
        _clienteRepositorio = clienteRepositorio;
        _proprietarioRepositorio = proprietarioRepositorio;
    }

    public ICollection<AgendamentoClienteListagemDto> Executar(ListarAgendamentosClienteQuery query)
    {
        var cliente = _clienteRepositorio.BuscarPorId(query.ClienteId)
            ?? throw new OperacaoInvalidaExcessao("Cliente não encontrado");

        HashSet<string>? slotIdsProprietario = null;
        if (query.ProprietarioId is not null)
        {
            var proprietario = _proprietarioRepositorio.BuscarPorId(query.ProprietarioId)
                ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");
            slotIdsProprietario = proprietario.Horarios.Select(h => h.Id.Valor).ToHashSet();
        }

        return cliente.GerenciadorAgendamentos.BuscarAgendamentos()
            .Where(a =>
            {
                if (slotIdsProprietario is null) return true;
                var slotId = a.SlotHorario?.Id.Valor;
                return slotId is not null && slotIdsProprietario.Contains(slotId);
            })
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
