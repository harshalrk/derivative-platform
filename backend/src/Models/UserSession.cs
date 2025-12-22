namespace Models;

public class UserSession
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastAccessedAt { get; set; }
}

public class SessionCreateRequest
{
    public string UserName { get; set; } = string.Empty;
}

public class SessionResponse
{
    public string SessionId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}