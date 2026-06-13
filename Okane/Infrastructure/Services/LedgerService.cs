using Okane.Accounts.Repositories;
using Okane.Core.Data;
using Okane.Core.Exceptions;
using Okane.Ledger;
using Okane.Ledger.Repositories;
using Okane.Ledger.Requests;
using Okane.Ledger.Services;

namespace Okane.Infrastructure.Services;

public class LedgerService(ILedgerRepository ledgerRepository, IAccountRepository accountRepository) : ILedgerService
{
    public async Task<Transaction> CreateTransactionAsync(ICreateTransactionRequest request, CancellationToken cancellationToken)
    {
        if (request.FromAccountId == request.ToAccountId) throw new DomainException("Transaction must interaction with two different accounts", 422);

        if (!await accountRepository.ExistAllAsync(request.UserId, cancellationToken, request.FromAccountId, request.ToAccountId))
            throw new DomainException($"Accounts are invalid. Please, provide an existing account for transactions.", 422);

        Transaction transaction = new Transaction()
        {
            Amount = request.Amount,
            Description = request.Description,
            FromAccountId = request.FromAccountId,
            ToAccountId = request.ToAccountId,
            UserId = request.UserId,
            OccuredAt = request.OccuredAt
        };

        return await ledgerRepository.CreateTransactionAsync(transaction, cancellationToken);
    }

    public Task<Page<Transaction>> GetPagedTransaction(int userId, int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        return ledgerRepository.GetPageAsync(userId, pageIndex, pageSize, cancellationToken);    
    }

    public Task<Transaction?> GetTransactionAsync(int id, int userId, CancellationToken cancellationToken)
    {
        return ledgerRepository.FindByIdAsync(id, userId, cancellationToken);
    }
}
