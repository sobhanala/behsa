using Application.DTOs.Account;
using Domain.Entities;

namespace Application.Mappers;

public static class AccountMapper
{
    public static Account ToAccount(this AccountCsvModel csvModel)
    {
        return new Account
        {
            AccountId = csvModel.AccountID,
            CardId = csvModel.CardID,
            Iban = csvModel.IBAN,
            AccountType = csvModel.AccountType,
            BranchTelephone = csvModel.BranchTelephone,
            BranchAddress = csvModel.BranchAdress,
            BranchName = csvModel.BranchName,
            OwnerName = csvModel.OwnerName,
            OwnerLastName = csvModel.OwnerLastName,
            OwnerId = csvModel.OwnerID
        };
    }
}