using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _dbContext;
    public TransactionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateBulkAsync(List<Transaction> transactions)
    {
        await _dbContext.Transactions.AddRangeAsync(transactions);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Transaction>> GetAllTransactions()
    {
        return await _dbContext.Transactions.ToListAsync();
    }

    public Task<List<Transaction>> GetBySourceAccountId(long accountId)
    {
        return _dbContext.Transactions
            .Where(transaction => transaction.SourceAccountId == accountId)
            .ToListAsync();
    }

    public Task<List<Transaction>> GetByDestinationAccountId(long accountId)
    {
        return _dbContext.Transactions
            .Where(transaction => transaction.DestinationAccountId == accountId)
            .ToListAsync();
    }

    public async Task<List<long>> GetAllIdsAsync()
    {
        return await _dbContext.Transactions
            .Select(a => a.TransactionId)
            .ToListAsync();
    }
}