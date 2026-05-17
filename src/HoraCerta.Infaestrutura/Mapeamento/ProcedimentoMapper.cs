using HoraCerta.Dominio.Procedimento;
using HoraCerta.Infaestrutura.Persistencia.Modelos;

namespace HoraCerta.Infaestrutura.Mapeamento;

public static class ProcedimentoMapper
{
    public static ProcedimentoModelo ParaModelo(ProcedimentoEntidade entidade)
        => new()
        {
            Id = entidade.Id.Valor,
            DataCriacao = entidade.DataCriacao,
            DataAlteracao = entidade.DataAlteracao,
            EstadoEntidade = entidade.EstadoEntidade,
            Nome = entidade.Nome,
            Valor = entidade.Valor,
            TempoEstimado = entidade.TempoEstimado
        };

    public static ProcedimentoEntidade ParaEntidade(ProcedimentoModelo modelo)
        => new(
            modelo.Id,
            modelo.DataCriacao,
            modelo.DataAlteracao,
            modelo.EstadoEntidade,
            modelo.Nome,
            modelo.Valor,
            modelo.TempoEstimado);
}
