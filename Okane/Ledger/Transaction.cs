using Okane.Accounts;
using Okane.Authentication;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Okane.Ledger;

[Table("ledger")]
public class Transaction
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey(nameof(FromAccountId))]
    public Account FromAccount { get; set; }

    [Column("from_account")]
    public int FromAccountId { get; set; }

    [ForeignKey(nameof(ToAccountId))]
    public Account ToAccount { get; set; }

    [Column("to_account")]
    public int ToAccountId { get; set; }

    [Column("occured_at")]
    public DateTime OccuredAt { get; set; }
}
