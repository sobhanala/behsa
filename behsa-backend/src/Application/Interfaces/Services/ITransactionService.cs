using Application.DTOs;
using Application.DTOs.Transaction;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface ITransactionService
{
    Task<Result> AddTransactionsFromCsvAsync(string filePath);
    Task<Result<List<Transaction>>> GetAllTransactionsAsync();
    Task<Result<List<GetTransactionsByAccountIdResponse>>> GetTransactionsByAccountIdAsync(long accountId);
}