using NotificationMicroservice.Application.Interfaces;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using NotificationMicroservice.Domain.Settings;


namespace NotificationMicroservice.Infrastructure.Services;

public class MailKitEmailService : IEmailSender
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = CreateMimeMessage(to, subject, body, false);

        using var client = new SmtpClient();

        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
        
        await client.ConnectAsync(
            EmailSettings.SmtpHost,
            EmailSettings.SmtpPort,
            EmailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

        if (!string.IsNullOrEmpty(EmailSettings.SmtpUsername))
        {
            await client.AuthenticateAsync(EmailSettings.SmtpUsername, EmailSettings.SmtpPassword);
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    private MimeMessage CreateMimeMessage(string to, string subject, string body, bool isHtml)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(EmailSettings.FromName, EmailSettings.FromEmail));
        message.To.Add(new MailboxAddress(to, to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder();
        if (isHtml)
        {
            bodyBuilder.HtmlBody = body;
        }
        else
        {
            bodyBuilder.TextBody = body;
        }

        message.Body = bodyBuilder.ToMessageBody();
        return message;
    }
}
