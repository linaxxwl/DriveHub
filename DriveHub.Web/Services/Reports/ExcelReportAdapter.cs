using ClosedXML.Excel;

namespace DriveHub.Web.Services.Reports;

/// <summary>Адаптер: приводит библиотеку ClosedXML (XLWorkbook) к интерфейсу IReportExporter.</summary>
public class ExcelReportAdapter : IReportExporter
{
    /// <inheritdoc />
    public string Format => "xlsx";

    /// <inheritdoc />
    public byte[] Export(SalesReport report)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Продажи");

        string[] headers = { "Договор", "Дата", "Автомобиль", "Клиент", "Сумма" };
        for (int i = 0; i < headers.Length; i++)
            sheet.Cell(1, i + 1).Value = headers[i];

        int row = 2;
        foreach (var r in report.Rows)
        {
            sheet.Cell(row, 1).Value = r.ContractNumber;
            sheet.Cell(row, 2).Value = r.DealDate;
            sheet.Cell(row, 3).Value = r.CarName;
            sheet.Cell(row, 4).Value = r.ClientName;
            sheet.Cell(row, 5).Value = r.TotalPrice;
            row++;
        }
        sheet.Cell(row, 4).Value = "Итого";
        sheet.Cell(row, 5).Value = report.Total;
        sheet.Row(1).Style.Font.Bold = true;
        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}