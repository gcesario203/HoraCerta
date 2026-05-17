using HoraCerta.Aplicacao.Autenticacao;
using HoraCerta.Infaestrutura.Persistencia;
using HoraCerta.Infaestrutura.Persistencia.Registros;
using Microsoft.EntityFrameworkCore;

namespace HoraCerta.Infaestrutura.Repositorio;

public class EfCredencialProprietarioRepositorio : ICredencialProprietarioRepositorio
{
    private readonly HoraCertaDbContext _context;

    public EfCredencialProprietarioRepositorio(HoraCertaDbContext context)
    {
        _context = context;
    }

    public void Salvar(string proprietarioId, string email, string passwordHash)
    {
        var registro = _context.CredenciaisProprietario.Find(proprietarioId);

        if (registro is null)
        {
            _context.CredenciaisProprietario.Add(new CredencialProprietarioRegistro
            {
                ProprietarioId = proprietarioId,
                Email = email.ToLowerInvariant(),
                PasswordHash = passwordHash
            });
        }
        else
        {
            registro.Email = email.ToLowerInvariant();
            registro.PasswordHash = passwordHash;
        }

        _context.SaveChanges();
    }

    public CredencialProprietario? BuscarPorEmail(string email)
    {
        var registro = _context.CredenciaisProprietario
            .AsNoTracking()
            .FirstOrDefault(x => x.Email == email.ToLowerInvariant());

        return registro is null
            ? null
            : new CredencialProprietario(registro.ProprietarioId, registro.Email, registro.PasswordHash);
    }

    public bool Existe(string proprietarioId)
        => _context.CredenciaisProprietario.Any(x => x.ProprietarioId == proprietarioId);
}
