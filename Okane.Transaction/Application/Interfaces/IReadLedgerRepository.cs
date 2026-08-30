using Okane.Transaction.Application.ReadModels;

namespace Okane.Transaction.Application.Interfaces;

public interface IReadLedgerRepository
{
    Task<LedgerPage> GetPagedForOwnerAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default);
}
