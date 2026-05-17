using HoraCerta.Dominio;
using HoraCerta.Dominio._Shared.Enums;

namespace HoraCerta.Dominio.Procedimento;

public class ProcedimentoEntidade : EntidadeBase<ProcedimentoEntidade>
{
    public string Nome { get; private set; }
    public decimal Valor { get; private set; }
    public TimeSpan TempoEstimado { get; private set; }
    public ProcedimentoEntidade(string nome, decimal valor, TimeSpan tempoEstimado) : base(new ValidadorProcedimento())
    {
        Nome = nome;
        Valor = valor;
        TempoEstimado = tempoEstimado;

        _validador!.Validar(this);
    }

    internal ProcedimentoEntidade(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade, string nome, decimal valor, TimeSpan tempoEstimado)
    : base(id, dataCriacao, dataAlteracao, estadoEntidade, new ValidadorProcedimento())
    {
        Nome = nome;
        Valor = valor;
        TempoEstimado = tempoEstimado;

        _validador!.Validar(this);
    }

    public void AtualizarNome(string nome)
    {
        Atualizar();

        Nome = nome;

        _validador!.Validar(this);
    }

    public void AtualizarValor(decimal valor)
    {
        Atualizar();

        Valor = valor;

        _validador!.Validar(this);
    }

    public void AtualizarTempoEstimado(TimeSpan tempoEstimado)
    {
        Atualizar();

        TempoEstimado = tempoEstimado;

        _validador!.Validar(this);
    }

}
