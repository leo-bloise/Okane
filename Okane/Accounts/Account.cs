using Microsoft.EntityFrameworkCore;
using Okane.Authentication;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Okane.Accounts;

[Index(nameof(Name), IsUnique = true)]
[Table("accounts")]
public class Account
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column(name: "name")]
    public string Name { get; set; }

    [Required]
    [Column(name: "description")]
    public string Description { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    public Account() { }

    public Account(string name, string description, int userId)
    {
        Name = name;
        Description = description;
        UserId = userId;
    }
}
