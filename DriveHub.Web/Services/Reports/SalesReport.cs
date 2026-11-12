namespace DriveHub.Web.Services.Reports;

/// <summary>Строка отчета о продажах.</summary>
/// <param name="ContractNumber">Номер договора.</param>
/// <param name="DealDate">Дата сделки.</param>
/// <param name="CarName">Марка и модель автомобиля.</param>
/// <param name="ClientName">ФИО клиента.</param>
/// <param name="TotalPrice">Итоговая цена.</param>
public record SalesReportRow(string ContractNumber, DateTime DealDate, string CarName, string ClientName, decimal TotalPrice);

/// <summary>Отчет о продажах за период.</summary>
/// <param name="From">Начало периода.</param>
/// <param name="To">Конец периода.</param>
/// <param name="Rows">Строки отчёта.</param>
public record SalesReport(DateTime From, DateTime To, IReadOnlyList<SalesReportRow> Rows)
{
    /// <summary>Общая сумма продаж.</summary>
    public decimal Total => Rows.Sum(r => r.TotalPrice);
}