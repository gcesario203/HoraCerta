using HoraCerta.Dominio._Shared;
using HoraCerta.Dominio._Shared.Enums;

namespace HoraCerta.Dominio.Procedimento;

public class ProcedimentoDTO : DTOBase
{
    public string Nome { get; private set; }
    public decimal Valor { get; private set; }
    public TimeSpan TempoEstimado { get; private set; }

    public ProcedimentoDTO(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade, string nome, decimal valor, TimeSpan tempoEstimado)
    : base(id, dataCriacao, dataAlteracao, estadoEntidade)
    {
        Nome = nome;
        Valor = valor;
        TempoEstimado = tempoEstimado;
    }
}
