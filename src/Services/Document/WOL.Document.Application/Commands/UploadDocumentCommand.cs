using MediatR;

namespace WOL.Document.Application.Commands;

public record UploadDocumentCommand : IRequest<UploadDocumentResponse>
{
    public Guid UserId { get; init; }
    public string DocumentType { get; init; } = string.Empty;
    public string DocumentNumber { get; init; } = string.Empty;
    public string FilePath { get; init; } = string.Empty;
    public DateTime IssueDate { get; init; }
    public DateTime ExpiryDate { get; init; }
}

public record UploadDocumentResponse
{
    public Guid DocumentId { get; init; }
    public string Message { get; init; } = string.Empty;
}
