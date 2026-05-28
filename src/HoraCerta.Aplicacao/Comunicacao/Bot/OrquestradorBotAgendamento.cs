using System.Text.RegularExpressions;
using HoraCerta.Aplicacao.Agendamento.Commands;
using HoraCerta.Aplicacao.Agendamento.Handlers;
using HoraCerta.Aplicacao.Cliente.Commands;
using HoraCerta.Aplicacao.Cliente.Handlers;
using HoraCerta.Aplicacao.Cliente.Queries;
using HoraCerta.Aplicacao.Comunicacao.Dtos;
using HoraCerta.Aplicacao.Comunicacao.Enums;
using HoraCerta.Aplicacao.Comunicacao.Ports;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Queries;
using HoraCerta.Dominio;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;
using Microsoft.Extensions.Options;

namespace HoraCerta.Aplicacao.Comunicacao.Bot;

public partial class OrquestradorBotAgendamento : IOrquestradorBotAgendamento
{
    private readonly ISessaoConversaRepositorio _sessaoRepositorio;
    private readonly IProprietarioRepositorio _proprietarioRepositorio;
    private readonly ListarProcedimentosAtivosHandler _listarProcedimentos;
    private readonly ListarSlotsDisponiveisHandler _listarSlots;
    private readonly BuscarClientePorTelefoneHandler _buscarCliente;
    private readonly CriarClienteHandler _criarCliente;
    private readonly IniciarAgendamentoHandler _iniciarAgendamento;
    private readonly TwilioOptions _twilioOptions;

    public OrquestradorBotAgendamento(
        ISessaoConversaRepositorio sessaoRepositorio,
        IProprietarioRepositorio proprietarioRepositorio,
        ListarProcedimentosAtivosHandler listarProcedimentos,
        ListarSlotsDisponiveisHandler listarSlots,
        BuscarClientePorTelefoneHandler buscarCliente,
        CriarClienteHandler criarCliente,
        IniciarAgendamentoHandler iniciarAgendamento,
        IOptions<TwilioOptions> twilioOptions)
    {
        _sessaoRepositorio = sessaoRepositorio;
        _proprietarioRepositorio = proprietarioRepositorio;
        _listarProcedimentos = listarProcedimentos;
        _listarSlots = listarSlots;
        _buscarCliente = buscarCliente;
        _criarCliente = criarCliente;
        _iniciarAgendamento = iniciarAgendamento;
        _twilioOptions = twilioOptions.Value;
    }

    public Task<string> ProcessarMensagemAsync(
        string telefone,
        string proprietarioIdInicial,
        string texto,
        CancellationToken cancellationToken = default)
    {
        var textoLimpo = texto.Trim();
        var proprietarioId = ResolverProprietarioId(textoLimpo, proprietarioIdInicial);

        if (string.IsNullOrWhiteSpace(proprietarioId))
        {
            return Task.FromResult(
                "Olá! Informe o código do estabelecimento (ex.: HC-{id}) ou acesse o link de agendamento do portal HoraCerta.");
        }

        var sessao = _sessaoRepositorio.Buscar(telefone, proprietarioId)
                       ?? NovaSessao(telefone, proprietarioId);

        if (sessao.ExpiraEm < DateTime.UtcNow)
            sessao = NovaSessao(telefone, proprietarioId);

        if (sessao.Passo == PassoFluxoBot.ResolverEstabelecimento && !string.IsNullOrWhiteSpace(proprietarioId))
            sessao.Passo = PassoFluxoBot.IdentificarCliente;

        var resposta = sessao.Passo switch
        {
            PassoFluxoBot.ResolverEstabelecimento => ResolverEstabelecimento(sessao, textoLimpo),
            PassoFluxoBot.IdentificarCliente => IdentificarCliente(sessao, textoLimpo),
            PassoFluxoBot.EscolherProcedimento => EscolherProcedimento(sessao, textoLimpo),
            PassoFluxoBot.EscolherHorario => EscolherHorario(sessao, textoLimpo),
            PassoFluxoBot.RevisarConfirmar => RevisarConfirmar(sessao, textoLimpo),
            PassoFluxoBot.Concluido => "Seu pedido já foi registrado. Para novo agendamento, envie HC-{id} novamente.",
            _ => "Não entendi. Digite *menu* para recomeçar."
        };

        AtualizarSessao(sessao);
        return Task.FromResult(resposta);
    }

