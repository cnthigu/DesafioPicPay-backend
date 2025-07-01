using Microsoft.EntityFrameworkCore;
using PicPayClone.Models;

namespace PicPayClone.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Payer)
                .WithMany()
                .HasForeignKey(t => t.PayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Payee)
                .WithMany()
                .HasForeignKey(t => t.PayeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
