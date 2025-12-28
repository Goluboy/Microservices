using Microsoft.Extensions.Logging;
using MimeKit;
using NotificationMicroservice.Application.Interfaces;
using NotificationMicroservice.Domain.Settings;

public class MailKitEmailService : IEmailSender
{
    private readonly ILogger<MailKitEmailService> _logger;

    public MailKitEmailService(ILogger<MailKitEmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = CreateMimeMessage(to, subject, body, false);

        _logger.LogInformation("Email to: {To}, Subject: {Subject}, Body: {Body}", to, subject, body);

        await Task.CompletedTask;
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