using Finances.Models;
using Microsoft.EntityFrameworkCore;

namespace Finances;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<WalletEntity> Wallets { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<TransactionEntity> Transactions { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<WalletEntity>()
            .HasOne(w => w.User)
            .WithMany(u => u.Wallets)
            .HasForeignKey(w => w.UserId);
        
        modelBuilder.Entity<TransactionEntity>()
            .HasOne(t => t.Wallet)
            .WithMany(w => w.Transactions)
            .HasForeignKey(t => t.WalletId);
        
        modelBuilder.Entity<TransactionEntity>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId);
        
        modelBuilder.Entity<CategoryEntity>()
            .HasOne(c => c.ParentCategory)
            .WithMany(p => p.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .IsRequired(false);
        
        modelBuilder.Entity<TransactionEntity>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<WalletEntity>()
            .Property(w => w.Balance)
            .HasPrecision(18, 2);
    }
}