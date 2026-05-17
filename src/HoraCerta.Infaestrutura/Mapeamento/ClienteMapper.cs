using HoraCerta.Dominio.Cliente;
using HoraCerta.Infaestrutura.Persistencia.Modelos;

namespace HoraCerta.Infaestrutura.Mapeamento;

public static class ClienteMapper
{
    public static ClienteModelo ParaModelo(ClienteEntidade entidade)
        => new()
        {
            Id = entidade.Id.Valor,
            DataCriacao = entidade.DataCriacao,
            DataAlteracao = entidade.DataAlteracao,
            EstadoEntidade = entidade.EstadoEntidade,
            Nome = entidade.Nome,
            Telefone = entidade.Telefone,
            Agendamentos = entidade.GerenciadorAgendamentos.BuscarAgendamentos()
                .Select(AgendamentoMapper.ParaModelo)
                .ToList()
        };

    public static ClienteEntidade ParaEntidade(ClienteModelo modelo)
        => new(
            modelo.Id,
            modelo.DataCriacao,
            modelo.DataAlteracao,
            modelo.EstadoEntidade,
            modelo.Nome,
            modelo.Telefone,
            modelo.Agendamentos.Select(AgendamentoMapper.ParaEntidade).ToList());
}
