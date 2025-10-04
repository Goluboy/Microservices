namespace NotificationMicroservice.Application.Dtos;

public record EmailResponse(string Id, bool Success, string Message);