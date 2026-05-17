using HoraCerta.Api.Contratos;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Procedimento;
using HoraCerta.Dominio.Proprietario;

namespace HoraCerta.Api.Mapeamento;

public static class RespostaMapeamento
{
    public static ProprietarioResposta ParaResposta(ProprietarioEntidade entidade)
        => new(entidade.Id.Valor, entidade.Nome);

    public static ClienteResposta ParaResposta(ClienteEntidade entidade)
        => new(entidade.Id.Valor, entidade.Nome, entidade.Telefone);

    public static ProcedimentoResposta ParaResposta(ProcedimentoEntidade entidade)
        => new(
            entidade.Id.Valor,
            entidade.Nome,
            entidade.Valor,
            (int)entidade.TempoEstimado.TotalMinutes,
            entidade.EstadoEntidade.ToString());

    public static SlotHorarioResposta ParaResposta(SlotHorarioEntidade entidade)
        => new(
            entidade.Id.Valor,
            entidade.Inicio,
            entidade.Fim,
            entidade.Status.ToString());

    public static AgendamentoResposta ParaResposta(AgendamentoEntidade entidade, string clienteId)
        => new(
            entidade.Id.Valor,
            clienteId,
            entidade.Procedimento.Id.Valor,
            entidade.SlotHorario?.Id.Valor,
            entidade.EstadoAtual().ToString(),
            entidade.Reagendamento?.Id.Valor);

    public static AtendimentoResposta ParaResposta(AtendimentoEntidade entidade)
        => new(
            entidade.Id.Valor,
            entidade.Origem.Id.Valor,
            entidade.ValorNegociado,
            entidade.EstadoAtual().ToString());

    public static IdEntidade Id(string valor) => new(valor);
}
