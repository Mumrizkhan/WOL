using MediatR;
using WOL.Shared.Common.Application;
using WOL.Reporting.Domain.Repositories;

namespace WOL.Reporting.Application.Commands;

public class GenerateReportCommandHandler : IRequestHandler<GenerateReportCommand, GenerateReportResponse>
{
    private readonly IReportRepository _reportRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GenerateReportCommandHandler(IReportRepository reportRepository, IUnitOfWork unitOfWork)
    {
        _reportRepository = reportRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GenerateReportResponse> Handle(GenerateReportCommand request, CancellationToken cancellationToken)
    {
        var report = Domain.Entities.Report.Create(
            request.ReportType,
            request.ReportName,
            request.RequestedBy,
            request.StartDate,
            request.EndDate,
            request.Parameters);

        await _reportRepository.AddAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new GenerateReportResponse
        {
            ReportId = report.Id,
            Message = "Report generation request submitted successfully"
        };
    }
}
