using HoraCerta.Dominio.Proprietario;

namespace HoraCerta.Dominio.Procedimento;

public class GerenciadorProcedimentos : IGerenciadorProcedimentos
{
    private readonly ProprietarioEntidade _proprietario;
    public ICollection<ProcedimentoEntidade> Procedimentos { get ; private set; }

    public GerenciadorProcedimentos(ProprietarioEntidade proprietario, ICollection<ProcedimentoEntidade>? procedimentos)
    {
        if (procedimentos == null || !procedimentos.Any())
            Procedimentos = new List<ProcedimentoEntidade>();
        else
            Procedimentos = procedimentos;

        _proprietario = proprietario;
    }

    public void CriarProcedimento(string nome, decimal valor, TimeSpan tempoEstimado)
    {
        var procedimento = new ProcedimentoEntidade(nome, valor, tempoEstimado);

        Procedimentos.Add(procedimento);
    }

    public void AlterarProcedimento(IdEntidade id, string nome, decimal valor, TimeSpan tempoEstimado)
    {
        var procedimentoParaAlterar = BuscarProcedimentoPorId(id);

        procedimentoParaAlterar.AtualizarNome(nome);
        procedimentoParaAlterar.AtualizarValor(valor);
        procedimentoParaAlterar.AtualizarTempoEstimado(tempoEstimado);
    }

    public void InativarProcedimento(ProcedimentoEntidade procedimentoEntidade)
    {
        var procedimentoParaAlterar = BuscarProcedimentoPorId(procedimentoEntidade.Id);

        procedimentoParaAlterar.MudarStatus(_Shared.Enums.EstadoEntidade.INATIVO);
    }


    public void AtivarProcedimento(ProcedimentoEntidade procedimentoEntidade)
    {
        var procedimentoParaAlterar = BuscarProcedimentoPorId(procedimentoEntidade.Id);

        procedimentoParaAlterar.MudarStatus(_Shared.Enums.EstadoEntidade.ATIVO);
    }

    public ProcedimentoEntidade BuscarProcedimentoPorId(IdEntidade idEntidade)
    {
        var procedimento = Procedimentos.FirstOrDefault(procedimento => procedimento.Id.Valor == idEntidade.Valor);

        if (procedimento is null)
            throw new OperacaoInvalidaExcessao("Procedimento não cadastrado");

        return procedimento;
    }
}
