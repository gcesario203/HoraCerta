using HoraCerta.Aplicacao.Comunicacao.Dtos;
using HoraCerta.Aplicacao.Comunicacao.Enums;
using HoraCerta.Aplicacao.Comunicacao.Ports;
using HoraCerta.Infaestrutura.Persistencia;
using HoraCerta.Infaestrutura.Persistencia.Registros;
using Microsoft.EntityFrameworkCore;

namespace HoraCerta.Infaestrutura.Comunicacao.Outbox;

public class EfMensagemOutboxRepositorio : IMensagemOutboxRepositorio
{
    private readonly HoraCertaDbContext _context;

    public EfMensagemOutboxRepositorio(HoraCertaDbContext context)
    {
        _context = context;
    }

    public void Adicionar(MensagemOutboxPendente mensagem)
    {
        _context.MensagensOutbox.Add(new MensagemOutboxRegistro
        {
            Id = mensagem.Id,
            Tipo = mensagem.Tipo.ToString(),
            TelefoneDestino = mensagem.TelefoneDestino,
            ProprietarioId = mensagem.ProprietarioId,
            Corpo = mensagem.Corpo,
            PayloadJson = mensagem.PayloadJson,
            IdempotencyKey = mensagem.IdempotencyKey,
            Status = nameof(StatusMensagemOutbox.Pendente),
            Tentativas = mensagem.Tentativas,
            ProximaTentativaEm = mensagem.ProximaTentativaEm,
            CriadoEm = DateTime.UtcNow
        });
        _context.SaveChanges();
    }

    public bool ExistePorIdempotencyKey(string idempotencyKey)
        => _context.MensagensOutbox.Any(x =>
            x.IdempotencyKey == idempotencyKey
            && (x.Status == nameof(StatusMensagemOutbox.Pendente)
                || x.Status == nameof(StatusMensagemOutbox.Processando)
                || x.Status == nameof(StatusMensagemOutbox.Enviado)));

    public IReadOnlyList<MensagemOutboxPendente> ReservarPendentes(DateTime ate, int limite)
    {
        var ids = _context.MensagensOutbox
            .Where(x => x.Status == nameof(StatusMensagemOutbox.Pendente) && x.ProximaTentativaEm <= ate)
            .OrderBy(x => x.ProximaTentativaEm)
            .Take(limite)
            .Select(x => x.Id)
            .ToList();

        var reservados = new List<MensagemOutboxPendente>();

        foreach (var id in ids)
        {
            var registro = _context.MensagensOutbox.FirstOrDefault(x =>
                x.Id == id && x.Status == nameof(StatusMensagemOutbox.Pendente));

            if (registro is null)
                continue;

            registro.Status = nameof(StatusMensagemOutbox.Processando);
            _context.SaveChanges();
            reservados.Add(Mapear(registro));
        }

        return reservados;
    }

    public void MarcarEnviado(string id)
    {
        var registro = _context.MensagensOutbox.Find(id);
        if (registro is null)
            return;

        registro.Status = nameof(StatusMensagemOutbox.Enviado);
        registro.EnviadoEm = DateTime.UtcNow;
        registro.UltimoErro = null;
        _context.SaveChanges();
    }

    public void RegistrarFalha(string id, string erro, DateTime proximaTentativaEm, int tentativas)
    {
        var registro = _context.MensagensOutbox.Find(id);
        if (registro is null)
            return;

        registro.Status = nameof(StatusMensagemOutbox.Pendente);
        registro.Tentativas = tentativas;
        registro.ProximaTentativaEm = proximaTentativaEm;
        registro.UltimoErro = erro;
        _context.SaveChanges();
    }

    public void MarcarFalhaDefinitiva(string id, string erro)
    {
        var registro = _context.MensagensOutbox.Find(id);
        if (registro is null)
            return;

        registro.Status = nameof(StatusMensagemOutbox.Falha);
        registro.UltimoErro = erro;
        _context.SaveChanges();
    }

    public void CancelarPorAgendamento(string agendamentoId)
    {
        var mensagens = _context.MensagensOutbox
            .Where(x => x.PayloadJson != null && x.PayloadJson.Contains(agendamentoId)
                && (x.Status == nameof(StatusMensagemOutbox.Pendente)
                    || x.Status == nameof(StatusMensagemOutbox.Processando)))
            .ToList();

        foreach (var msg in mensagens)
            msg.Status = nameof(StatusMensagemOutbox.Cancelado);

        if (mensagens.Count > 0)
            _context.SaveChanges();
    }

    private static MensagemOutboxPendente Mapear(MensagemOutboxRegistro registro)
        => new(
            registro.Id,
            Enum.Parse<TipoMensagemOutbox>(registro.Tipo),
            registro.TelefoneDestino,
            registro.ProprietarioId,
            registro.Corpo,
            registro.IdempotencyKey,
            registro.PayloadJson,
            registro.Tentativas,
            registro.ProximaTentativaEm);
}
