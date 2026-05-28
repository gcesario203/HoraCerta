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

public class EnviarNotificacaoCancelamentoWhatsAppHandler : IDomainEventHandler<AgendamentoCanceladoEvent>
{
    private readonly IEnfileiradorMensagemWhatsApp _enfileirador;
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IProprietarioRepositorio _proprietarioRepositorio;
    private readonly IMensagemOutboxRepositorio _outboxRepositorio;

    public EnviarNotificacaoCancelamentoWhatsAppHandler(
        IEnfileiradorMensagemWhatsApp enfileirador,
        IClienteRepositorio clienteRepositorio,
        IProprietarioRepositorio proprietarioRepositorio,
        IMensagemOutboxRepositorio outboxRepositorio)
    {
        _enfileirador = enfileirador;
        _clienteRepositorio = clienteRepositorio;
        _proprietarioRepositorio = proprietarioRepositorio;
        _outboxRepositorio = outboxRepositorio;
    }

    public void Handle(AgendamentoCanceladoEvent evento)
    {
        _outboxRepositorio.CancelarPorAgendamento(evento.AgendamentoId);

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
            TipoMensagemOutbox.NotificacaoCancelamento,
            evento.TelefoneCliente,
            evento.ProprietarioId,
            MensagensWhatsAppTemplates.Cancelamento(evento.SlotInicio),
            $"Cancelamento:{evento.AgendamentoId}",
            payload);
    }
}
