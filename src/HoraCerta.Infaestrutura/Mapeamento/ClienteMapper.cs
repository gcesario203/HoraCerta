using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Proprietario;
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
            OptOutWhatsApp = entidade.OptOutWhatsApp,
            Agendamentos = entidade.GerenciadorAgendamentos.BuscarAgendamentos()
                .Select(AgendamentoMapper.ParaModelo)
                .ToList(),
            Avaliacoes = entidade.GerenciadorAgendamentos.Avaliacoes
                .Select(AvaliacaoMapper.ParaModelo)
                .ToList()
        };

    public static ClienteEntidade ParaEntidade(ClienteModelo modelo, ProprietarioEntidade proprietario)
        => new(
            modelo.Id,
            modelo.DataCriacao,
            modelo.DataAlteracao,
            modelo.EstadoEntidade,
            modelo.Nome,
            modelo.Telefone,
            modelo.OptOutWhatsApp,
            AgendamentoMapper.ParaEntidades(modelo.Agendamentos, proprietario),
            modelo.Avaliacoes.Select(AvaliacaoMapper.ParaEntidade).ToList());

    /// <summary>
    /// Reidratação legada (cópias embutidas no JSON). Usar apenas quando o estabelecimento não é necessário.
    /// </summary>
    public static ClienteEntidade ParaEntidadeLegado(ClienteModelo modelo)
        => new(
            modelo.Id,
            modelo.DataCriacao,
            modelo.DataAlteracao,
            modelo.EstadoEntidade,
            modelo.Nome,
            modelo.Telefone,
            modelo.OptOutWhatsApp,
            modelo.Agendamentos.Select(AgendamentoMapper.ParaEntidadeLegado).ToList(),
            modelo.Avaliacoes.Select(AvaliacaoMapper.ParaEntidade).ToList());
}
