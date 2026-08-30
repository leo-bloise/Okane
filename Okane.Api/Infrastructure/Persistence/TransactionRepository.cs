using Npgsql;
using Okane.Kernel;
using Okane.Transaction.Application;
using Okane.Transaction.Application.Interfaces;
using Okane.Wallet.Application.Interfaces;

namespace Okane.Api.Infrastructure.Persistence;

public sealed class TransactionRepository(IDbConnectionProvider<NpgsqlConnection> dbConnectionProvider) : ITransactionRepository, IWalletActivityChecker
{
    private const string SelectColumns = "id, from_wallet_id, to_wallet_id, owner_id, amount, description, recorded_at, created_at";

    public async Task<Okane.Transaction.Domain.Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var activity = DatabaseObservability.Source.StartActivity("database.get_by_id.transaction");

        var connection = await dbConnectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)dbConnectionProvider.CurrentTransaction;
        command.CommandText = $"SELECT {SelectColumns} FROM ledger WHERE id = @id";
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<IReadOnlyCollection<Okane.Transaction.Domain.Transaction>> GetByWalletAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        using var activity = DatabaseObservability.Source.StartActivity("database.get_by_wallet.transaction");

        var connection = await dbConnectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)dbConnectionProvider.CurrentTransaction;
        command.CommandText = $"""
            SELECT {SelectColumns} FROM ledger
            WHERE from_wallet_id = @walletId OR to_wallet_id = @walletId
            ORDER BY recorded_at DESC
            """;
        command.Parameters.AddWithValue("walletId", walletId);

        var transactions = new List<Okane.Transaction.Domain.Transaction>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            transactions.Add(Map(reader));
        }

        return transactions;
    }

    public async Task<bool> ExistsForWalletAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        using var activity = DatabaseObservability.Source.StartActivity("database.exists_for_wallet.transaction");

        var connection = await dbConnectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)dbConnectionProvider.CurrentTransaction;
        command.CommandText = "SELECT EXISTS(SELECT 1 FROM ledger WHERE from_wallet_id = @walletId OR to_wallet_id = @walletId)";
        command.Parameters.AddWithValue("walletId", walletId);

        return (bool)(await command.ExecuteScalarAsync(cancellationToken))!;
    }

    public Task<bool> HasTransactionsAsync(Guid walletId, CancellationToken cancellationToken = default)
        => ExistsForWalletAsync(walletId, cancellationToken);

    public async Task AddAsync(Okane.Transaction.Domain.Transaction transaction, CancellationToken cancellationToken = default)
    {
        using var activity = DatabaseObservability.Source.StartActivity("database.add.transaction");

        var connection = await dbConnectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)dbConnectionProvider.CurrentTransaction;
        command.CommandText = """
            INSERT INTO ledger (id, from_wallet_id, to_wallet_id, owner_id, amount, description, recorded_at, created_at)
            VALUES (@id, @fromWalletId, @toWalletId, @ownerId, @amount, @description, @recordedAt, @createdAt)
            """;
        command.Parameters.AddWithValue("id", transaction.Id);
        command.Parameters.AddWithValue("fromWalletId", transaction.FromWalletId);
        command.Parameters.AddWithValue("toWalletId", transaction.ToWalletId);
        command.Parameters.AddWithValue("ownerId", transaction.OwnerId);
        command.Parameters.AddWithValue("amount", transaction.Amount);
        command.Parameters.AddWithValue("description", (object?)transaction.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("recordedAt", transaction.RecordedAt);
        command.Parameters.AddWithValue("createdAt", transaction.CreatedAt);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<PagedResult<Okane.Transaction.Domain.Transaction>> GetPagedForOwnerAsync(
        Guid ownerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        using var activity = DatabaseObservability.Source.StartActivity("database.get_paged_for_owner.transaction");

        var connection = await dbConnectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)dbConnectionProvider.CurrentTransaction;
        command.CommandText = $"""
            SELECT {SelectColumns}, COUNT(*) OVER() AS total_count
            FROM ledger
            WHERE owner_id = @ownerId
            ORDER BY recorded_at DESC
            LIMIT @pageSize OFFSET @offset
            """;
        command.Parameters.AddWithValue("ownerId", ownerId);
        command.Parameters.AddWithValue("pageSize", pageSize);
        command.Parameters.AddWithValue("offset", (page - 1) * pageSize);

        var transactions = new List<Okane.Transaction.Domain.Transaction>();
        var totalCount = 0;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            transactions.Add(Map(reader));
            totalCount = reader.GetInt32(8);
        }

        return new PagedResult<Okane.Transaction.Domain.Transaction>(transactions, page, pageSize, totalCount);
    }

    private static Okane.Transaction.Domain.Transaction Map(NpgsqlDataReader reader) => Okane.Transaction.Domain.Transaction.FromPersistence(
        reader.GetGuid(0),
        reader.GetGuid(1),
        reader.GetGuid(2),
        reader.GetGuid(3),
        reader.GetFieldValue<decimal>(4),
        reader.IsDBNull(5) ? null : reader.GetString(5),
        reader.GetFieldValue<DateTimeOffset>(6),
        reader.GetFieldValue<DateTimeOffset>(7));
}
