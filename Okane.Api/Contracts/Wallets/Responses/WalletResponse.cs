using System.Text.Json.Serialization;
using Okane.Wallet.Domain;

namespace Okane.Api.Contracts.Wallets.Responses;

public sealed record WalletResponse(
    Guid Id,
    string Name,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] WalletKind Kind,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] WalletStatus Status,
    DateTimeOffset CreatedAt);
