using Application.Services.SharedService;
using CsvHelper.Configuration.Attributes;

namespace Application.DTOs.Transaction;

public class TransactionCsvModel
{
    public long TransactionID { get; set; }
    public long SourceAcount { get; set; }
    public long DestiantionAccount { get; set; }
    public decimal Amount { get; set; }
    [TypeConverter(typeof(PersianDateConverter))]
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
}