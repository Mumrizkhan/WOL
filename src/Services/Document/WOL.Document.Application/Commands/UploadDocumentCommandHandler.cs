using MediatR;
using WOL.Shared.Common.Application;
using WOL.Document.Domain.Repositories;

namespace WOL.Document.Application.Commands;

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, UploadDocumentResponse>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UploadDocumentCommandHandler(IDocumentRepository documentRepository, IUnitOfWork unitOfWork)
    {
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UploadDocumentResponse> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = Domain.Entities.Document.Create(
            request.UserId,
            request.DocumentType,
            request.DocumentNumber,
            request.FilePath,
            request.IssueDate,
            request.ExpiryDate);

        await _documentRepository.AddAsync(document, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UploadDocumentResponse
        {
            DocumentId = document.Id,
            Message = "Document uploaded successfully"
        };
    }
}
