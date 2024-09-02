namespace Web.DTOs.Account;

public class AccountDto
{
    public long AccountId { get; set; }
    public long CardId { get; set; }
    public string Iban { get; set; } = String.Empty;
    public string AccountType { get; set; } = String.Empty;
    public string BranchTelephone { get; set; } = String.Empty;
    public string BranchAddress { get; set; } = String.Empty;
    public string BranchName { get; set; } = String.Empty;
    public string OwnerName { get; set; } = String.Empty;
    public string OwnerLastName { get; set; } = String.Empty;
    public long OwnerId { get; set; }
}