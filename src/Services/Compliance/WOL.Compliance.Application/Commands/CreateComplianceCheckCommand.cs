using MediatR;

namespace WOL.Compliance.Application.Commands;

public record CreateComplianceCheckCommand : IRequest<CreateComplianceCheckResponse>
{
    public Guid EntityId { get; init; }
    public string EntityType { get; init; } = string.Empty;
    public string CheckType { get; init; } = string.Empty;
}

public record CreateComplianceCheckResponse
{
    public Guid CheckId { get; init; }
    public string Message { get; init; } = string.Empty;
}
