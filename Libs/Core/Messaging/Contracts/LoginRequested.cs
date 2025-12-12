namespace Core.Messaging.Contracts;

public record LoginRequested
{
    public Guid CorrelationId { get; init; }
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}