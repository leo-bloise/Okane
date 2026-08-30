using Npgsql;
using Okane.Kernel;
using Okane.User.Application;
using Okane.User.Application.Interfaces;
using Okane.Wallet.Application.Interfaces;

namespace Okane.Api.Infrastructure.UseCases;

public sealed class CreateUserUseCase(
    IUserService userService, IWalletService walletService, IDbConnectionProvider<NpgsqlConnection> dbConnectionProvider
): ICreateUserUseCase
{
    public async Task<User.Domain.User> CreateUser(string name, string email, string password, CancellationToken cancellationToken = default)
    {
        await dbConnectionProvider.BeginTransactionAsync(cancellationToken);

        User.Domain.User? user = null;

        try
        {
            user = await userService.CreateUserAsync(name, email, password, cancellationToken);
            await walletService.CreateExternalWalletAsync(user.Id, cancellationToken);

            await dbConnectionProvider.CommitAsync(cancellationToken);
        } catch(Exception ex)
        {
            await dbConnectionProvider.RollbackAsync(cancellationToken);
            throw;
        }

        return user;
    }
}
