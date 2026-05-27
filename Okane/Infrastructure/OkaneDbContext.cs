using Microsoft.EntityFrameworkCore;
using Okane.Accounts;
using Okane.Authentication;

namespace Okane.Infrastructure;

public class OkaneDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<Account> Accounts { get; set; }

    public OkaneDbContext(DbContextOptions options) : base(options)
    {
    }
}
