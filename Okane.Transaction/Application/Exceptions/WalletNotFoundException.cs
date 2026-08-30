using System.Net;
using Okane.Kernel.Exceptions;

namespace Okane.Transaction.Application.Exceptions;

public sealed class WalletNotFoundException(Guid fromWalletId, Guid toWalletId)
    : DomainException(
        $"Wallet not found ({fromWalletId} / {toWalletId}).",
        HttpStatusCode.NotFound,
        "Wallet not found.");
