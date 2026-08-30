using Npgsql;
using Okane.Kernel;
using Okane.Transaction.Application.Interfaces;
using Okane.Wallet.Application;
using Okane.Wallet.Application.Interfaces;
using Okane.Wallet.Domain;

namespace Okane.Api.Infrastructure.Persistence;

public sealed class WalletRepository(IDbConnectionProvider<NpgsqlConnection> dbConnectionProvider) : IWalletRepository, IWalletLookup
{
    public async Task<Okane.Wallet.Domain.Wallet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var activity = DatabaseObservability.Source.StartActivity("database.get_by_id.wallet");

        var connection = await dbConnectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)dbConnectionProvider.CurrentTransaction;
        command.CommandText = "SELECT id, owner_id, name, kind, status, created_at FROM wallets WHERE id = @id";
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<IReadOnlyCollection<Okane.Wallet.Domain.Wallet>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        using var activity = DatabaseObservability.Source.StartActivity("database.get_by_owner.wallet");

        var connection = await dbConnectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)dbConnectionProvider.CurrentTransaction;
        command.CommandText = "SELECT id, owner_id, name, kind, status, created_at FROM wallets WHERE owner_id = @ownerId";
        command.Parameters.AddWithValue("ownerId", ownerId);

        var wallets = new List<Okane.Wallet.Domain.Wallet>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            wallets.Add(Map(reader));
        }

        return wallets;
    }

    public async Task<PagedResult<Okane.Wallet.Domain.Wallet>> GetPagedForOwnerAsync(
        Guid ownerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        using var activity = DatabaseObservability.Source.StartActivity("database.get_paged_for_owner.wallet");

        var connection = await dbConnectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)dbConnectionProvider.CurrentTransaction;
        command.CommandText = """
            SELECT id, owner_id, name, kind, status, created_at, COUNT(*) OVER() AS total_count
            FROM wallets
            WHERE owner_id = @ownerId
            ORDER BY created_at DESC
            LIMIT @pageSize OFFSET @offset
            """;
        command.Parameters.AddWithValue("ownerId", ownerId);
        command.Parameters.AddWithValue("pageSize", pageSize);
        command.Parameters.AddWithValue("offset", (page - 1) * pageSize);

        var wallets = new List<Okane.Wallet.Domain.Wallet>();
        var totalCount = 0;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            wallets.Add(Map(reader));
            totalCount = reader.GetInt32(6);
        }

        return new PagedResult<Okane.Wallet.Domain.Wallet>(wallets, page, pageSize, totalCount);
    }

    public async Task AddAsync(Okane.Wallet.Domain.Wallet wallet, CancellationToken cancellationToken = default)
    {
        using var activity = DatabaseObservability.Source.StartActivity("database.add.wallet");

        var connection = await dbConnectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)dbConnectionProvider.CurrentTransaction;
        command.CommandText = """
            INSERT INTO wallets (id, owner_id, name, kind, status, created_at)
            VALUES (@id, @ownerId, @name, @kind, @status, @createdAt)
            """;
        command.Parameters.AddWithValue("id", wallet.Id);
        command.Parameters.AddWithValue("ownerId", wallet.OwnerId);
        command.Parameters.AddWithValue("name", wallet.Name);
        command.Parameters.AddWithValue("kind", wallet.Kind.ToString());
        command.Parameters.AddWithValue("status", wallet.Status.ToString());
        command.Parameters.AddWithValue("createdAt", wallet.CreatedAt);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task UpdateAsync(Okane.Wallet.Domain.Wallet wallet, CancellationToken cancellationToken = default)
    {
        using var activity = DatabaseObservability.Source.StartActivity("database.update.wallet");

        var connection = await dbConnectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)dbConnectionProvider.CurrentTransaction;
        command.CommandText = """
            UPDATE wallets
            SET name = @name, status = @status
            WHERE id = @id
            """;
        command.Parameters.AddWithValue("id", wallet.Id);
        command.Parameters.AddWithValue("name", wallet.Name);
        command.Parameters.AddWithValue("status", wallet.Status.ToString());

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var activity = DatabaseObservability.Source.StartActivity("database.delete.wallet");

        var connection = await dbConnectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)dbConnectionProvider.CurrentTransaction;
        command.CommandText = "DELETE FROM wallets WHERE id = @id";
        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<WalletInfo?> GetWalletInfoAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var wallet = await GetByIdAsync(walletId, cancellationToken);
        return wallet is null ? null : new WalletInfo(wallet.Id, wallet.OwnerId, wallet.Status == WalletStatus.Active);
    }

    private static Okane.Wallet.Domain.Wallet Map(NpgsqlDataReader reader) => Okane.Wallet.Domain.Wallet.FromPersistence(
        reader.GetGuid(0),
        reader.GetGuid(1),
        reader.GetString(2),
        Enum.Parse<WalletKind>(reader.GetString(3)),
        Enum.Parse<WalletStatus>(reader.GetString(4)),
        reader.GetFieldValue<DateTimeOffset>(5));
}
