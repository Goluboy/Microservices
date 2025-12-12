namespace ConnectionLib.ConnectionServices.DtoModels.Email;

public record EmailResponse(Guid Id, bool Success, string Message);