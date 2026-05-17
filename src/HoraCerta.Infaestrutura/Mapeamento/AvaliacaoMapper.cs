using HoraCerta.Dominio.Cliente;
using HoraCerta.Infaestrutura.Persistencia.Modelos;

namespace HoraCerta.Infaestrutura.Mapeamento;

public static class AvaliacaoMapper
{
    public static AvaliacaoModelo ParaModelo(AvaliacaoEntidade entidade)
        => new()
        {
            AgendamentoId = entidade.AgendamentoId.Valor,
            ProprietarioId = entidade.ProprietarioId.Valor,
            Nota = entidade.Nota,
            Comentario = entidade.Comentario,
            DataAvaliacao = entidade.DataAvaliacao
        };

    public static AvaliacaoEntidade ParaEntidade(AvaliacaoModelo modelo)
        => new(
            modelo.AgendamentoId,
            modelo.ProprietarioId,
            modelo.Nota,
            modelo.Comentario,
            modelo.DataAvaliacao);
}
