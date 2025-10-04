namespace NotificationMicroservice.Domain.Entities;

public class EmailNotification
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public EmailStatus Status { get; set; } = EmailStatus.Pending;
    public string? ErrorMessage { get; set; }
    public DateTime? SentAt { get; set; }

    public void MarkAsSent()
    {
        Status = EmailStatus.Sent;
        SentAt = DateTime.UtcNow;
        ErrorMessage = null;
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = EmailStatus.Failed;
        ErrorMessage = errorMessage;
    }
}

public enum EmailStatus
{
    Pending,
    Sent,
    Failed
}