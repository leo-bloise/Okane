using System.Net;
using Okane.Kernel.Exceptions;

namespace Okane.Transaction.Application.Exceptions;

public sealed class WalletOwnershipMismatchException(Guid ownerId)
    : DomainException(
        $"Owner '{ownerId}' does not own both wallets.",
        HttpStatusCode.Forbidden,
        "You do not own one or both wallets.");
