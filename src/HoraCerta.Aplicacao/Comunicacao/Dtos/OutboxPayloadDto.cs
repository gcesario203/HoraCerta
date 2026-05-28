namespace HoraCerta.Aplicacao.Comunicacao.Dtos;

public record OutboxPayloadDto(string? AgendamentoId = null, string? LembreteId = null);
