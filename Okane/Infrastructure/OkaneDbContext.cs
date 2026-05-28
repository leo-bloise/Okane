using Microsoft.EntityFrameworkCore;
using Okane.Accounts;
using Okane.Authentication;
using Okane.Ledger;

namespace Okane.Infrastructure;

public class OkaneDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<Account> Accounts { get; set; }

    public DbSet<Transaction> Ledger { get; set; }

    public OkaneDbContext(DbContextOptions options) : base(options)
    {
    }
}
