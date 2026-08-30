namespace Okane.Wallet.Domain;

public sealed class Wallet
{
    private const string ExternalWalletName = "External Wallet";

    public Guid Id { get; }

    public Guid OwnerId { get; }

    public string Name { get; private set; }

    public WalletKind Kind { get; }

    public WalletStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    private Wallet(Guid id, Guid ownerId, string name, WalletKind kind, WalletStatus status, DateTimeOffset createdAt)
    {
        Id = id;
        OwnerId = ownerId;
        Name = name;
        Kind = kind;
        Status = status;
        CreatedAt = createdAt;
    }

    public static Wallet CreateStandard(Guid ownerId, string name)
    {
        EnsureValidName(name);
        return new Wallet(Guid.NewGuid(), ownerId, name.Trim(), WalletKind.Standard, WalletStatus.Active, DateTimeOffset.UtcNow);
    }

    public static Wallet CreateExternal(Guid ownerId)
    {
        return new Wallet(Guid.NewGuid(), ownerId, ExternalWalletName, WalletKind.External, WalletStatus.Active, DateTimeOffset.UtcNow);
    }

    public static Wallet FromPersistence(Guid id, Guid ownerId, string name, WalletKind kind, WalletStatus status, DateTimeOffset createdAt)
        => new(id, ownerId, name, kind, status, createdAt);

    public void Rename(string name)
    {
        EnsureModifiable();
        EnsureValidName(name);
        Name = name.Trim();
    }

    public void Archive()
    {
        EnsureModifiable();

        if (Status == WalletStatus.Archived)
        {
            throw new InvalidOperationException("Wallet is already archived.");
        }

        Status = WalletStatus.Archived;
    }

    public void Reactivate()
    {
        EnsureModifiable();

        if (Status == WalletStatus.Active)
        {
            throw new InvalidOperationException("Wallet is already active.");
        }

        Status = WalletStatus.Active;
    }

    private void EnsureModifiable()
    {
        if (Kind == WalletKind.External)
        {
            throw new InvalidOperationException("The External Wallet cannot be renamed, archived, or deleted.");
        }
    }

    private static void EnsureValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Wallet name cannot be empty.", nameof(name));
        }
    }
}
