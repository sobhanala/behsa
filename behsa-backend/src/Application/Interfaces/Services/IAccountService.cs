using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface IAccountService
{
    Task<Result> AddAccountsFromCsvAsync(string filePath);
    Task<Result<Account>> GetAccountByIdAsync(long accountId);
    Task<Result<List<Account>>> GetAllAccountsAsync();
}