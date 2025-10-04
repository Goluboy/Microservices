using NotificationMicroservice.Application.Dtos;

namespace NotificationMicroservice.Application.Interfaces;

public interface IEmailService
{
    Task<EmailResponse> SendEmailAsync(EmailRequest emailDto);
    Task<EmailResponse> GetEmailStatusAsync(string emailId);
}