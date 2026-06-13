using Okane.Core.Data;

namespace Okane.Ledger.Repositories;

public interface ILedgerRepository
{
    public Task<Transaction?> FindByIdAsync(int id, int userId, CancellationToken cancellationToken);
    public Task<Transaction> CreateTransactionAsync(Transaction transaction, CancellationToken cancellationToken);
    public Task<Page<Transaction>> GetPageAsync(int userId, int pageIndex, int pageSize, CancellationToken cancellationToken);
}