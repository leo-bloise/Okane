using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Okane.Wallet.Application.Exceptions;
using Okane.Wallet.Application.Interfaces;

namespace Okane.Wallet.Application;

public sealed class WalletService(
    IWalletRepository walletRepository,
    IWalletActivityChecker walletActivityChecker,
    ILogger<WalletService> logger
) : IWalletService
{
    private const int MinPageSize = 1;
    private const int MaxPageSize = 100;

    public async Task<Domain.Wallet> CreateStandardWalletAsync(Guid ownerId, string name, CancellationToken cancellationToken = default)
    {
        using var activity = WalletObservability.ActivitySource.StartActivity("wallet.create_standard");

        var trimmedName = name.Trim();
        if (await walletRepository.ExistsByOwnerAndNameAsync(ownerId, trimmedName, cancellationToken))
        {
            logger.LogWarning("Wallet creation rejected: owner {OwnerId} already has a wallet named '{Name}'.", ownerId, trimmedName);
            activity?.SetStatus(ActivityStatusCode.Error, "Wallet name already exists.");
            throw new WalletNameAlreadyExistsException(ownerId, trimmedName);
        }

        var wallet = Domain.Wallet.CreateStandard(ownerId, name);
        await walletRepository.AddAsync(wallet, cancellationToken);

        activity?.SetTag("wallet.id", wallet.Id);
        logger.LogInformation("Standard wallet {WalletId} created for owner {OwnerId}.", wallet.Id, ownerId);

        return wallet;
    }

    public async Task<Domain.Wallet> CreateExternalWalletAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        using var activity = WalletObservability.ActivitySource.StartActivity("wallet.create_external");

        var existingWallets = await walletRepository.GetByOwnerAsync(ownerId, cancellationToken);
        if (existingWallets.Any(wallet => wallet.Kind == Domain.WalletKind.External))
        {
            logger.LogWarning("External wallet creation rejected: owner {OwnerId} already has one.", ownerId);
            activity?.SetStatus(ActivityStatusCode.Error, "External wallet already exists.");
            throw new ExternalWalletAlreadyExistsException(ownerId);
        }

        var wallet = Domain.Wallet.CreateExternal(ownerId);
        await walletRepository.AddAsync(wallet, cancellationToken);

        activity?.SetTag("wallet.id", wallet.Id);
        logger.LogInformation("External wallet {WalletId} created for owner {OwnerId}.", wallet.Id, ownerId);

        return wallet;
    }

    public Task<Domain.Wallet?> GetWalletAsync(Guid id, CancellationToken cancellationToken = default)
        => walletRepository.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyCollection<Domain.Wallet>> GetWalletsForOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
        => walletRepository.GetByOwnerAsync(ownerId, cancellationToken);

    public Task<PagedResult<Domain.Wallet>> GetWalletsForOwnerAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        using var activity = WalletObservability.ActivitySource.StartActivity("wallet.get_paged_for_owner");

        var clampedPage = Math.Max(page, 1);
        var clampedPageSize = Math.Clamp(pageSize, MinPageSize, MaxPageSize);

        activity?.SetTag("wallet.page", clampedPage);
        activity?.SetTag("wallet.page_size", clampedPageSize);

        return walletRepository.GetPagedForOwnerAsync(ownerId, clampedPage, clampedPageSize, cancellationToken);
    }

    public async Task RenameWalletAsync(Guid id, string name, CancellationToken cancellationToken = default)
    {
        using var activity = WalletObservability.ActivitySource.StartActivity("wallet.rename");

        var wallet = await walletRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new WalletNotFoundException(id);

        wallet.Rename(name);
        logger.LogInformation("Wallet {WalletId} renamed.", wallet.Id);
        await walletRepository.UpdateAsync(wallet, cancellationToken);
    }

    public async Task ArchiveWalletAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var activity = WalletObservability.ActivitySource.StartActivity("wallet.archive");

        var wallet = await walletRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new WalletNotFoundException(id);

        wallet.Archive();
        logger.LogInformation("Wallet {WalletId} archived.", wallet.Id);
        await walletRepository.UpdateAsync(wallet, cancellationToken);
    }

    public async Task ReactivateWalletAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var activity = WalletObservability.ActivitySource.StartActivity("wallet.reactivate");

        var wallet = await walletRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new WalletNotFoundException(id);

        wallet.Reactivate();
        logger.LogInformation("Wallet {WalletId} reactivated.", wallet.Id);
        await walletRepository.UpdateAsync(wallet, cancellationToken);
    }

    public async Task DeleteWalletAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var activity = WalletObservability.ActivitySource.StartActivity("wallet.delete");

        var wallet = await walletRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new WalletNotFoundException(id);

        if (wallet.Kind == Domain.WalletKind.External)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Cannot delete the External wallet.");
            throw new ExternalWalletModificationNotAllowedException();
        }

        if (await walletActivityChecker.HasTransactionsAsync(id, cancellationToken))
        {
            logger.LogWarning("Delete rejected: wallet {WalletId} has recorded transactions.", id);
            activity?.SetStatus(ActivityStatusCode.Error, "Wallet has transactions.");
            throw new WalletHasTransactionsException(id);
        }

        logger.LogInformation("Wallet {WalletId} deleted.", id);
        await walletRepository.DeleteAsync(id, cancellationToken);
    }
}
