using WOL.Shared.Common.Domain;

namespace WOL.Reporting.Domain.Entities;

public class Report : BaseEntity
{
    public string ReportType { get; private set; } = string.Empty;
    public string ReportName { get; private set; } = string.Empty;
    public Guid? RequestedBy { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string Status { get; private set; } = "Pending";
    public string? FilePath { get; private set; }
    public DateTime? GeneratedAt { get; private set; }
    public string Parameters { get; private set; } = "{}";

    private Report() { }

    public static Report Create(string reportType, string reportName, Guid? requestedBy, DateTime? startDate, DateTime? endDate, string parameters)
    {
        return new Report
        {
            ReportType = reportType,
            ReportName = reportName,
            RequestedBy = requestedBy,
            StartDate = startDate,
            EndDate = endDate,
            Status = "Pending",
            Parameters = parameters
        };
    }

    public void MarkAsGenerating()
    {
        Status = "Generating";
        SetUpdatedAt();
    }

    public void MarkAsCompleted(string filePath)
    {
        Status = "Completed";
        FilePath = filePath;
        GeneratedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void MarkAsFailed()
    {
        Status = "Failed";
        SetUpdatedAt();
    }
}
