using HoraCerta.Dominio;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Procedimento;
using HoraCerta.Dominio.Proprietario;
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
            ProcedimentoId = entidade.Procedimento.Id.Valor,
            SlotHorarioId = entidade.SlotHorario?.Id.Valor,
            ReagendamentoId = entidade.Reagendamento?.Id.Valor,
            Estado = entidade.EstadoAtual()
        };

    public static ICollection<AgendamentoEntidade> ParaEntidades(
        IEnumerable<AgendamentoModelo> modelos,
        ProprietarioEntidade proprietario)
    {
        var todos = modelos.ToList();
        var cache = new Dictionary<string, AgendamentoEntidade>();

        foreach (var modelo in todos)
            ParaEntidade(modelo, proprietario, cache, todos);

        return cache.Values.ToList();
    }

    public static AgendamentoEntidade ParaEntidade(
        AgendamentoModelo modelo,
        ProprietarioEntidade proprietario)
        => ParaEntidade(modelo, proprietario, new Dictionary<string, AgendamentoEntidade>(), [modelo]);

    /// <summary>
    /// Reidratação legada a partir de objetos embutidos no JSON (sem vínculo ao calendário do estabelecimento).
    /// </summary>
    public static AgendamentoEntidade ParaEntidadeLegado(AgendamentoModelo modelo)
    {
        var procedimento = modelo.Procedimento is not null
            ? ProcedimentoMapper.ParaEntidade(modelo.Procedimento)
            : throw new OperacaoInvalidaExcessao("Procedimento do agendamento não informado na persistência");

        var slot = modelo.SlotHorario is null ? null : SlotHorarioMapper.ParaEntidade(modelo.SlotHorario);
        var reagendamento = modelo.Reagendamento is null ? null : ParaEntidadeLegado(modelo.Reagendamento);

        return new AgendamentoEntidade(
            modelo.Id,
            modelo.DataCriacao,
            modelo.DataAlteracao,
            modelo.EstadoEntidade,
            modelo.Estado,
            slot,
            procedimento,
            reagendamento);
    }

    private static AgendamentoEntidade ParaEntidade(
        AgendamentoModelo modelo,
        ProprietarioEntidade proprietario,
        IDictionary<string, AgendamentoEntidade> cache,
        IReadOnlyList<AgendamentoModelo> todos)
    {
        if (cache.TryGetValue(modelo.Id, out var existente))
            return existente;

        var reagendamento = ResolverReagendamento(modelo, proprietario, cache, todos);
        var procedimento = ResolverProcedimento(modelo, proprietario);
        var slot = ResolverSlot(modelo, proprietario);

        var entidade = new AgendamentoEntidade(
            modelo.Id,
            modelo.DataCriacao,
            modelo.DataAlteracao,
            modelo.EstadoEntidade,
            modelo.Estado,
            slot,
            procedimento,
            reagendamento);

        cache[modelo.Id] = entidade;
        return entidade;
    }

    private static AgendamentoEntidade? ResolverReagendamento(
        AgendamentoModelo modelo,
        ProprietarioEntidade proprietario,
        IDictionary<string, AgendamentoEntidade> cache,
        IReadOnlyList<AgendamentoModelo> todos)
    {
        if (!string.IsNullOrEmpty(modelo.ReagendamentoId))
        {
            if (cache.TryGetValue(modelo.ReagendamentoId, out var emCache))
                return emCache;

            var modeloPai = todos.FirstOrDefault(m => m.Id == modelo.ReagendamentoId)
                ?? throw new OperacaoInvalidaExcessao($"Agendamento anterior {modelo.ReagendamentoId} não encontrado");

            return ParaEntidade(modeloPai, proprietario, cache, todos);
        }

        if (modelo.Reagendamento is not null)
            return ParaEntidade(modelo.Reagendamento, proprietario, cache, todos);

        return null;
    }

    private static ProcedimentoEntidade ResolverProcedimento(
        AgendamentoModelo modelo,
        ProprietarioEntidade proprietario)
    {
        if (!string.IsNullOrEmpty(modelo.ProcedimentoId))
            return proprietario.GerenciadorProcedimentos.BuscarProcedimentoPorId(new IdEntidade(modelo.ProcedimentoId));

        if (modelo.Procedimento is not null)
            return ProcedimentoMapper.ParaEntidade(modelo.Procedimento);

        throw new OperacaoInvalidaExcessao("Procedimento do agendamento não informado na persistência");
    }

    private static SlotHorarioEntidade? ResolverSlot(
        AgendamentoModelo modelo,
        ProprietarioEntidade proprietario)
    {
        if (!string.IsNullOrEmpty(modelo.SlotHorarioId))
        {
            return proprietario.Horarios.FirstOrDefault(s => s.Id.Valor == modelo.SlotHorarioId)
                ?? throw new OperacaoInvalidaExcessao($"Slot {modelo.SlotHorarioId} não encontrado no estabelecimento");
        }

        if (modelo.SlotHorario is not null)
            return SlotHorarioMapper.ParaEntidade(modelo.SlotHorario);

        return null;
    }
}
