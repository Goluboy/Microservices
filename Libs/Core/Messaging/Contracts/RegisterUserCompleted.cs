namespace Core.Messaging.Contracts;
public record RegisterUserCompleted
{
    public Guid CorrelationId { get; init; }
    public Guid UserId { get; init; }
    public string Email { get; init; } = default!;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}