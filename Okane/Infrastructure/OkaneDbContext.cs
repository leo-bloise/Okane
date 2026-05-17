using Microsoft.EntityFrameworkCore;
using Okane.Authentication;

namespace Okane.Infrastructure;

public class OkaneDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public OkaneDbContext(DbContextOptions options) : base(options)
    {
    }
}
