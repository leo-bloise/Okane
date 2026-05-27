using Okane.Core.Data;

namespace Okane.Accounts.Repositories;

public interface IAccountRepository
{
    public Task<Account> SaveAccountAsync(Account account, CancellationToken cancellationToken);

    public Task<bool> ExistsByNameAsync(string name, int userId, CancellationToken cancellationToken);

    public Task<Account?> FindByIdAsync(int id, int userId, CancellationToken cancellationToken);

    public Task<Page<Account>> GetPageAsync(int userId, int pageIndex, int pageSize, CancellationToken cancellationToken);
}
