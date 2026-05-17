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
    }
}
