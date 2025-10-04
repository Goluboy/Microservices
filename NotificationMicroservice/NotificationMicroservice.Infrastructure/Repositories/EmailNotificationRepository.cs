using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotificationMicroservice.Domain.Entities;
using NotificationMicroservice.Domain.Interface;

namespace NotificationMicroservice.Infrastructure.Repositories;

public class EmailNotificationRepository : IEmailNotificationRepository
{
    private readonly NotificationDbContext _context;

    public EmailNotificationRepository(
        NotificationDbContext context,
        ILogger<EmailNotificationRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<EmailNotification> CreateAsync(EmailNotification notification)
    {
        var entry = await _context.EmailNotifications.AddAsync(notification);

        return entry.Entity;
    }

    public async Task<EmailNotification?> GetByIdAsync(string id)
    {
        var notification = await _context.EmailNotifications
            .FirstOrDefaultAsync(e => e.Id == id);

        return notification;
    }

    public async Task<EmailNotification> UpdateAsync(EmailNotification notification)
    {
        _context.EmailNotifications.Update(notification);

        return notification;
    }

    public async Task<IEnumerable<EmailNotification>> GetByStatusAsync(EmailStatus status)
    {
        var notifications = await _context.EmailNotifications
                .Where(e => e.Status == status)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

        return notifications;
    }

    public async Task<IEnumerable<EmailNotification>> GetRecentAsync(int count = 10)
    {
        var notifications = await _context.EmailNotifications
                .OrderByDescending(e => e.CreatedAt)
                .Take(count)
                .ToListAsync();

         return notifications;
    }

    public async Task<IEnumerable<EmailNotification>> GetByRecipientAsync(string recipient)
    {
        var notifications = await _context.EmailNotifications
                .Where(e => e.To.ToLower() == recipient.ToLower())
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

        return notifications;
    }

    public async Task<int> GetCountByStatusAsync(EmailStatus status)
    {
        var count = await _context.EmailNotifications
                .CountAsync(e => e.Status == status);

        return count;
    }

    public async Task DeleteAsync(EmailNotification notification)
    {
        _context.EmailNotifications.Remove(notification);
    }
}
