namespace Application.DTOs.Transaction;

public class GetTransactionsByAccountIdResponse
{
    public long AccountId { get; set; }
    public List<TransactionCsvModel> TransactionWithSources = new();
}