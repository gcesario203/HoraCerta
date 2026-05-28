using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Cliente.Queries;

public record BuscarClientePorTelefoneQuery(IdEntidade ProprietarioId, string Telefone);
