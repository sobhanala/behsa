using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IAccountRepository
{
    Task CreateBulkAsync(List<Account> accounts);
    Task<Account?> GetByIdAsync(long accountId);
    Task<List<Account>> GetAllAccounts();
    Task<List<long>> GetAllIdsAsync();
}