using Microsoft.EntityFrameworkCore;
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
}
