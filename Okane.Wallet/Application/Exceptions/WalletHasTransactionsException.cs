using System.Net;
using Okane.Kernel.Exceptions;

namespace Okane.Wallet.Application.Exceptions;

public sealed class WalletHasTransactionsException(Guid walletId)
    : DomainException(
        $"Wallet '{walletId}' has recorded transactions and cannot be deleted.",
        HttpStatusCode.Conflict,
        "A wallet with recorded transactions cannot be deleted; archive it instead.");
