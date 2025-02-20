using HoraCerta.Dominio.Procedimento;

namespace HoraCerta.Dominio.Proprietario;

public class ProprietarioEntidade : EntidadeBase<ProprietarioEntidade>
{
    public string Nome { get; private set; }

    public IGerenciadorProcedimentos GerenciadorProcedimentos { get; private set; }

    public ProprietarioEntidade(string nome, ICollection<ProcedimentoEntidade> procedimentos = null) :  base(new ValidadorProprietario())
    {
        Nome = nome;

        GerenciadorProcedimentos = new GerenciadorProcedimentos(this, procedimentos);

        _validador.Validar(this);
    }

    public void AtualizarNome(string nome)
    {
        Nome = nome;

        _validador.Validar(this);

        Atualizar();
    }
}
