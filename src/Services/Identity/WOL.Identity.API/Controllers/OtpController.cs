using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Identity.Application.Commands;
using WOL.Identity.Domain.Enums;

namespace WOL.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OtpController : ControllerBase
{
    private readonly IMediator _mediator;

    public OtpController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateOtp([FromBody] GenerateOtpRequest request)
    {
        var command = new GenerateOtpCommand(
            request.UserId,
            request.Purpose,
            request.PhoneNumber,
            request.Email);

        var code = await _mediator.Send(command);

        return Ok(new { message = "OTP generated successfully", code = code });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        var command = new VerifyOtpCommand(request.UserId, request.Code, request.Purpose);
        var isValid = await _mediator.Send(command);

        if (!isValid)
            return BadRequest(new { message = "Invalid or expired OTP" });

        return Ok(new { message = "OTP verified successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginWithOtp([FromBody] OtpLoginRequest request)
    {
        var verifyCommand = new VerifyOtpCommand(request.UserId, request.Code, OtpPurpose.Login);
        var isValid = await _mediator.Send(verifyCommand);

        if (!isValid)
            return BadRequest(new { message = "Invalid or expired OTP" });

        return Ok(new { message = "Login successful" });
    }
}

public record GenerateOtpRequest(Guid UserId, OtpPurpose Purpose, string? PhoneNumber = null, string? Email = null);
public record VerifyOtpRequest(Guid UserId, string Code, OtpPurpose Purpose);
public record OtpLoginRequest(Guid UserId, string Code);
