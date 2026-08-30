using Npgsql;
using Okane.Kernel;
using System.Data.Common;

namespace Okane.Api.Infrastructure.Persistence;

public sealed class NpgsqlConnectionProvider(NpgsqlConnectionFactory factory) : IDbConnectionProvider<NpgsqlConnection>, IAsyncDisposable
{
    public DbTransaction? CurrentTransaction => _currentTransaction;

    private DbTransaction? _currentTransaction;

    private NpgsqlConnection? _connection;

    public async Task<NpgsqlConnection> GetConnectionAsync(CancellationToken cancellationToken)
    {
        _connection ??= await factory.OpenConnectionAsync(cancellationToken);
        return _connection;
    }

    public async Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        DbConnection conn = await GetConnectionAsync(cancellationToken);
        _currentTransaction ??= await conn.BeginTransactionAsync(cancellationToken);
        return _currentTransaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        if (_currentTransaction is null)
            return;

        await _currentTransaction.CommitAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        if (_currentTransaction is null)
            return;

        await _currentTransaction.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction is not null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }
}
