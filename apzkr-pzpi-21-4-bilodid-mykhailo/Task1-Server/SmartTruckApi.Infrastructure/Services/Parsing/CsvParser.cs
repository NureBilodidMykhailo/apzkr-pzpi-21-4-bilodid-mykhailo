using System.Reflection;
using System.Text;

namespace SmartTruckApi.Infrastructure.Services.Parsing;
public static class CsvHelper
{
    public static string ConvertToCsv<T>(IEnumerable<T> data)
    {
        var csv = new StringBuilder();

        if (data == null || !data.Any())
        {
            return csv.ToString();
        }

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        csv.AppendLine(string.Join(",", properties.Select(p => p.Name)));

        foreach (var item in data)
        {
            var values = properties.Select(p => p.GetValue(item)?.ToString() ?? string.Empty);
            csv.AppendLine(string.Join(",", values));
        }

        return csv.ToString();
    }
}