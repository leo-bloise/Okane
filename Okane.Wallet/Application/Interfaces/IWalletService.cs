namespace Okane.Wallet.Application.Interfaces;

public interface IWalletService
{
    Task<Domain.Wallet> CreateStandardWalletAsync(Guid ownerId, string name, CancellationToken cancellationToken = default);

    Task<Domain.Wallet> CreateExternalWalletAsync(Guid ownerId, CancellationToken cancellationToken = default);

    Task<Domain.Wallet?> GetWalletAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Domain.Wallet>> GetWalletsForOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);

    Task RenameWalletAsync(Guid id, string name, CancellationToken cancellationToken = default);

    Task ArchiveWalletAsync(Guid id, CancellationToken cancellationToken = default);

    Task ReactivateWalletAsync(Guid id, CancellationToken cancellationToken = default);

    Task DeleteWalletAsync(Guid id, CancellationToken cancellationToken = default);
}
