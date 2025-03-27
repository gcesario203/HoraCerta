using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Agendamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Cliente;

public class ClienteEntidade : EntidadeBase<ClienteEntidade>
{
    public string Nome { get; private set; }

    public string Telefone { get; private set; }

    public IGerenciadorAgendamentos GerenciadorAgendamentos { get; private set; }

    public ClienteEntidade(string nome, string telefone, ICollection<AgendamentoEntidade> agendamentos = null) : base(new ValidadorCliente())
    {
        Nome = nome;
        Telefone = telefone;

        _validador.Validar(this);


        GerenciadorAgendamentos = new GerenciadorAgendamentos(this, agendamentos);

    }

    private ClienteEntidade(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade, string nome, string telefone, ICollection<AgendamentoEntidade> agendamentos = null)
    : base(id, dataCriacao, dataAlteracao, estadoEntidade, new ValidadorCliente())
    {
        Nome = nome;
        Telefone = telefone;

        _validador.Validar(this);


        GerenciadorAgendamentos = new GerenciadorAgendamentos(this, agendamentos);
    }

    public void AtualizarNome(string nome)
    {
        Nome = nome;

        _validador.Validar(this);

        Atualizar();
    }

    public void AtualizarTelefone(string telefone)
    {
        Telefone = telefone;

        _validador.Validar(this);

        Atualizar();
    }

    public static ClienteDTO ParaDTO(ClienteEntidade cliente)
    => new ClienteDTO(cliente.Id.Valor, cliente.DataCriacao, cliente.DataAlteracao, cliente.EstadoEntidade, cliente.Nome, cliente.Telefone, cliente.GerenciadorAgendamentos.BuscarAgendamentos().Select(x => AgendamentoEntidade.ParaDTO(x))?.ToList() ?? new List<AgendamentoDTO>());

    public static ClienteEntidade ParaEntidade(ClienteDTO cliente)
    => new ClienteEntidade(cliente.Id, cliente.DataCriacao, cliente.DataAlteracao, cliente.EstadoEntidade, cliente.Nome, cliente.Telefone, cliente.Agendamentos.Select(x => AgendamentoEntidade.ParaEntidade(x))?.ToList() ?? new List<AgendamentoEntidade>());
}
