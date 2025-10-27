using MediatR;

namespace WOL.Reporting.Application.Commands;

public record GenerateReportCommand : IRequest<GenerateReportResponse>
{
    public string ReportType { get; init; } = string.Empty;
    public string ReportName { get; init; } = string.Empty;
    public Guid? RequestedBy { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string Parameters { get; init; } = "{}";
}

public record GenerateReportResponse
{
    public Guid ReportId { get; init; }
    public string Message { get; init; } = string.Empty;
}
