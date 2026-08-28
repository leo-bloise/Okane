namespace Okane.Transaction.Application.Interfaces;

public interface ITransactionService
{
    Task<Domain.Transaction> RecordTransactionAsync(
        Guid fromWalletId,
        Guid toWalletId,
        decimal amount,
        string? description = null,
        CancellationToken cancellationToken = default);

    Task<Domain.Transaction?> GetTransactionAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Domain.Transaction>> GetTransactionsForWalletAsync(Guid walletId, CancellationToken cancellationToken = default);

    Task<decimal> GetWalletBalanceAsync(Guid walletId, CancellationToken cancellationToken = default);
}
