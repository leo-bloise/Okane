namespace Okane.Transaction.Domain;

public sealed class Transaction
{
    public Guid Id { get; }

    public Guid FromWalletId { get; }

    public Guid ToWalletId { get; }

    public Guid OwnerId { get; }

    public decimal Amount { get; }

    public string? Description { get; }

    public DateTimeOffset RecordedAt { get; }

    public DateTimeOffset CreatedAt { get; }

    private Transaction(
        Guid id,
        Guid fromWalletId,
        Guid toWalletId,
        Guid ownerId,
        decimal amount,
        string? description,
        DateTimeOffset recordedAt,
        DateTimeOffset createdAt)
    {
        Id = id;
        FromWalletId = fromWalletId;
        ToWalletId = toWalletId;
        OwnerId = ownerId;
        Amount = amount;
        Description = description;
        RecordedAt = recordedAt;
        CreatedAt = createdAt;
    }

    public static Transaction Record(Guid fromWalletId, Guid toWalletId, Guid ownerId, decimal amount, string? description = null)
    {
        if (fromWalletId == toWalletId)
        {
            throw new ArgumentException("A transaction cannot transfer value to the same wallet.", nameof(toWalletId));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Transaction amount must be greater than zero.");
        }

        DateTimeOffset now = DateTimeOffset.UtcNow;
        return new Transaction(Guid.NewGuid(), fromWalletId, toWalletId, ownerId, amount, description?.Trim(), now, now);
    }

    public static Transaction FromPersistence(
        Guid id,
        Guid fromWalletId,
        Guid toWalletId,
        Guid ownerId,
        decimal amount,
        string? description,
        DateTimeOffset recordedAt,
        DateTimeOffset createdAt)
        => new(id, fromWalletId, toWalletId, ownerId, amount, description, recordedAt, createdAt);
}
