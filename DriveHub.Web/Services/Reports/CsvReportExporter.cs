using System.Text;

namespace DriveHub.Web.Services.Reports;

/// <summary>Выгрузка отчета в CSV (разделитель «;», чтобы Excel открыл без настройки).</summary>
public class CsvReportExporter : IReportExporter
{
    /// <inheritdoc />
    public string Format => "csv";

    /// <inheritdoc />
    public byte[] Export(SalesReport report)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Договор;Дата;Автомобиль;Клиент;Сумма");
        foreach (var r in report.Rows)
            sb.AppendLine($"{r.ContractNumber};{r.DealDate:dd.MM.yyyy};{r.CarName};{r.ClientName};{r.TotalPrice:0.00}");
        sb.AppendLine($"Итого;;;;{report.Total:0.00}");
        return new UTF8Encoding(true).GetBytes(sb.ToString());
    }
}