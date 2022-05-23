namespace MrCMS.Models;

public class UserImpersonationResult
{
    public bool Success => string.IsNullOrWhiteSpace(Error);
    public string Error { get; set; }
}