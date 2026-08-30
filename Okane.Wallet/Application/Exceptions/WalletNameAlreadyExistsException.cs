using System.Net;
using Okane.Kernel.Exceptions;

namespace Okane.Wallet.Application.Exceptions;

public sealed class WalletNameAlreadyExistsException(Guid ownerId, string name)
    : DomainException(
        $"Owner '{ownerId}' already has a wallet named '{name}'.",
        HttpStatusCode.Conflict,
        "A wallet with this name already exists.");
