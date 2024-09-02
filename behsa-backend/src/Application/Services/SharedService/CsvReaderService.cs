using System.Globalization;
using Application.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;

namespace Application.Services.SharedService;

public class CsvReaderService : IFileReaderService
{
    public List<T> ReadFromFile<T>(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        });

        var records = csv.GetRecords<T>().ToList();
        return records;
    }
}