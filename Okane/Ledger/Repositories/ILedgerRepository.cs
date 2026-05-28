namespace Okane.Ledger.Repositories;

public interface ILedgerRepository
{
    public Task<Transaction?> FindByIdAsync(int id, int userId, CancellationToken cancellationToken);
    public Task<Transaction> CreateTransactionAsync(Transaction transaction, CancellationToken cancellationToken);
}