using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Models;
using Persistence;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionController : ControllerBase
{
    private readonly ISessionRepository _sessionRepository;

    public SessionController(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SessionResponse>>> GetSessions()
    {
        try
        {
            var sessions = await _sessionRepository.GetActiveSessionsAsync();
            var response = sessions.Select(s => new SessionResponse { Name = s.UserName, SessionId = s.Id });
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse
            {
                Code = "SESSION_ERROR",
                Message = $"Failed to retrieve sessions: {ex.Message}"
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<SessionResponse>> SetSession([FromBody] SessionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse
            {
                Code = "VALIDATION_ERROR",
                Message = "Name is required and cannot be empty"
            });
        }

        try
        {
            var userName = request.Name.Trim();
            var session = await _sessionRepository.CreateSessionAsync(userName);
            return Ok(new SessionResponse { Name = session.UserName, SessionId = session.Id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse
            {
                Code = "SESSION_ERROR",
                Message = $"Failed to create session: {ex.Message}"
            });
        }
    }

    [HttpDelete("{sessionId}")]
    public async Task<ActionResult> DeleteSession(string sessionId)
    {
        try
        {
            var deleted = await _sessionRepository.DeleteSessionAsync(sessionId);
            if (!deleted)
            {
                return NotFound(new ErrorResponse
                {
                    Code = "SESSION_NOT_FOUND",
                    Message = "Session not found"
                });
            }
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse
            {
                Code = "SESSION_ERROR",
                Message = $"Failed to delete session: {ex.Message}"
            });
        }
    }
}

public class SessionRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "Name cannot be empty")]
    public string Name { get; set; } = string.Empty;
}

public class SessionResponse
{
    public string Name { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
}

public class ErrorResponse
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}