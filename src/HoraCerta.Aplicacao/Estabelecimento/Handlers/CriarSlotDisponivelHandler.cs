using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao._Shared.Persistencia;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Estabelecimento.Handlers;

public class CriarSlotDisponivelHandler : ICommandHandler<CriarSlotDisponivelCommand, SlotHorarioEntidade>
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;
    private readonly IDomainEventDispatcher _dispatcher;

    public CriarSlotDisponivelHandler(
        IProprietarioRepositorio proprietarioRepositorio,
        IDomainEventDispatcher dispatcher)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
        _dispatcher = dispatcher;
    }

    public SlotHorarioEntidade Executar(CriarSlotDisponivelCommand command)
    {
        var proprietario = _proprietarioRepositorio.BuscarPorId(command.ProprietarioId)
            ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");

        proprietario.GerenciadorAgenda.CriarHorarioDisponivel(command.InicioDoHorario);

        var slot = proprietario.Horarios.Last();

        UnidadeTrabalhoDominio.SalvarEDispararEventos(
            p => _proprietarioRepositorio.Salvar(p),
            proprietario,
            _dispatcher);

        return slot;
    }
}
