using Npgsql;
using NpgsqlTypes;
using Okane.Kernel;
using System.Collections;

namespace Okane.Api.Reports;

public record DashboardReportRawDataItem(
    Guid WalletId,
    string WalletName,
    Guid FromWalletId,
    Guid ToWalletId,
    decimal Amount
);

public class DashboardReportRepository(IDbConnectionProvider<NpgsqlConnection> connectionProvider) : IDashboardReportRepository
{
    public async Task<IEnumerable<DashboardReportRawDataItem>> GetRawInformationAsync(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT
                w.id AS wallet_id,
                w.name AS wallet_name,
                l.from_wallet_id AS transaction_from_wallet_id,
                l.to_wallet_id AS transaction_to_wallet_id,
                l.amount AS transaction_amount
            FROM users u
            INNER JOIN wallets w ON w.owner_id = u.id
            INNER JOIN ledger l ON (l.from_wallet_id = w.id OR l.to_wallet_id = w.id)
            WHERE
                u.id = @UserId AND
                l.recorded_at BETWEEN @StartDate AND @EndDate;";

        var conn = await connectionProvider.GetConnectionAsync(cancellationToken);

        using var command = new NpgsqlCommand(sql, conn);
        command.Transaction = (NpgsqlTransaction?)connectionProvider.CurrentTransaction;

        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@StartDate", startDate);
        command.Parameters.AddWithValue("@EndDate", endDate);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        int walletIdIdx = reader.GetOrdinal("wallet_id");
        int walletNameIdx = reader.GetOrdinal("wallet_name");
        int fromWalletIdIdx = reader.GetOrdinal("transaction_from_wallet_id");
        int toWalletIdIdx = reader.GetOrdinal("transaction_to_wallet_id");
        int amountIdx = reader.GetOrdinal("transaction_amount");

        List<DashboardReportRawDataItem> rawData = new List<DashboardReportRawDataItem>();

        while (await reader.ReadAsync(cancellationToken))
        {
            DashboardReportRawDataItem item = new DashboardReportRawDataItem(
                reader.GetGuid(walletIdIdx),
                reader.GetString(walletNameIdx),
                reader.GetGuid(fromWalletIdIdx),
                reader.GetGuid(toWalletIdIdx),
                reader.GetDecimal(amountIdx)
            );

            rawData.Add(item);
        }

        return rawData.AsEnumerable();
    }
}
