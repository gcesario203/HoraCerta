using HoraCerta.Aplicacao.Integracao.Lembretes;
using HoraCerta.Infaestrutura.Persistencia;
using HoraCerta.Infaestrutura.Persistencia.Registros;
using Microsoft.EntityFrameworkCore;

namespace HoraCerta.Infaestrutura.Repositorio;

public class EfLembreteRepositorio : ILembreteRepositorio
{
    private readonly HoraCertaDbContext _context;

    public EfLembreteRepositorio(HoraCertaDbContext context)
    {
        _context = context;
    }

    public void Agendar(
        string proprietarioId,
        string clienteId,
        string agendamentoId,
        string telefoneCliente,
        DateTime slotInicio,
        DateTime enviarEm)
    {
        var existente = _context.Lembretes
            .FirstOrDefault(x => x.AgendamentoId == agendamentoId && x.Status == nameof(LembreteStatus.Pendente));

        if (existente is not null)
            return;

        _context.Lembretes.Add(new LembreteRegistro
        {
            ProprietarioId = proprietarioId,
            ClienteId = clienteId,
            AgendamentoId = agendamentoId,
            TelefoneCliente = telefoneCliente,
            SlotInicio = slotInicio,
            EnviarEm = enviarEm,
            Status = nameof(LembreteStatus.Pendente)
        });

        _context.SaveChanges();
    }

    public void CancelarPorAgendamento(string agendamentoId)
    {
        var lembretes = _context.Lembretes
            .Where(x => x.AgendamentoId == agendamentoId && x.Status == nameof(LembreteStatus.Pendente))
            .ToList();

        foreach (var lembrete in lembretes)
            lembrete.Status = nameof(LembreteStatus.Cancelado);

        if (lembretes.Count > 0)
            _context.SaveChanges();
    }

    public void Reagendar(
        string agendamentoAnteriorId,
        string novoAgendamentoId,
        DateTime novoSlotInicio,
        DateTime enviarEm)
    {
        CancelarPorAgendamento(agendamentoAnteriorId);

        var anterior = _context.Lembretes
            .AsNoTracking()
            .FirstOrDefault(x => x.AgendamentoId == agendamentoAnteriorId);

        if (anterior is null)
            return;

        Agendar(
            anterior.ProprietarioId,
            anterior.ClienteId,
            novoAgendamentoId,
            anterior.TelefoneCliente,
            novoSlotInicio,
            enviarEm);
    }

    public IReadOnlyList<LembretePendente> BuscarPendentesParaEnvio(DateTime ate)
        => _context.Lembretes
            .AsNoTracking()
            .Where(x => x.Status == nameof(LembreteStatus.Pendente) && x.EnviarEm <= ate)
            .Select(x => new LembretePendente(
                x.Id,
                x.ProprietarioId,
                x.ClienteId,
                x.AgendamentoId,
                x.TelefoneCliente,
                x.SlotInicio,
                x.EnviarEm,
                LembreteStatus.Pendente))
            .ToList();

    public void MarcarEnviado(string id)
    {
        var lembrete = _context.Lembretes.Find(id);

        if (lembrete is null)
            return;

        lembrete.Status = nameof(LembreteStatus.Enviado);
        lembrete.EnviadoEm = DateTime.UtcNow;
        _context.SaveChanges();
    }
}
