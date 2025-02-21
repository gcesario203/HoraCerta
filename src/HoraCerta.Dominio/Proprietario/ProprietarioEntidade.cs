using HoraCerta.Dominio.Agenda;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Procedimento;

namespace HoraCerta.Dominio.Proprietario;

public class ProprietarioEntidade : EntidadeBase<ProprietarioEntidade>
{
    public string Nome { get; private set; }

    public IGerenciadorProcedimentos GerenciadorProcedimentos { get; private set; }

    public IGerenciadorAgenda GerenciadorAgenda { get; private set; }

    public ProprietarioEntidade(string nome, ICollection<ProcedimentoEntidade> procedimentos = null, ICollection<SlotHorarioEntidade> horarios = null, ICollection<AtendimentoEntidade> atendimentos = null) :  base(new ValidadorProprietario())
    {
        Nome = nome;

        GerenciadorProcedimentos = new GerenciadorProcedimentos(this, procedimentos);

        GerenciadorAgenda = new GerenciadorAgenda(this, horarios, atendimentos);

        _validador.Validar(this);
    }

    public void AtualizarNome(string nome)
    {
        Nome = nome;

        _validador.Validar(this);

        Atualizar();
    }
}
