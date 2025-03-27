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

    private ProcedimentoEntidade(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade, string nome, decimal valor, TimeSpan tempoEstimado)
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

    public static ProcedimentoDTO ParaDTO(ProcedimentoEntidade entidade)
    {
        return new ProcedimentoDTO(entidade.Id.Valor, entidade.DataCriacao, entidade.DataAlteracao, entidade.EstadoEntidade, entidade.Nome, entidade.Valor, entidade.TempoEstimado);
    }

    public static ProcedimentoEntidade ParaEntidade(ProcedimentoDTO dto)
    {
        return new ProcedimentoEntidade(dto.Id, dto.DataCriacao, dto.DataAlteracao, dto.EstadoEntidade, dto.Nome, dto.Valor, dto.TempoEstimado);
    }

}
