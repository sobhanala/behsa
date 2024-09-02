using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Application.Services.SharedService;

public class PersianDateConverter : DateTimeConverter
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return DateTime.MinValue;
        }

        try
        {
            var persianCalendar = new PersianCalendar();
            var parts = text.Split('/');

            var year = int.Parse(parts[0]);
            var month = int.Parse(parts[1]);
            var day = int.Parse(parts[2]);

            var gregorianDate = persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            return DateTime.SpecifyKind(gregorianDate, DateTimeKind.Utc);
        }
        catch (Exception ex)
        {
            throw new TypeConverterException(this, memberMapData, text, row.Context, ex.Message);
        }
    }
}