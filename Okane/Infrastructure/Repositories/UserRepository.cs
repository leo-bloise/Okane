using Microsoft.EntityFrameworkCore;
using Okane.Authentication;
using Okane.Authentication.Repositories;

namespace Okane.Infrastructure.Repositories;

public class UserRepository(OkaneDbContext okaneDbContext): IUserRepository
{
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return okaneDbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return okaneDbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User> SaveUserAsync(User user, CancellationToken cancellationToken)
    {
        await okaneDbContext.AddAsync(user, cancellationToken);
        await okaneDbContext.SaveChangesAsync(cancellationToken);

        return user;
    }
}
