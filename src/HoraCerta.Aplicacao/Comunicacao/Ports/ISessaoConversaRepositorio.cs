using HoraCerta.Aplicacao.Comunicacao.Dtos;

namespace HoraCerta.Aplicacao.Comunicacao.Ports;

public interface ISessaoConversaRepositorio
{
    SessaoConversaDto? Buscar(string telefone, string proprietarioId);

    void Salvar(SessaoConversaDto sessao);

    void RemoverExpiradas(DateTime antesDe);
}
