using HoraCerta.Dominio.Agendamento;
using HoraCerta.Infaestrutura.Persistencia.Modelos;

namespace HoraCerta.Infaestrutura.Mapeamento;

public static class AgendamentoMapper
{
    public static AgendamentoModelo ParaModelo(AgendamentoEntidade entidade)
        => new()
        {
            Id = entidade.Id.Valor,
            DataCriacao = entidade.DataCriacao,
            DataAlteracao = entidade.DataAlteracao,
            EstadoEntidade = entidade.EstadoEntidade,
            SlotHorario = entidade.SlotHorario is null ? null : SlotHorarioMapper.ParaModelo(entidade.SlotHorario),
            Estado = entidade.EstadoAtual(),
            Reagendamento = entidade.Reagendamento is null ? null : ParaModelo(entidade.Reagendamento),
            Procedimento = ProcedimentoMapper.ParaModelo(entidade.Procedimento)
        };

    public static AgendamentoEntidade ParaEntidade(AgendamentoModelo modelo)
        => new(
            modelo.Id,
            modelo.DataCriacao,
            modelo.DataAlteracao,
            modelo.EstadoEntidade,
            modelo.Estado,
            modelo.SlotHorario is null ? null : SlotHorarioMapper.ParaEntidade(modelo.SlotHorario),
            ProcedimentoMapper.ParaEntidade(modelo.Procedimento),
            modelo.Reagendamento is null ? null : ParaEntidade(modelo.Reagendamento));
}
