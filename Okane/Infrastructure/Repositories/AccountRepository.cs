using Microsoft.EntityFrameworkCore;
using Okane.Accounts;
using Okane.Accounts.Repositories;
using Okane.Core.Data;
using System.Collections.Immutable;

namespace Okane.Infrastructure.Repositories;

public class AccountRepository(OkaneDbContext okaneDbContext) : IAccountRepository
{
    public Task<bool> ExistsByNameAsync(string name, int userId, CancellationToken cancellationToken)
    {
        return okaneDbContext.Accounts.AnyAsync(a => a.Name == name && a.User.Id == userId, cancellationToken);
    }

    public async Task<bool> ExistAllAsync(int userId, CancellationToken cancellationToken, params int[] ids)
    {
        return await okaneDbContext
            .Accounts
            .Where(a => ids.Contains(a.Id))
            .CountAsync(cancellationToken) == ids.Length;
    }

    public Task<Account?> FindByIdAsync(int id, int userId, CancellationToken cancellationToken)
    {
        return okaneDbContext.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.User.Id == userId, cancellationToken);
    }

    public async Task<Page<Account>> GetPageAsync(int userId, int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        IOrderedQueryable<Account> accounts = okaneDbContext.Accounts
            .Where(a => a.User.Id == userId)
            .OrderBy(a => a.Name);

        int totalAccounts = await accounts.CountAsync(cancellationToken);

        List<Account> page = await accounts.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new Page<Account>
        {
            Items = page,
            TotalPages = (int)Math.Ceiling(totalAccounts / (double)pageSize),
            PageSize = pageSize,
              PageIndex = pageIndex
        };
    }

    public async Task<Account> SaveAccountAsync(Account account, CancellationToken cancellationToken)
    {
        await okaneDbContext.Accounts.AddAsync(account, cancellationToken);

        await okaneDbContext.SaveChangesAsync(cancellationToken);

        return account;
    }

    public async Task<IEnumerable<Account>> SearchAsync(string query, CancellationToken cancellationToken)
    {
        return await okaneDbContext.Accounts.Where(a => EF.Functions.ILike(a.Name, $"%{query}%"))
            .ToListAsync(cancellationToken);
    }
}
