using System.Net;
using Okane.Kernel.Exceptions;

namespace Okane.Wallet.Application.Exceptions;

public sealed class ExternalWalletAlreadyExistsException(Guid ownerId)
    : DomainException(
        $"Owner '{ownerId}' already has an External Wallet.",
        HttpStatusCode.Conflict,
        "This owner already has an External Wallet.");
