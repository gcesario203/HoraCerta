using HoraCerta.Dominio._Shared;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Procedimento;
using HoraCerta.Dominio.SlotHorario;

namespace HoraCerta.Dominio.Atendimento;

public class AtendimentoDTO : DTOBase
{
    public AtendimentoDTO(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade, AgendamentoDTO origem, decimal valorNegociado, EstadoAtendimento estado) : base(id, dataCriacao, dataAlteracao, estadoEntidade)
    {
        Origem = origem;
        ValorNegociado = valorNegociado;
        Estado = estado;
    }

    public AgendamentoDTO Origem { get; }

    public decimal ValorNegociado { get; private set; }

    public EstadoAtendimento Estado { get; private set; }
}