using HoraCerta.Dominio.Proprietario;
using HoraCerta.Infaestrutura.Persistencia.Modelos;

namespace HoraCerta.Infaestrutura.Mapeamento;

public static class ProprietarioMapper
{
    public static ProprietarioModelo ParaModelo(ProprietarioEntidade entidade)
        => new()
        {
            Id = entidade.Id.Valor,
            DataCriacao = entidade.DataCriacao,
            DataAlteracao = entidade.DataAlteracao,
            EstadoEntidade = entidade.EstadoEntidade,
            Nome = entidade.Nome,
            Horarios = entidade.Horarios.Select(SlotHorarioMapper.ParaModelo).ToList(),
            Atendimentos = entidade.Atendimentos.Select(AtendimentoMapper.ParaModelo).ToList(),
            Procedimentos = entidade.GerenciadorProcedimentos.RecuperarProcedimentos()
                .Select(ProcedimentoMapper.ParaModelo)
                .ToList()
        };

    public static ProprietarioEntidade ParaEntidade(ProprietarioModelo modelo)
        => new(
            modelo.Id,
            modelo.DataCriacao,
            modelo.DataAlteracao,
            modelo.EstadoEntidade,
            modelo.Nome,
            modelo.Horarios.Select(SlotHorarioMapper.ParaEntidade).ToList(),
            modelo.Atendimentos.Select(AtendimentoMapper.ParaEntidade).ToList(),
            modelo.Procedimentos.Select(ProcedimentoMapper.ParaEntidade).ToList());
}
