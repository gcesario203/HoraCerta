using HoraCerta.Infaestrutura.Persistencia.Registros;
using Microsoft.EntityFrameworkCore;

namespace HoraCerta.Infaestrutura.Persistencia;

public class HoraCertaDbContext : DbContext
{
    public HoraCertaDbContext(DbContextOptions<HoraCertaDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProprietarioRegistro> Proprietarios => Set<ProprietarioRegistro>();

    public DbSet<ClienteRegistro> Clientes => Set<ClienteRegistro>();

    public DbSet<LembreteRegistro> Lembretes => Set<LembreteRegistro>();

    public DbSet<CredencialProprietarioRegistro> CredenciaisProprietario => Set<CredencialProprietarioRegistro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProprietarioRegistro>(entity =>
        {
            entity.ToTable("proprietarios");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(36);
            entity.Property(x => x.Conteudo).IsRequired();
        });

        modelBuilder.Entity<ClienteRegistro>(entity =>
        {
            entity.ToTable("clientes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(36);
            entity.Property(x => x.Conteudo).IsRequired();
        });

        modelBuilder.Entity<LembreteRegistro>(entity =>
        {
            entity.ToTable("lembretes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(36);
            entity.HasIndex(x => x.AgendamentoId);
            entity.HasIndex(x => new { x.Status, x.EnviarEm });
        });

        modelBuilder.Entity<CredencialProprietarioRegistro>(entity =>
        {
            entity.ToTable("credenciais_proprietario");
            entity.HasKey(x => x.ProprietarioId);
            entity.Property(x => x.ProprietarioId).HasMaxLength(36);
            entity.Property(x => x.Email).HasMaxLength(256).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.PasswordHash).IsRequired();
        });
    }
}
