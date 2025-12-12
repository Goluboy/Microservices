namespace Core.Messaging.Contracts;

public record LoginCompleted
{
    public Guid CorrelationId { get; init; }
    public string Email { get; init; } = default!;
    public string Token { get; init; } = default!;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}