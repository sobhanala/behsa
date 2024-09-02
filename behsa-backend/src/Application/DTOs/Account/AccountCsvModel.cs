namespace Application.DTOs.Account;

public class AccountCsvModel
{
    public long AccountID { get; set; }
    public long CardID { get; set; }
    public string IBAN { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string BranchTelephone { get; set; } = string.Empty;
    public string BranchAdress { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string OwnerLastName { get; set; } = string.Empty;
    public long OwnerID { get; set; }
}