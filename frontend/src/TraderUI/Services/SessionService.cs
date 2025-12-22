using Microsoft.Extensions.Logging;

namespace TraderUI.Services;

public class SessionService
{
    private readonly ILogger<SessionService> _logger;
    private string? _currentUserName;
    private string? _sessionId;
    
    public SessionService(ILogger<SessionService> logger)
    {
        _logger = logger;
    }
    
    public string? CurrentUserName => _currentUserName;
    public string? SessionId => _sessionId;
    public bool IsUserSet => !string.IsNullOrEmpty(_currentUserName);
    
    public event Action? OnSessionChanged;
    
    public void SetUser(string userName, string? sessionId = null)
    {
        _logger.LogInformation("[SessionService] SetUser called: userName='{UserName}', sessionId='{SessionId}'", userName, sessionId);
        _currentUserName = userName;
        _sessionId = sessionId ?? userName;
        _logger.LogInformation("[SessionService] After SetUser: IsUserSet={IsUserSet}, CurrentUserName='{CurrentUserName}'", IsUserSet, CurrentUserName);
        OnSessionChanged?.Invoke();
    }
    
    public void ClearUser()
    {
        _logger.LogInformation("[SessionService] ClearUser called. Was: IsUserSet={IsUserSet}, CurrentUserName='{CurrentUserName}'", IsUserSet, CurrentUserName);
        _currentUserName = null;
        _sessionId = null;
        _logger.LogInformation("[SessionService] After ClearUser: IsUserSet={IsUserSet}", IsUserSet);
        OnSessionChanged?.Invoke();
    }
}