    private string ResolverEstabelecimento(SessaoConversaDto sessao, string texto)
    {
        var id = ResolverProprietarioId(texto, sessao.ProprietarioId);
        if (string.IsNullOrWhiteSpace(id))
            return "Informe o código do estabelecimento (formato HC-{id}).";

        sessao.ProprietarioId = id;
        sessao.Passo = PassoFluxoBot.IdentificarCliente;
        return "Estabelecimento identificado. Qual é o seu nome completo?";
    }

    private string IdentificarCliente(SessaoConversaDto sessao, string texto)
    {
        if (texto.Equals("menu", StringComparison.OrdinalIgnoreCase))
        {
            sessao.Passo = PassoFluxoBot.IdentificarCliente;
            sessao.NomePendente = null;
            return "Vamos recomeçar. Qual é o seu nome completo?";
        }

        var proprietario = _proprietarioRepositorio.BuscarPorId(new IdEntidade(sessao.ProprietarioId));
        if (proprietario is null)
            return "Estabelecimento não encontrado.";

        var clienteExistente = _buscarCliente.Executar(
            new BuscarClientePorTelefoneQuery(new IdEntidade(sessao.ProprietarioId), sessao.Telefone));

        if (clienteExistente is not null)
        {
            sessao.ClienteId = clienteExistente.Id.Valor;
            sessao.Passo = PassoFluxoBot.EscolherProcedimento;
            return ListarProcedimentos(sessao);
        }

        if (string.IsNullOrWhiteSpace(sessao.NomePendente))
        {
            if (HcProprietario().IsMatch(texto) || texto.Length < 2)
                return "Qual é o seu nome completo?";

            sessao.NomePendente = texto;
            return $"Confirma o nome *{texto}*? Responda SIM ou informe outro nome.";
        }

        if (texto.Equals("sim", StringComparison.OrdinalIgnoreCase))
        {
            var cliente = _criarCliente.Executar(new CriarClienteCommand(sessao.NomePendente, sessao.Telefone));
            sessao.ClienteId = cliente.Id.Valor;
            sessao.NomePendente = null;
            sessao.Passo = PassoFluxoBot.EscolherProcedimento;
            return ListarProcedimentos(sessao);
        }

        sessao.NomePendente = texto;
        return $"Confirma o nome *{texto}*? Responda SIM.";
    }

    private string ListarProcedimentos(SessaoConversaDto sessao)
    {
        var procedimentos = _listarProcedimentos
            .Executar(new ListarProcedimentosAtivosQuery(new IdEntidade(sessao.ProprietarioId)))
            .Take(9)
            .ToList();

        if (procedimentos.Count == 0)
            return "Nenhum procedimento disponível no momento.";

        sessao.Passo = PassoFluxoBot.EscolherProcedimento;
        var linhas = procedimentos
            .Select((p, i) => $"{i + 1}. {p.Nome}")
            .ToList();

        return "Escolha o procedimento (número):\n" + string.Join("\n", linhas);
    }

    private string EscolherProcedimento(SessaoConversaDto sessao, string texto)
    {
        var procedimentos = _listarProcedimentos
            .Executar(new ListarProcedimentosAtivosQuery(new IdEntidade(sessao.ProprietarioId)))
            .Take(9)
            .ToList();

        if (!int.TryParse(texto, out var indice) || indice < 1 || indice > procedimentos.Count)
            return "Opção inválida. " + ListarProcedimentos(sessao);

        sessao.ProcedimentoId = procedimentos[indice - 1].Id.Valor;
        sessao.Passo = PassoFluxoBot.EscolherHorario;
        return ListarHorarios(sessao);
    }

    private string ListarHorarios(SessaoConversaDto sessao)
    {
        var slots = _listarSlots
            .Executar(new ListarSlotsDisponiveisQuery(new IdEntidade(sessao.ProprietarioId)))
            .Where(s => s.Inicio >= DateTime.UtcNow)
            .OrderBy(s => s.Inicio)
            .Take(9)
            .ToList();

        if (slots.Count == 0)
            return "Não há horários disponíveis. Tente outro procedimento ou mais tarde.";

        var linhas = slots
            .Select((s, i) => $"{i + 1}. {s.Inicio.ToLocalTime():dd/MM HH:mm}")
            .ToList();

        return "Escolha o horário (número):\n" + string.Join("\n", linhas);
    }

