using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Agenda;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Procedimento;

namespace HoraCerta.Dominio.Proprietario;

public class ProprietarioEntidade : EntidadeBase<ProprietarioEntidade>
{
    public string Nome { get; private set; }

    public IGerenciadorProcedimentos GerenciadorProcedimentos { get; private set; }

    public IGerenciadorAgenda GerenciadorAgenda { get; private set; }

    public ProprietarioEntidade(string nome, ICollection<ProcedimentoEntidade>? procedimentos = null, ICollection<SlotHorarioEntidade>? horarios = null, ICollection<AtendimentoEntidade>? atendimentos = null) : base(new ValidadorProprietario())
    {
        Nome = nome;

        GerenciadorProcedimentos = new GerenciadorProcedimentos(procedimentos ?? null);

        GerenciadorAgenda = new GerenciadorAgenda(horarios, atendimentos);

        _validador!.Validar(this);
    }

    private ProprietarioEntidade(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade, string nome, AgendaEntidade? agenda = null, ICollection<ProcedimentoEntidade>? procedimentos = null)
    : base(id, dataCriacao, dataAlteracao, estadoEntidade, new ValidadorProprietario())
    {
        Nome = nome;

        GerenciadorProcedimentos = new GerenciadorProcedimentos(procedimentos ?? null);

        GerenciadorAgenda = new GerenciadorAgenda(agenda ?? new AgendaEntidade(null, null));

        _validador!.Validar(this);
    }

    public void AtualizarNome(string nome)
    {
        Nome = nome;

        _validador!.Validar(this);

        Atualizar();
    }

    public static ProprietarioDTO ParaDTO(ProprietarioEntidade proprietario)
     => new ProprietarioDTO(proprietario.Id.Valor, proprietario.DataCriacao, proprietario.DataAlteracao, proprietario.EstadoEntidade, proprietario.Nome, AgendaEntidade.ParaDTO(proprietario.GerenciadorAgenda.RecuperarAgenda()), proprietario.GerenciadorProcedimentos.RecuperarProcedimentos()?.Select(ProcedimentoEntidade.ParaDTO)?.ToList());
}
