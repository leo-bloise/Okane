using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Okane.Wallet.Application.Interfaces;

namespace Okane.Wallet.Application;

public sealed class WalletService(
    IWalletRepository walletRepository,
    IWalletActivityChecker walletActivityChecker,
    ILogger<WalletService> logger) : IWalletService
{
    public async Task<Domain.Wallet> CreateStandardWalletAsync(Guid ownerId, string name, CancellationToken cancellationToken = default)
    {
        using var activity = WalletObservability.ActivitySource.StartActivity("wallet.create_standard");

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
            throw new InvalidOperationException("This owner already has an External Wallet.");
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

    public async Task RenameWalletAsync(Guid id, string name, CancellationToken cancellationToken = default)
    {
        using var activity = WalletObservability.ActivitySource.StartActivity("wallet.rename");

        var wallet = await walletRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Wallet not found.");

        wallet.Rename(name);
        logger.LogInformation("Wallet {WalletId} renamed.", wallet.Id);
        await walletRepository.UpdateAsync(wallet, cancellationToken);
    }

    public async Task ArchiveWalletAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var activity = WalletObservability.ActivitySource.StartActivity("wallet.archive");

        var wallet = await walletRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Wallet not found.");

        wallet.Archive();
        logger.LogInformation("Wallet {WalletId} archived.", wallet.Id);
        await walletRepository.UpdateAsync(wallet, cancellationToken);
    }

    public async Task ReactivateWalletAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var activity = WalletObservability.ActivitySource.StartActivity("wallet.reactivate");

        var wallet = await walletRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Wallet not found.");

        wallet.Reactivate();
        logger.LogInformation("Wallet {WalletId} reactivated.", wallet.Id);
        await walletRepository.UpdateAsync(wallet, cancellationToken);
    }

    public async Task DeleteWalletAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var activity = WalletObservability.ActivitySource.StartActivity("wallet.delete");

        var wallet = await walletRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Wallet not found.");

        if (wallet.Kind == Domain.WalletKind.External)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Cannot delete the External wallet.");
            throw new InvalidOperationException("The External Wallet cannot be renamed, archived, or deleted.");
        }

        if (await walletActivityChecker.HasTransactionsAsync(id, cancellationToken))
        {
            logger.LogWarning("Delete rejected: wallet {WalletId} has recorded transactions.", id);
            activity?.SetStatus(ActivityStatusCode.Error, "Wallet has transactions.");
            throw new InvalidOperationException("A wallet with recorded transactions cannot be deleted; archive it instead.");
        }

        logger.LogInformation("Wallet {WalletId} deleted.", id);
        await walletRepository.DeleteAsync(id, cancellationToken);
    }
}
