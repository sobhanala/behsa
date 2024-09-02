using Application.DTOs.Transaction;
using Domain.Entities;

namespace Application.Mappers;
public static class TransactionMapper
{
    public static Transaction ToTransaction(this TransactionCsvModel csvModel)
    {
        return new Transaction
        {
            TransactionId = csvModel.TransactionID,
            SourceAccountId = csvModel.SourceAcount,
            DestinationAccountId = csvModel.DestiantionAccount,
            Amount = csvModel.Amount,
            Date = csvModel.Date,
            Type = csvModel.Type
        };
    }
}