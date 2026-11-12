using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveHub.Web.Services.Reports;

namespace DriveHub.Tests;

public class ReportExporterTests
{
    [Fact]
    public void CsvExport_ContainsContractAndTotal()
    {
        var rows = new List<SalesReportRow>
        {
            new("DH-1", new DateTime(2026, 11, 12), "Kia Rio", "Иванов И.И.", 900000m)
        };
        var report = new SalesReport(new DateTime(2026, 11, 1), new DateTime(2026, 11, 30), rows);

        var text = Encoding.UTF8.GetString(new CsvReportExporter().Export(report));

        Assert.Contains("DH-1", text);
        Assert.Contains("Итого;;;;900000.00", text.Replace(",", "."));
    }
}