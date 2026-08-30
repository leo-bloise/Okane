namespace Okane.Transaction.Application.Interfaces;

public interface ITransactionRepository
{
    Task<Domain.Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Domain.Transaction>> GetByWalletAsync(Guid walletId, CancellationToken cancellationToken = default);

    Task<bool> ExistsForWalletAsync(Guid walletId, CancellationToken cancellationToken = default);

    Task AddAsync(Domain.Transaction transaction, CancellationToken cancellationToken = default);

    Task<PagedResult<Domain.Transaction>> GetPagedForOwnerAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default);
}
