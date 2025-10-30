namespace ConnectionLib.ConnectionServices.DtoModels.Email;

public record EmailRequest(string To, string Subject, string Body);