using HoraCerta.Aplicacao.Comunicacao.Dtos;
using HoraCerta.Aplicacao.Comunicacao.Enums;
using HoraCerta.Aplicacao.Comunicacao.Ports;
using HoraCerta.Infaestrutura.Persistencia;
using HoraCerta.Infaestrutura.Persistencia.Registros;
using Microsoft.EntityFrameworkCore;

namespace HoraCerta.Infaestrutura.Comunicacao.Repositorio;

public class EfSessaoConversaRepositorio : ISessaoConversaRepositorio
{
    private readonly HoraCertaDbContext _context;

    public EfSessaoConversaRepositorio(HoraCertaDbContext context)
    {
        _context = context;
    }

    public SessaoConversaDto? Buscar(string telefone, string proprietarioId)
    {
        var registro = _context.SessoesConversa
            .AsNoTracking()
            .FirstOrDefault(x => x.Telefone == telefone && x.ProprietarioId == proprietarioId);

        return registro is null ? null : Mapear(registro);
    }

    public void Salvar(SessaoConversaDto sessao)
    {
        var registro = _context.SessoesConversa
            .FirstOrDefault(x => x.Telefone == sessao.Telefone && x.ProprietarioId == sessao.ProprietarioId);

        if (registro is null)
        {
            _context.SessoesConversa.Add(new SessaoConversaRegistro
            {
                Telefone = sessao.Telefone,
                ProprietarioId = sessao.ProprietarioId,
                Passo = sessao.Passo.ToString(),
                ClienteId = sessao.ClienteId,
                ProcedimentoId = sessao.ProcedimentoId,
                SlotHorarioId = sessao.SlotHorarioId,
                NomePendente = sessao.NomePendente,
                AtualizadoEm = sessao.AtualizadoEm,
                ExpiraEm = sessao.ExpiraEm
            });
        }
        else
        {
            registro.Passo = sessao.Passo.ToString();
            registro.ClienteId = sessao.ClienteId;
            registro.ProcedimentoId = sessao.ProcedimentoId;
            registro.SlotHorarioId = sessao.SlotHorarioId;
            registro.NomePendente = sessao.NomePendente;
            registro.AtualizadoEm = sessao.AtualizadoEm;
            registro.ExpiraEm = sessao.ExpiraEm;
        }

        _context.SaveChanges();
    }

    public void RemoverExpiradas(DateTime antesDe)
    {
        var expiradas = _context.SessoesConversa.Where(x => x.ExpiraEm < antesDe).ToList();
        if (expiradas.Count == 0)
            return;

        _context.SessoesConversa.RemoveRange(expiradas);
        _context.SaveChanges();
    }

    private static SessaoConversaDto Mapear(SessaoConversaRegistro registro)
        => new()
        {
            Telefone = registro.Telefone,
            ProprietarioId = registro.ProprietarioId,
            Passo = Enum.Parse<PassoFluxoBot>(registro.Passo),
            ClienteId = registro.ClienteId,
            ProcedimentoId = registro.ProcedimentoId,
            SlotHorarioId = registro.SlotHorarioId,
            NomePendente = registro.NomePendente,
            AtualizadoEm = registro.AtualizadoEm,
            ExpiraEm = registro.ExpiraEm
        };
}
