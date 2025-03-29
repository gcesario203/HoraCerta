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

    private AtendimentoEntidade(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade, AgendamentoEntidade origem, decimal valorNegociado, EstadoAtendimento estadoAtendimento)
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

    
    public static AtendimentoDTO ParaDTO(AtendimentoEntidade atendimento)
        => new AtendimentoDTO(atendimento.Id.Valor, atendimento.DataCriacao, atendimento.DataAlteracao, atendimento.EstadoEntidade, AgendamentoEntidade.ParaDTO(atendimento.Origem), atendimento.ValorNegociado, atendimento.EstadoAtual());

    public static AtendimentoEntidade ParaEntidade(AtendimentoDTO atendimento)
        => new AtendimentoEntidade(atendimento.Id, atendimento.DataCriacao, atendimento.DataAlteracao, atendimento.EstadoEntidade, AgendamentoEntidade.ParaEntidade(atendimento.Origem), atendimento.ValorNegociado, atendimento.Estado);
}
