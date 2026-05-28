namespace Okane.Ledger.Requests;

public interface ICreateTransactionRequest
{
    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public int UserId { get; set; }

    public int FromAccountId { get; set; }

    public int ToAccountId { get; set; }

    public DateTime OccuredAt { get; set; }
}
