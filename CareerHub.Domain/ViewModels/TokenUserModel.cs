namespace CareerHub.Domain.ViewModels;

public class TokenUserModel
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; } = false;
    public DateTimeOffset LastLogin { get; set; }
}
