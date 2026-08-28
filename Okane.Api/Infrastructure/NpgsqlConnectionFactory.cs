using Npgsql;

namespace Okane.Api.Infrastructure;

/// <summary>
/// Factory for ADO.NET connections and commands against the Okane Postgres database.
/// Registered as a singleton in the D.I. container; concrete repositories receive it
/// through their constructor. Wraps a single pooled <see cref="NpgsqlDataSource"/>, as
/// recommended by Npgsql, rather than creating one per connection.
/// </summary>
public sealed class NpgsqlConnectionFactory(string connectionString) : IDisposable
{
    private readonly NpgsqlDataSource _dataSource = NpgsqlDataSource.Create(connectionString);

    public NpgsqlConnection CreateConnection() => _dataSource.CreateConnection();

    public async Task<NpgsqlConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
        => await _dataSource.OpenConnectionAsync(cancellationToken);

    public NpgsqlCommand CreateCommand(string commandText, NpgsqlConnection connection)
        => new(commandText, connection);

    public void Dispose() => _dataSource.Dispose();
}
