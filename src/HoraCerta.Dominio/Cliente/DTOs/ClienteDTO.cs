
using HoraCerta.Dominio._Shared;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Agendamento;

namespace HoraCerta.Dominio.Cliente;

public class ClienteDTO : DTOBase
{
    public ClienteDTO(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade,string nome, string telefone, ICollection<AgendamentoDTO> agendamentos) : base(id, dataCriacao, dataAlteracao, estadoEntidade)
    {
        Nome = nome;
        Telefone = telefone;
        Agendamentos = agendamentos;
    }

    public string Nome { get; private set; }

    public string Telefone { get; private set; }

    public ICollection<AgendamentoDTO> Agendamentos { get; private set; }
}
