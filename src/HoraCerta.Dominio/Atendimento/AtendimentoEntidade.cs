using HoraCerta.Dominio.Agendamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        _validador.Validar(this);
    }

    public void AlterarEstado(EstadoAtendimento novoEstado)
    {
        Estado = Estado.AlterarEstado(this,novoEstado);

        Atualizar();
    }

    public void AlterarValorNegociado(decimal valorNegociado)
    {
        if (EstadoAtual() == EstadoAtendimento.REALIZADO || EstadoAtual() == EstadoAtendimento.CANCELADO)
            throw new OperacaoInvalidaExcessao("Não é possível alterar o valor negociado de um atendimento finalizado");

        ValorNegociado = valorNegociado;

        _validador.Validar(this);

        Atualizar();
    }

    public EstadoAtendimento EstadoAtual()
        => Estado.EstadoAtual();

}
