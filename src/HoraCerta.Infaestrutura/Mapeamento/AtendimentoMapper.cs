using HoraCerta.Dominio.Atendimento;
using HoraCerta.Infaestrutura.Persistencia.Modelos;

namespace HoraCerta.Infaestrutura.Mapeamento;

public static class AtendimentoMapper
{
    public static AtendimentoModelo ParaModelo(AtendimentoEntidade entidade)
        => new()
        {
            Id = entidade.Id.Valor,
            DataCriacao = entidade.DataCriacao,
            DataAlteracao = entidade.DataAlteracao,
            EstadoEntidade = entidade.EstadoEntidade,
            Origem = AgendamentoMapper.ParaModelo(entidade.Origem),
            ValorNegociado = entidade.ValorNegociado,
            Estado = entidade.EstadoAtual()
        };

    public static AtendimentoEntidade ParaEntidade(AtendimentoModelo modelo)
        => new(
            modelo.Id,
            modelo.DataCriacao,
            modelo.DataAlteracao,
            modelo.EstadoEntidade,
            AgendamentoMapper.ParaEntidade(modelo.Origem),
            modelo.ValorNegociado,
            modelo.Estado);
}
