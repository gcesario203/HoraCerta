using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao.Agendamento.Queries;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Agendamento.Handlers;

public class ObterAvaliacaoAgendamentoHandler : IQueryHandler<ObterAvaliacaoAgendamentoQuery, AvaliacaoEntidade?>
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;
    private readonly IClienteRepositorio _clienteRepositorio;

    public ObterAvaliacaoAgendamentoHandler(
        IProprietarioRepositorio proprietarioRepositorio,
        IClienteRepositorio clienteRepositorio)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
        _clienteRepositorio = clienteRepositorio;
    }

    public AvaliacaoEntidade? Executar(ObterAvaliacaoAgendamentoQuery query)
    {
        var proprietario = _proprietarioRepositorio.BuscarPorId(query.ProprietarioId)
            ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");

        foreach (var cliente in _clienteRepositorio.ListarComProprietario(proprietario))
        {
            var avaliacao = cliente.GerenciadorAgendamentos.Avaliacoes
                .FirstOrDefault(a =>
                    a.AgendamentoId.Valor == query.AgendamentoId.Valor &&
                    a.ProprietarioId.Valor == query.ProprietarioId.Valor);

            if (avaliacao is not null)
                return avaliacao;
        }

        return null;
    }
}
