using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions)
    : IdentityDbContext<AppUser>(dbContextOptions)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        List<IdentityRole> roles =
        [
            new IdentityRole { Name = AppRoles.Admin, NormalizedName = AppRoles.Admin.ToUpper()},
            new IdentityRole { Name = AppRoles.DataAdmin, NormalizedName = AppRoles.DataAdmin.ToUpper()},
            new IdentityRole { Name = AppRoles.DataAnalyst, NormalizedName = AppRoles.DataAnalyst.ToUpper()}
        ];
        modelBuilder.Entity<IdentityRole>().HasData(roles);
        
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.SourceAccount)
            .WithMany(a => a.SourceTransactions)
            .HasForeignKey(t => t.SourceAccountId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.DestinationAccount)
            .WithMany(a => a.DestinationTransactions)
            .HasForeignKey(t => t.DestinationAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}