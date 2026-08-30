using System.Data;
using System.Data.Common;

namespace Okane.Kernel;

public interface IDbConnectionProvider<T> where T : IDbConnection, IDisposable
{
    Task<T> GetConnectionAsync(CancellationToken cancellationToken);

    Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken);

    Task CommitAsync(CancellationToken cancellationToken);

    Task RollbackAsync(CancellationToken cancellationToken);

    DbTransaction? CurrentTransaction { get; }
}
