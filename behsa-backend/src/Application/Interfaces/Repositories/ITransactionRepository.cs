using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task CreateBulkAsync(List<Transaction> transactions);
    Task<List<Transaction>> GetAllTransactions();
    Task<List<Transaction>> GetBySourceAccountId(long accountId);
    Task<List<Transaction>> GetByDestinationAccountId(long accountId);
    Task<List<long>> GetAllIdsAsync();
}