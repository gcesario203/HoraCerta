using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Agendamento;

namespace HoraCerta.Dominio.Atendimento;

public class AtendimentoEntidade : EntidadeBase<AtendimentoEntidade>
{
    public AgendamentoEntidade Origem { get; }

    public decimal ValorNegociado { get; private set; }

    public IEstadoAtendimento Estado { get; private set; }

    public AtendimentoEntidade(AgendamentoEntidade origem, decimal valorNegociado) : base(new ValidadorAtendimento())
    {
        Origem = origem;

        ValorNegociado = valorNegociado;

        Estado = new AtendimentoPendente();

        _validador!.Validar(this);
    }

    internal AtendimentoEntidade(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade, AgendamentoEntidade origem, decimal valorNegociado, EstadoAtendimento estadoAtendimento)
    :base(id, dataCriacao, dataAlteracao, estadoEntidade, new ValidadorAtendimento())
    {
        Origem = origem;

        ValorNegociado = valorNegociado;

        Estado = UtilidadesDeEstado.MontaObjetoDeEstado(estadoAtendimento);

        _validador!.Validar(this);
    }

    public void AlterarEstado(EstadoAtendimento novoEstado)
    {
        Estado = Estado.AlterarEstado(this, novoEstado);

        Atualizar();
    }

    public void AlterarValorNegociado(decimal valorNegociado)
    {
        if (EstadoAtual() == EstadoAtendimento.REALIZADO || EstadoAtual() == EstadoAtendimento.CANCELADO)
            throw new OperacaoInvalidaExcessao("Não é possível alterar o valor negociado de um atendimento finalizado");

        ValorNegociado = valorNegociado;

        _validador!.Validar(this);

        Atualizar();
    }

    public EstadoAtendimento EstadoAtual()
        => Estado.EstadoAtual();
}
