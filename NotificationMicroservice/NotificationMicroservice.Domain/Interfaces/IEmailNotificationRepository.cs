using NotificationMicroservice.Domain.Entities;

namespace NotificationMicroservice.Domain.Interface;

public interface IEmailNotificationRepository
{
    Task<EmailNotification> CreateAsync(EmailNotification notification);
    Task<EmailNotification?> GetByIdAsync(string id);
    Task<EmailNotification> UpdateAsync(EmailNotification notification);
    Task<IEnumerable<EmailNotification>> GetByStatusAsync(EmailStatus status);
}
