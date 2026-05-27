using Okane.Accounts;
using Okane.Accounts.Repositories;
using Okane.Accounts.Requests;
using Okane.Accounts.Services;
using Okane.Core.Data;
using Okane.Core.Data.Requests;
using Okane.Core.Exceptions;

namespace Okane.Infrastructure.Services;

public class AccountService(IAccountRepository repository) : IAccountService
{
    public async Task<Account> CreateAccountAsync(ICreateAccountRequest request, CancellationToken cancellationToken)
    {
        Account account = new(request.Name, request.Description, request.User.Id);

        if (await repository.ExistsByNameAsync(account.Name, account.UserId, cancellationToken))
            throw new DomainException($"Account with name {account.Name} already exists", 422);

        return await repository.SaveAccountAsync(account, cancellationToken);
    }

    public Task<Account?> GetAccountAsync(int accountId, int userId, CancellationToken cancellationToken)
    {
        return repository.FindByIdAsync(accountId, userId, cancellationToken);
    }

    public Task<Page<Account>> GetAccountPageAsync(IPageRequest request, int userId, CancellationToken cancellationToken)
    {
        return repository.GetPageAsync(userId, request.Page, request.PageSize, cancellationToken);
    }
}
