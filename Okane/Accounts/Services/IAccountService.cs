using Okane.Accounts.Requests;
using Okane.Core.Data;
using Okane.Core.Data.Requests;

namespace Okane.Accounts.Services;

public interface IAccountService
{
    public Task<Account> CreateAccountAsync(ICreateAccountRequest request, CancellationToken cancellationToken);

    public Task<Page<Account>> GetAccountPageAsync(IPageRequest request, int userId, CancellationToken cancellationToken);

    public Task<Account?> GetAccountAsync(int accountId, int userId, CancellationToken cancellationToken);
}
