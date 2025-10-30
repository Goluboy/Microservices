using ConnectionLib.ConnectionServices.DtoModels.Email;

namespace ConnectionLib.ConnectionServices.Interfaces;

public interface IProfileConnectionService
{
    Task<EmailResponse> SendEmailAsync(EmailRequest emailRequest);
}