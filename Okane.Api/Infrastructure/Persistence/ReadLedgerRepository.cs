using Npgsql;
using Okane.Kernel;
using Okane.Transaction.Application.Interfaces;
using Okane.Transaction.Application.ReadModels;

namespace Okane.Api.Infrastructure.Persistence;

public sealed class ReadLedgerRepository(IDbConnectionProvider<NpgsqlConnection> dbConnectionProvider) : IReadLedgerRepository
{
    public async Task<LedgerPage> GetFilteredPagedForOwnerAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        using var activity = DatabaseObservability.Source.StartActivity("database.get_paged_for_owner.ledger");

        var connection = await dbConnectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)dbConnectionProvider.CurrentTransaction;
        command.CommandText = """
            SELECT l.id, l.from_wallet_id, fw.name, l.to_wallet_id, tw.name, l.owner_id, l.amount, l.description, l.recorded_at, l.created_at,
                   COUNT(*) OVER() AS total_count
            FROM ledger l
            JOIN wallets fw ON fw.id = l.from_wallet_id
            JOIN wallets tw ON tw.id = l.to_wallet_id
            WHERE l.owner_id = @ownerId
            ORDER BY l.recorded_at DESC
            LIMIT @pageSize OFFSET @offset
            """;
        command.Parameters.AddWithValue("ownerId", ownerId);
        command.Parameters.AddWithValue("pageSize", pageSize);
        command.Parameters.AddWithValue("offset", (page - 1) * pageSize);

        var entries = new List<LedgerEntry>();
        var totalCount = 0;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            entries.Add(Map(reader));
            totalCount = reader.GetInt32(10);
        }

        return new LedgerPage(new Ledger(entries), page, pageSize, totalCount);
    }

    private static LedgerEntry Map(NpgsqlDataReader reader) => new(
        reader.GetGuid(0),
        new WalletSummary(reader.GetGuid(1), reader.GetString(2)),
        new WalletSummary(reader.GetGuid(3), reader.GetString(4)),
        reader.GetGuid(5),
        reader.GetFieldValue<decimal>(6),
        reader.IsDBNull(7) ? null : reader.GetString(7),
        reader.GetFieldValue<DateTimeOffset>(8),
        reader.GetFieldValue<DateTimeOffset>(9));
}
