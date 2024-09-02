using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;
[Table("Accounts")]
public class Account
{
    [Key]
    public long AccountId { get; set; }
    public long CardId { get; set; }
    
    [MaxLength(50)]
    public string Iban { get; set; } = String.Empty;

    [MaxLength(50)]
    public string AccountType { get; set; } = String.Empty;

    [MaxLength(20)]
    public string BranchTelephone { get; set; } = String.Empty;

    [MaxLength(150)]
    public string BranchAddress { get; set; } = String.Empty;

    [MaxLength(50)]
    public string BranchName { get; set; } = String.Empty;

    [MaxLength(50)]
    public string OwnerName { get; set; } = String.Empty;

    [MaxLength(50)]
    public string OwnerLastName { get; set; } = String.Empty;
    public long OwnerId { get; set; }

    public List<Transaction> SourceTransactions { get; set; } = new();
    public List<Transaction> DestinationTransactions { get; set; } = new();
}