using System.Text.Json;
using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Aplicacao.Comunicacao.Dtos;
using HoraCerta.Aplicacao.Comunicacao.Enums;
using HoraCerta.Aplicacao.Comunicacao.Ports;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Cliente.Eventos;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Comunicacao.Eventos;

public class EnviarNotificacaoConfirmacaoWhatsAppHandler : IDomainEventHandler<AgendamentoConfirmadoEvent>
{
    private readonly IEnfileiradorMensagemWhatsApp _enfileirador;
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IProprietarioRepositorio _proprietarioRepositorio;

    public EnviarNotificacaoConfirmacaoWhatsAppHandler(
        IEnfileiradorMensagemWhatsApp enfileirador,
        IClienteRepositorio clienteRepositorio,
        IProprietarioRepositorio proprietarioRepositorio)
    {
        _enfileirador = enfileirador;
        _clienteRepositorio = clienteRepositorio;
        _proprietarioRepositorio = proprietarioRepositorio;
    }

    public void Handle(AgendamentoConfirmadoEvent evento)
    {
        if (string.IsNullOrWhiteSpace(evento.TelefoneCliente))
            return;

        var proprietario = _proprietarioRepositorio.BuscarPorId(new IdEntidade(evento.ProprietarioId));
        if (proprietario is null)
            return;

        var cliente = _clienteRepositorio.BuscarPorId(new IdEntidade(evento.ClienteId), proprietario);
        if (cliente?.OptOutWhatsApp == true)
            return;

        var payload = JsonSerializer.Serialize(new OutboxPayloadDto(evento.AgendamentoId));

        _enfileirador.Enfileirar(
            TipoMensagemOutbox.NotificacaoConfirmacao,
            evento.TelefoneCliente,
            evento.ProprietarioId,
            MensagensWhatsAppTemplates.Confirmacao(evento.SlotInicio),
            $"Confirmacao:{evento.AgendamentoId}",
            payload);
    }
}
