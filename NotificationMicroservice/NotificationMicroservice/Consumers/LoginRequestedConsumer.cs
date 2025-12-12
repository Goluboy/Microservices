using Core.Messaging.Contracts;
using MassTransit;
using NotificationMicroservice.Application.Interfaces;
using NotificationMicroservice.Application.Services;

namespace NotificationMicroservice.API.Consumers;

public class LoginRequestedConsumer(IEmailService emailService) : IConsumer<LoginRequested>
{
    public async Task Consume(ConsumeContext<LoginRequested> context)
    {
        var msg = context.Message;

        await emailService.SendEmailAsync(new(msg.Email, "New login to your account", "You`re now logged in"));

        await context.Publish(new LoginCompleted
        {
            CorrelationId = msg.CorrelationId,
            Email = msg.Email,
            Token = Guid.NewGuid().ToString()
        });
    }
}