using Npgsql;
using Okane.Kernel;
using Okane.Transaction.Application;
using Okane.Transaction.Application.Interfaces;

namespace Okane.Api.Infrastructure.Persistence;

public sealed class TransactionalTransactionService(
    TransactionService inner,
    IDbConnectionProvider<NpgsqlConnection> dbConnectionProvider) : ITransactionService
{
    public async Task<Okane.Transaction.Domain.Transaction> RecordTransactionAsync(
        Guid ownerId,
        Guid fromWalletId,
        Guid toWalletId,
        decimal amount,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        await dbConnectionProvider.BeginTransactionAsync(cancellationToken);
        try
        {
            var transaction = await inner.RecordTransactionAsync(ownerId, fromWalletId, toWalletId, amount, description, cancellationToken);
            await dbConnectionProvider.CommitAsync(cancellationToken);
            return transaction;
        }
        catch
        {
            await dbConnectionProvider.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public Task<Okane.Transaction.Domain.Transaction?> GetTransactionAsync(Guid id, CancellationToken cancellationToken = default)
        => inner.GetTransactionAsync(id, cancellationToken);

    public Task<IReadOnlyCollection<Okane.Transaction.Domain.Transaction>> GetTransactionsForWalletAsync(Guid walletId, CancellationToken cancellationToken = default)
        => inner.GetTransactionsForWalletAsync(walletId, cancellationToken);

    public Task<decimal> GetWalletBalanceAsync(Guid walletId, CancellationToken cancellationToken = default)
        => inner.GetWalletBalanceAsync(walletId, cancellationToken);

    public Task<PagedResult<Okane.Transaction.Domain.Transaction>> GetTransactionsForOwnerAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default)
        => inner.GetTransactionsForOwnerAsync(ownerId, page, pageSize, cancellationToken);

    public Task<Okane.Transaction.Application.ReadModels.LedgerPage> GetLedgerForOwnerAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default)
        => inner.GetLedgerForOwnerAsync(ownerId, page, pageSize, cancellationToken);
}
