using Application.DTOs.Account;
using Domain.Entities;
using Web.DTOs.Account;

namespace Web.Mappers;

public static class AccountMapper
{
    public static AccountDto ToAccountDto(this Account account)
    {
        return new AccountDto
        {
            AccountId = account.AccountId,
            CardId = account.CardId,
            Iban = account.Iban,
            AccountType = account.AccountType,
            BranchTelephone = account.BranchTelephone,
            BranchAddress = account.BranchAddress,
            BranchName = account.BranchName,
            OwnerName = account.OwnerName,
            OwnerLastName = account.OwnerLastName,
            OwnerId = account.OwnerId
        };
    }
    
    public static List<AccountDto> ToGotAllAccountsDto(this List<Account> accounts)
    {
        return accounts.Select(account => new AccountDto
        {
            AccountId = account.AccountId,
            CardId = account.CardId,
            Iban = account.Iban,
            AccountType = account.AccountType,
            BranchTelephone = account.BranchTelephone,
            BranchAddress = account.BranchAddress,
            BranchName = account.BranchName,
            OwnerName = account.OwnerName,
            OwnerLastName = account.OwnerLastName,
            OwnerId = account.OwnerId
        }).ToList();
    }
}