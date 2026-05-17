using HoraCerta.Dominio._Shared.Abstracoes;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Agendamento;

namespace HoraCerta.Dominio.Cliente;

public class ClienteEntidade : AggregateRootBase<ClienteEntidade>
{
    public string Nome { get; private set; }

    public string Telefone { get; private set; }

    public IGerenciadorAgendamentos GerenciadorAgendamentos { get; private set; }

    public ClienteEntidade(
        string nome,
        string telefone,
        ICollection<AgendamentoEntidade>? agendamentos = null,
        ICollection<AvaliacaoEntidade>? avaliacoes = null) : base(new ValidadorCliente())
    {
        Nome = nome;
        Telefone = telefone;

        _validador?.Validar(this);

        GerenciadorAgendamentos = new GerenciadorAgendamentos(this, agendamentos, avaliacoes);
    }

    internal ClienteEntidade(
        string id,
        DateTime dataCriacao,
        DateTime? dataAlteracao,
        EstadoEntidade estadoEntidade,
        string nome,
        string telefone,
        ICollection<AgendamentoEntidade>? agendamentos = null,
        ICollection<AvaliacaoEntidade>? avaliacoes = null)
        : base(id, dataCriacao, dataAlteracao, estadoEntidade, new ValidadorCliente())
    {
        Nome = nome;
        Telefone = telefone;

        _validador?.Validar(this);

        GerenciadorAgendamentos = new GerenciadorAgendamentos(this, agendamentos, avaliacoes);
    }

    public void AtualizarNome(string nome)
    {
        Nome = nome;
        _validador?.Validar(this);
        Atualizar();
    }

    public void AtualizarTelefone(string telefone)
    {
        Telefone = telefone;
        _validador?.Validar(this);
        Atualizar();
    }
}
