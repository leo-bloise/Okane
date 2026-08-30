using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Npgsql;
using Okane.Kernel;
using Okane.Transaction.Application.Exceptions;
using Okane.Transaction.Application.Interfaces;

namespace Okane.Transaction.Application;

public sealed class TransactionService(
    ITransactionRepository transactionRepository,
    IReadLedgerRepository readLedgerRepository,
    IWalletLookup walletLookup,
    IDbConnectionProvider<NpgsqlConnection> dbConnectionProvider,
    ILogger<TransactionService> logger
) : ITransactionService
{
    private const int MinPageSize = 1;
    private const int MaxPageSize = 100;

    public async Task<Domain.Transaction> RecordTransactionAsync(
        Guid ownerId,
        Guid fromWalletId,
        Guid toWalletId,
        decimal amount,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        using var activity = TransactionObservability.ActivitySource.StartActivity("transaction.record");

        await dbConnectionProvider.BeginTransactionAsync(cancellationToken);
        try
        {
            var fromWallet = await walletLookup.GetWalletInfoAsync(fromWalletId, cancellationToken);
            var toWallet = await walletLookup.GetWalletInfoAsync(toWalletId, cancellationToken);

            if (fromWallet is null || toWallet is null)
            {
                logger.LogWarning("Transaction rejected: wallet not found ({FromWalletId} / {ToWalletId}).", fromWalletId, toWalletId);
                activity?.SetStatus(ActivityStatusCode.Error, "Wallet not found.");
                throw new WalletNotFoundException(fromWalletId, toWalletId);
            }

            if (fromWallet.OwnerId != ownerId || toWallet.OwnerId != ownerId)
            {
                logger.LogWarning("Transaction rejected: owner {OwnerId} does not own both wallets.", ownerId);
                activity?.SetStatus(ActivityStatusCode.Error, "Wallets not owned by caller.");
                throw new WalletOwnershipMismatchException(ownerId);
            }

            if (!fromWallet.IsActive || !toWallet.IsActive)
            {
                logger.LogWarning("Transaction rejected: one or both wallets are not active.");
                activity?.SetStatus(ActivityStatusCode.Error, "Wallet not active.");
                throw new InactiveWalletException();
            }

            var transaction = Domain.Transaction.Record(fromWalletId, toWalletId, ownerId, amount, description);
            await transactionRepository.AddAsync(transaction, cancellationToken);

            await dbConnectionProvider.CommitAsync(cancellationToken);

            activity?.SetTag("transaction.id", transaction.Id);
            logger.LogInformation("Transaction {TransactionId} recorded for owner {OwnerId}.", transaction.Id, ownerId);

            return transaction;
        }
        catch
        {
            await dbConnectionProvider.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public Task<Domain.Transaction?> GetTransactionAsync(Guid id, CancellationToken cancellationToken = default)
        => transactionRepository.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyCollection<Domain.Transaction>> GetTransactionsForWalletAsync(Guid walletId, CancellationToken cancellationToken = default)
        => transactionRepository.GetByWalletAsync(walletId, cancellationToken);

    public async Task<decimal> GetWalletBalanceAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var transactions = await transactionRepository.GetByWalletAsync(walletId, cancellationToken);

        return transactions.Sum(transaction => transaction.ToWalletId == walletId ? transaction.Amount : -transaction.Amount);
    }

    public Task<ReadModels.LedgerPage> GetLedgerForOwnerAsync(
        Guid ownerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        using var activity = TransactionObservability.ActivitySource.StartActivity("transaction.get_ledger_for_owner");

        var clampedPage = Math.Max(page, 1);
        var clampedPageSize = Math.Clamp(pageSize, MinPageSize, MaxPageSize);

        activity?.SetTag("ledger.page", clampedPage);
        activity?.SetTag("ledger.page_size", clampedPageSize);

        return readLedgerRepository.GetFilteredPagedForOwnerAsync(ownerId, clampedPage, clampedPageSize, cancellationToken);
    }
}
