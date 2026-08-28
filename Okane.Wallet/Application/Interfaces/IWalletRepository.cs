namespace Okane.Wallet.Application.Interfaces;

public interface IWalletRepository
{
    Task<Domain.Wallet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Domain.Wallet>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);

    Task AddAsync(Domain.Wallet wallet, CancellationToken cancellationToken = default);

    Task UpdateAsync(Domain.Wallet wallet, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
