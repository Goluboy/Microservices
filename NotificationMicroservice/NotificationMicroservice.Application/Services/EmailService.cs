using ConnectionLib.ConnectionServices.DtoModels.Email;
using NotificationMicroservice.Application.Interfaces;
using NotificationMicroservice.Domain.Entities;
using NotificationMicroservice.Domain.Interface;

namespace NotificationMicroservice.Application.Services;

public class EmailService(
    IEmailSender emailSender,
    IEmailNotificationRepository repository) : IEmailService
{
    public async Task<EmailResponse> SendEmailAsync(EmailRequest emailDto)
    {
        var notification = new EmailNotification
        {
            To = emailDto.To,
            Subject = emailDto.Subject,
            Body = emailDto.Body
        };

        await repository.CreateAsync(notification);

        await emailSender.SendEmailAsync(emailDto.To, emailDto.Subject, emailDto.Body);

        // Mark as sent
        notification.MarkAsSent();
        await repository.UpdateAsync(notification);


        return new EmailResponse(
            notification.Id,
            true,
            "Email sent successfully"
        );

    }

    public async Task<EmailResponse> GetEmailStatusAsync(string emailId)
    {
        var notification = await repository.GetByIdAsync(emailId);
        if (notification == null)
        {
            return new EmailResponse
            (
                 emailId,
                 false,
                 "Email notification not found"
            );
        }

        return new EmailResponse
        (
            notification.Id,
            notification.Status == EmailStatus.Sent,
            notification.Status switch
            {
                EmailStatus.Pending => "Email is pending",
                EmailStatus.Sent => "Email was sent successfully",
                EmailStatus.Failed => $"Email failed: {notification.ErrorMessage}",
                _ => "Unknown status"
            }
        );
    }
}
