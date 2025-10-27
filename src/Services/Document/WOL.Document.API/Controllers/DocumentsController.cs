using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Document.Application.Commands;

namespace WOL.Document.API.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(IMediator mediator, ILogger<DocumentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<UploadDocumentResponse>> UploadDocument([FromBody] UploadDocumentCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document");
            return BadRequest(new { message = ex.Message });
        }
    }
}
