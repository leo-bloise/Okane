using System.Net;
using Okane.Kernel.Exceptions;

namespace Okane.Wallet.Application.Exceptions;

public sealed class WalletNotFoundException(Guid walletId)
    : DomainException($"Wallet '{walletId}' was not found.", HttpStatusCode.NotFound, "Wallet not found.");
