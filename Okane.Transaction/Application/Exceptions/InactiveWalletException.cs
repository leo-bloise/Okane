using System.Net;
using Okane.Kernel.Exceptions;

namespace Okane.Transaction.Application.Exceptions;

public sealed class InactiveWalletException()
    : DomainException("Both wallets must be active.", HttpStatusCode.UnprocessableEntity);
