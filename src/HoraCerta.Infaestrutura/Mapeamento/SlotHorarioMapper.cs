using HoraCerta.Dominio;
using HoraCerta.Infaestrutura.Persistencia.Modelos;

namespace HoraCerta.Infaestrutura.Mapeamento;

public static class SlotHorarioMapper
{
    public static SlotHorarioModelo ParaModelo(SlotHorarioEntidade entidade)
        => new()
        {
            Id = entidade.Id.Valor,
            DataCriacao = entidade.DataCriacao,
            DataAlteracao = entidade.DataAlteracao,
            EstadoEntidade = entidade.EstadoEntidade,
            Inicio = entidade.Inicio,
            Fim = entidade.Fim,
            Status = entidade.Status
        };

    public static SlotHorarioEntidade ParaEntidade(SlotHorarioModelo modelo)
        => new(
            modelo.Id,
            modelo.DataCriacao,
            modelo.DataAlteracao,
            modelo.EstadoEntidade,
            modelo.Inicio,
            modelo.Fim,
            modelo.Status);
}
