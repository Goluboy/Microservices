namespace NotificationMicroservice.Application.Dtos;

public record EmailRequest(string To, string Subject, string Body);