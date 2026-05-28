using Okane.Ledger.Requests;

namespace Okane.Ledger.Services;

public interface ILedgerService
{
    public Task<Transaction?> GetTransactionAsync(int id, int userId, CancellationToken cancellationToken);
    public Task<Transaction> CreateTransactionAsync(ICreateTransactionRequest request, CancellationToken cancellationToken);
}
