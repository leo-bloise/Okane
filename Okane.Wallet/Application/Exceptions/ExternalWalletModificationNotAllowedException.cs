using System.Net;
using Okane.Kernel.Exceptions;

namespace Okane.Wallet.Application.Exceptions;

public sealed class ExternalWalletModificationNotAllowedException()
    : DomainException("The External Wallet cannot be renamed, archived, or deleted.", HttpStatusCode.Conflict);
