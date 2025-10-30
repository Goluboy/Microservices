using AuthMicroservice.Logic.Interfaces;
using ConnectionLib.ConnectionServices.DtoModels.Email;
using ConnectionLib.ConnectionServices.Interfaces;

namespace AuthMicroservice.Logic.Services;

/// <summary>
/// Service for authentication-related operations
/// </summary>
/// <param name="connectionService"></param>
public class EmailService(IProfileConnectionService connectionService) : IEmailService
{
    /// <inheritdoc />
    public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
    {
        var response =  await connectionService.SendEmailAsync(emailRequest);
        return response.Success;
    }
}