using Microsoft.EntityFrameworkCore;
using Okane.Accounts;
using Okane.Core.Data;
using Okane.Ledger;
using Okane.Ledger.Repositories;

namespace Okane.Infrastructure.Repositories;

public class LedgerRepository(OkaneDbContext okaneDbContext) : ILedgerRepository
{
    public async Task<Transaction> CreateTransactionAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        await okaneDbContext.Ledger.AddAsync(transaction, cancellationToken);

        await okaneDbContext.SaveChangesAsync(cancellationToken);

        return transaction;
    }

    public Task<Transaction?> FindByIdAsync(int id, int userId, CancellationToken cancellationToken)
    {
        return okaneDbContext.Ledger.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task<Page<Transaction>> GetPageAsync(int userId, int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        IOrderedQueryable<Transaction> transactions = okaneDbContext.Ledger
            .Where(t => t.User.Id == userId)
            .OrderBy(t => t.Description);

        int totalTransactions = await transactions.CountAsync(cancellationToken);

        List<Transaction> page = await transactions.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new Page<Transaction>
        {
            Items = page,
            TotalPages = (int) Math.Ceiling(totalTransactions / (double) pageSize),
            PageSize = pageSize,
            PageIndex = pageIndex
        };
    }
}
