namespace DriveHub.Web.Services.Reports;

/// <summary>Единый интерфейс выгрузки отчета в файл.</summary>
public interface IReportExporter
{
    /// <summary>Формат файла (csv, xlsx).</summary>
    string Format { get; }

    /// <summary>Преобразует отчет в массив байт файла.</summary>
    /// <param name="report">Отчет о продажах.</param>
    byte[] Export(SalesReport report);
}