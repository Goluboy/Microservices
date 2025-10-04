using NotificationMicroservice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace NotificationMicroservice.Infrastructure;

public class NotificationDbContext : DbContext
{
    public DbSet<EmailNotification> EmailNotifications { get; set; }
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options, ILogger<NotificationDbContext> logger)
        : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure EmailNotification entity
        modelBuilder.Entity<EmailNotification>(entity =>
        {
            // Primary Key
            entity.HasKey(e => e.Id);

            entity.Property(e => e.To)
                .IsRequired();

            entity.Property(e => e.Subject)
                .IsRequired();

            entity.Property(e => e.Body)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.Status)
                .IsRequired();
        });

        var emailStatusConverter = new ValueConverter<EmailStatus, string>(
            v => v.ToString(),
            v => (EmailStatus)Enum.Parse(typeof(EmailStatus), v));

        modelBuilder.Entity<EmailNotification>()
            .Property(e => e.Status)
            .HasConversion(emailStatusConverter);
    }
}