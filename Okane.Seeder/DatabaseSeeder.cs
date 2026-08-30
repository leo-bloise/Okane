using System.Text;
using Microsoft.Extensions.Logging;
using Npgsql;
using Okane.Kernel;
using Okane.User.Application.Interfaces;

namespace Okane.Seeder;

public sealed class DatabaseSeeder(
    IDbConnectionProvider<NpgsqlConnection> connectionProvider,
    IPasswordHasher passwordHasher,
    ILogger<DatabaseSeeder> logger)
{
    private const string DemoEmail = "dashboard.demo@example.com";
    private const string DemoPassword = "DemoPass123";
    private const int TransactionsPerMonth = 5_000;
    private const int BatchSize = 1_000;

    private static readonly Guid UserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    // 1 External wallet + 9 Standard wallets = 10 wallets total.
    private static readonly (Guid Id, string Name, string Kind)[] Wallets =
    [
        (Guid.Parse("22222222-2222-2222-2222-222222222222"), "External Wallet", "External"),
        (Guid.Parse("33333333-3333-3333-3333-333333333331"), "Checking", "Standard"),
        (Guid.Parse("33333333-3333-3333-3333-333333333332"), "Savings", "Standard"),
        (Guid.Parse("33333333-3333-3333-3333-333333333333"), "Credit Card", "Standard"),
        (Guid.Parse("33333333-3333-3333-3333-333333333334"), "Cash", "Standard"),
        (Guid.Parse("33333333-3333-3333-3333-333333333335"), "Investments", "Standard"),
        (Guid.Parse("33333333-3333-3333-3333-333333333336"), "Travel Fund", "Standard"),
        (Guid.Parse("33333333-3333-3333-3333-333333333337"), "Emergency Fund", "Standard"),
        (Guid.Parse("33333333-3333-3333-3333-333333333338"), "Business", "Standard"),
        (Guid.Parse("33333333-3333-3333-3333-333333333339"), "Gifts", "Standard"),
    ];

    private static readonly string[] Descriptions =
    [
        "Salary", "Groceries", "Rent", "Transfer", "Refund", "Utility bill", "Dining out",
        "Entertainment", "Shopping", "Subscription", "Fuel", "Insurance", "Healthcare",
        "Gift", "Freelance income",
    ];

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Database seeding started.");

        try
        {
            logger.LogInformation("Seeding demo user ({Email})...", DemoEmail);
            await SeedUserAsync(cancellationToken);
            logger.LogInformation("Demo user ready.");

            logger.LogInformation("Seeding {WalletCount} wallets...", Wallets.Length);
            foreach (var wallet in Wallets)
            {
                await SeedWalletAsync(wallet.Id, wallet.Name, wallet.Kind, cancellationToken);
            }
            logger.LogInformation("Wallets ready.");

            if (await HasTransactionsAsync(cancellationToken))
            {
                logger.LogInformation("Demo transactions already exist, skipping generation.");
            }
            else
            {
                var now = DateTimeOffset.UtcNow;
                var currentMonthStart = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);
                var previousMonthStart = currentMonthStart.AddMonths(-1);

                // Fixed seed so the "random" sample data is reproducible across seed runs.
                var random = new Random(42);

                logger.LogInformation("Generating {Count} transactions for the current month ({Month:yyyy-MM})...", TransactionsPerMonth, currentMonthStart);
                await SeedTransactionsForMonthAsync(currentMonthStart, random, cancellationToken);
                logger.LogInformation("Current month transactions ready.");

                logger.LogInformation("Generating {Count} transactions for the previous month ({Month:yyyy-MM})...", TransactionsPerMonth, previousMonthStart);
                await SeedTransactionsForMonthAsync(previousMonthStart, random, cancellationToken);
                logger.LogInformation("Previous month transactions ready.");
            }

            logger.LogInformation(
                "Database seeding finished successfully. Demo account ready - email: {Email}, password: {Password}.",
                DemoEmail,
                DemoPassword);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database seeding failed.");
            Console.Error.WriteLine($"[Okane.Seeder] Database seeding failed: {ex.Message}");
        }
    }

    private async Task<bool> HasTransactionsAsync(CancellationToken cancellationToken)
    {
        var connection = await connectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)connectionProvider.CurrentTransaction;
        command.CommandText = "SELECT EXISTS(SELECT 1 FROM ledger WHERE owner_id = @ownerId)";
        command.Parameters.AddWithValue("ownerId", UserId);

        return (bool)(await command.ExecuteScalarAsync(cancellationToken))!;
    }

    private async Task SeedUserAsync(CancellationToken cancellationToken)
    {
        var connection = await connectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)connectionProvider.CurrentTransaction;
        command.CommandText = """
            INSERT INTO users (id, name, email, password_hash, created_at)
            VALUES (@id, @name, @email, @passwordHash, @createdAt)
            ON CONFLICT (email) DO NOTHING
            """;
        command.Parameters.AddWithValue("id", UserId);
        command.Parameters.AddWithValue("name", "Dashboard Demo");
        command.Parameters.AddWithValue("email", DemoEmail);
        command.Parameters.AddWithValue("passwordHash", passwordHasher.Hash(DemoPassword));
        command.Parameters.AddWithValue("createdAt", DateTimeOffset.UtcNow);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task SeedWalletAsync(Guid walletId, string name, string kind, CancellationToken cancellationToken)
    {
        var connection = await connectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)connectionProvider.CurrentTransaction;
        command.CommandText = """
            INSERT INTO wallets (id, owner_id, name, kind, status, created_at)
            VALUES (@id, @ownerId, @name, @kind, 'Active', @createdAt)
            ON CONFLICT (owner_id, name) DO NOTHING
            """;
        command.Parameters.AddWithValue("id", walletId);
        command.Parameters.AddWithValue("ownerId", UserId);
        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("kind", kind);
        command.Parameters.AddWithValue("createdAt", DateTimeOffset.UtcNow);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task SeedTransactionsForMonthAsync(DateTimeOffset monthStart, Random random, CancellationToken cancellationToken)
    {
        var daysInMonth = DateTime.DaysInMonth(monthStart.Year, monthStart.Month);
        var remaining = TransactionsPerMonth;
        var inserted = 0;
        var batchNumber = 0;
        var totalBatches = (int)Math.Ceiling(TransactionsPerMonth / (double)BatchSize);

        while (remaining > 0)
        {
            batchNumber++;
            var batchCount = Math.Min(BatchSize, remaining);
            await InsertTransactionBatchAsync(monthStart, daysInMonth, batchCount, random, cancellationToken);
            remaining -= batchCount;
            inserted += batchCount;

            logger.LogInformation(
                "  Batch {BatchNumber}/{TotalBatches}: inserted {Inserted}/{Total} transactions.",
                batchNumber,
                totalBatches,
                inserted,
                TransactionsPerMonth);
        }
    }

    private async Task InsertTransactionBatchAsync(
        DateTimeOffset monthStart,
        int daysInMonth,
        int count,
        Random random,
        CancellationToken cancellationToken)
    {
        var connection = await connectionProvider.GetConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction?)connectionProvider.CurrentTransaction;

        var sql = new StringBuilder(
            "INSERT INTO ledger (id, from_wallet_id, to_wallet_id, owner_id, amount, description, recorded_at, created_at) VALUES ");

        for (var i = 0; i < count; i++)
        {
            var fromIndex = random.Next(Wallets.Length);
            int toIndex;
            do
            {
                toIndex = random.Next(Wallets.Length);
            }
            while (toIndex == fromIndex);

            var recordedAt = monthStart
                .AddDays(random.Next(daysInMonth))
                .AddHours(random.Next(24))
                .AddMinutes(random.Next(60));

            var amount = Math.Round((decimal)(random.NextDouble() * 1495 + 5), 2);
            var description = Descriptions[random.Next(Descriptions.Length)];

            if (i > 0)
            {
                sql.Append(", ");
            }

            sql.Append($"(@id{i}, @from{i}, @to{i}, @owner{i}, @amount{i}, @description{i}, @recordedAt{i}, @recordedAt{i})");

            command.Parameters.AddWithValue($"id{i}", Guid.NewGuid());
            command.Parameters.AddWithValue($"from{i}", Wallets[fromIndex].Id);
            command.Parameters.AddWithValue($"to{i}", Wallets[toIndex].Id);
            command.Parameters.AddWithValue($"owner{i}", UserId);
            command.Parameters.AddWithValue($"amount{i}", amount);
            command.Parameters.AddWithValue($"description{i}", description);
            command.Parameters.AddWithValue($"recordedAt{i}", recordedAt);
        }

        sql.Append(" ON CONFLICT (id) DO NOTHING");
        command.CommandText = sql.ToString();

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
