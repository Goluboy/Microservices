using Core.Messaging.Contracts;
using MassTransit;
using NotificationMicroservice.Application.Interfaces;
using NotificationMicroservice.Application.Services;

namespace NotificationListener.Consumers;

public class RegisterUserRequestedConsumer(IEmailService emailService) : IConsumer<RegisterUserRequested>
{
    public async Task Consume(ConsumeContext<RegisterUserRequested> context)
    {
        var msg = context.Message;

        await emailService.SendEmailAsync(new(msg.Email, "New account created", "You`re now registered"));

        await context.Publish(new RegisterUserCompleted
        {
            CorrelationId = msg.CorrelationId,
            UserId = msg.UserId ?? Guid.NewGuid(),
            Email = msg.Email
        });
    }
}