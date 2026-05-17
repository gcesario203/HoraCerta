using HoraCerta.Dominio._Shared.Abstracoes;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Agenda;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Procedimento;

namespace HoraCerta.Dominio.Proprietario;

public class ProprietarioEntidade : AggregateRootBase<ProprietarioEntidade>
{
    public string Nome { get; private set; }

    public ICollection<SlotHorarioEntidade> Horarios { get; private set; }

    public ICollection<AtendimentoEntidade> Atendimentos { get; private set; }

    public IGerenciadorProcedimentos GerenciadorProcedimentos { get; private set; }

    public IGerenciadorAgenda GerenciadorAgenda { get; private set; }

    public ProprietarioEntidade(
        string nome,
        ICollection<ProcedimentoEntidade>? procedimentos = null,
        ICollection<SlotHorarioEntidade>? horarios = null,
        ICollection<AtendimentoEntidade>? atendimentos = null) : base(new ValidadorProprietario())
    {
        Nome = nome;

        Horarios = horarios is null || !horarios.Any()
            ? new List<SlotHorarioEntidade>()
            : horarios;

        Atendimentos = atendimentos is null || !atendimentos.Any()
            ? new List<AtendimentoEntidade>()
            : atendimentos;

        GerenciadorProcedimentos = new GerenciadorProcedimentos(procedimentos);

        GerenciadorAgenda = new GerenciadorAgenda(this);

        _validador!.Validar(this);
    }

    private ProprietarioEntidade(
        string id,
        DateTime dataCriacao,
        DateTime? dataAlteracao,
        EstadoEntidade estadoEntidade,
        string nome,
        ICollection<SlotHorarioEntidade>? horarios,
        ICollection<AtendimentoEntidade>? atendimentos,
        ICollection<ProcedimentoEntidade>? procedimentos)
    : base(id, dataCriacao, dataAlteracao, estadoEntidade, new ValidadorProprietario())
    {
        Nome = nome;

        Horarios = horarios is null || !horarios.Any()
            ? new List<SlotHorarioEntidade>()
            : horarios;

        Atendimentos = atendimentos is null || !atendimentos.Any()
            ? new List<AtendimentoEntidade>()
            : atendimentos;

        GerenciadorProcedimentos = new GerenciadorProcedimentos(procedimentos);

        GerenciadorAgenda = new GerenciadorAgenda(this);

        _validador!.Validar(this);
    }

    public void AtualizarNome(string nome)
    {
        Nome = nome;

        _validador!.Validar(this);

        Atualizar();
    }

    public static ProprietarioDTO ParaDTO(ProprietarioEntidade proprietario)
        => new ProprietarioDTO(
            proprietario.Id.Valor,
            proprietario.DataCriacao,
            proprietario.DataAlteracao,
            proprietario.EstadoEntidade,
            proprietario.Nome,
            proprietario.Horarios.Select(SlotHorarioEntidade.ParaDTO).ToList(),
            proprietario.Atendimentos.Select(AtendimentoEntidade.ParaDTO).ToList(),
            proprietario.GerenciadorProcedimentos.RecuperarProcedimentos()?.Select(ProcedimentoEntidade.ParaDTO).ToList());

    public static ProprietarioEntidade ParaEntidade(ProprietarioDTO dto)
        => new ProprietarioEntidade(
            dto.Id,
            dto.DataCriacao,
            dto.DataAlteracao,
            dto.EstadoEntidade,
            dto.Nome,
            dto.Horarios?.Select(SlotHorarioEntidade.ParaEntidade).ToList(),
            dto.Atendimentos?.Select(AtendimentoEntidade.ParaEntidade).ToList(),
            dto.Procedimentos?.Select(ProcedimentoEntidade.ParaEntidade).ToList());
}
