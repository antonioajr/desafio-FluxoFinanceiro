using FluxoCaixaApp.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FluxoCaixaApp.Api.Data;

public class AppDbContext : DbContext
{
    public DbSet<Lancamento> Lancamentos => Set<Lancamento>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lancamento>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Descricao).IsRequired();
            entity.Property(x => x.Categoria).IsRequired();
            entity.Property(x => x.Valor).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Tipo).IsRequired();
            entity.Property(x => x.Payload).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
