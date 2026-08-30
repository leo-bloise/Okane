using Okane.Transaction.Application.ReadModels;

namespace Okane.Transaction.Application.Interfaces;

public interface IReadLedgerRepository
{
    Task<LedgerPage> GetFilteredPagedForOwnerAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default);
}
