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
    {
        var horarios = modelo.Horarios.Select(SlotHorarioMapper.ParaEntidade).ToList();
        var procedimentos = modelo.Procedimentos.Select(ProcedimentoMapper.ParaEntidade).ToList();

        var proprietarioBase = new ProprietarioEntidade(
            modelo.Id,
            modelo.DataCriacao,
            modelo.DataAlteracao,
            modelo.EstadoEntidade,
            modelo.Nome,
            horarios,
            [],
            procedimentos);

        var atendimentos = modelo.Atendimentos
            .Select(atendimento => AtendimentoMapper.ParaEntidade(atendimento, proprietarioBase))
            .ToList();

        return new ProprietarioEntidade(
            modelo.Id,
            modelo.DataCriacao,
            modelo.DataAlteracao,
            modelo.EstadoEntidade,
            modelo.Nome,
            horarios,
            atendimentos,
            procedimentos);
    }
}
