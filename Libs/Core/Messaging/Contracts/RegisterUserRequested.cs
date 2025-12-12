namespace Core.Messaging.Contracts;
public record RegisterUserRequested
{
    public Guid CorrelationId { get; init; }
    public Guid? UserId { get; init; }
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}