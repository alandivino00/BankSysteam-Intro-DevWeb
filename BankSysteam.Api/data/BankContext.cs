using BankSysteam.Api.Controllers;
using BankSysteam.Api.models;
using Microsoft.EntityFrameworkCore;

namespace BankSysteam.Api.data
{
    public class BankContext : DbContext
    {
       public BankContext(DbContextOptions<BankContext> options) : base(options)
       {
        
       }

        public DbSet<Conta> Contas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Conta>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.Numero).IsRequired().HasMaxLength(20);
                //entity.Property(e => e.Titular).IsRequired();
                entity.Property(e => e.Saldo).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Tipo).IsRequired();
                entity.Property(e => e.DataCriacao).IsRequired();
                entity.Property(e => e.Status).IsRequired();
            });
        }
    }
}