    private string EscolherHorario(SessaoConversaDto sessao, string texto)
    {
        var slots = _listarSlots
            .Executar(new ListarSlotsDisponiveisQuery(new IdEntidade(sessao.ProprietarioId)))
            .Where(s => s.Inicio >= DateTime.UtcNow)
            .OrderBy(s => s.Inicio)
            .Take(9)
            .ToList();

        if (!int.TryParse(texto, out var indice) || indice < 1 || indice > slots.Count)
            return "Opção inválida. " + ListarHorarios(sessao);

        var slot = slots[indice - 1];
        sessao.SlotHorarioId = slot.Id.Valor;
        sessao.Passo = PassoFluxoBot.RevisarConfirmar;

        var proprietario = _proprietarioRepositorio.BuscarPorId(new IdEntidade(sessao.ProprietarioId));
        var procedimento = proprietario?.GerenciadorProcedimentos
            .BuscarProcedimentoPorId(new IdEntidade(sessao.ProcedimentoId!));

        return $"Confirmar agendamento?\nProcedimento: {procedimento?.Nome}\nHorário: {slot.Inicio.ToLocalTime():dd/MM/yyyy HH:mm}\n\nResponda SIM para confirmar ou NAO para escolher outro horário.";
    }

    private string RevisarConfirmar(SessaoConversaDto sessao, string texto)
    {
        if (texto.Equals("nao", StringComparison.OrdinalIgnoreCase)
            || texto.Equals("não", StringComparison.OrdinalIgnoreCase))
        {
            sessao.Passo = PassoFluxoBot.EscolherHorario;
            return ListarHorarios(sessao);
        }

        if (!texto.Equals("sim", StringComparison.OrdinalIgnoreCase))
            return "Responda SIM para confirmar ou NAO para escolher outro horário.";

        if (sessao.ClienteId is null || sessao.ProcedimentoId is null || sessao.SlotHorarioId is null)
            return "Sessão incompleta. Envie *menu* para recomeçar.";

        _iniciarAgendamento.Executar(new IniciarAgendamentoCommand(
            new IdEntidade(sessao.ProprietarioId),
            new IdEntidade(sessao.ClienteId),
            new IdEntidade(sessao.ProcedimentoId),
            new IdEntidade(sessao.SlotHorarioId)));

        sessao.Passo = PassoFluxoBot.Concluido;
        return "Pedido enviado! Aguarde a confirmação do estabelecimento no portal. Obrigado.";
    }

    private static string? ResolverProprietarioId(string texto, string proprietarioIdInicial)
    {
        if (!string.IsNullOrWhiteSpace(proprietarioIdInicial)
            && Guid.TryParse(proprietarioIdInicial, out _))
            return proprietarioIdInicial;

        var match = HcProprietario().Match(texto);
        if (match.Success && Guid.TryParse(match.Groups[1].Value, out _))
            return match.Groups[1].Value;

        if (Guid.TryParse(texto, out _))
            return texto;

        return null;
    }

    private SessaoConversaDto NovaSessao(string telefone, string proprietarioId)
        => new()
        {
            Telefone = telefone,
            ProprietarioId = proprietarioId,
            Passo = PassoFluxoBot.ResolverEstabelecimento,
            AtualizadoEm = DateTime.UtcNow,
            ExpiraEm = DateTime.UtcNow.AddHours(_twilioOptions.SessaoExpiracaoHoras)
        };

    private void AtualizarSessao(SessaoConversaDto sessao)
    {
        sessao.AtualizadoEm = DateTime.UtcNow;
        sessao.ExpiraEm = DateTime.UtcNow.AddHours(_twilioOptions.SessaoExpiracaoHoras);
        _sessaoRepositorio.Salvar(sessao);
    }

    [GeneratedRegex(@"HC[-\s]?([0-9a-fA-F\-]{36})", RegexOptions.IgnoreCase)]
    private static partial Regex HcProprietario();
}
