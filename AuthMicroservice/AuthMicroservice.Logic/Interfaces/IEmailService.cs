using ConnectionLib.ConnectionServices.DtoModels.Email;

namespace AuthMicroservice.Logic.Interfaces;

/// <summary>
/// Interface for service for authentication-related operations.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sending email
    /// </summary>
    /// <param name="emailRequest"></param>
    /// <returns></returns>
    public Task<bool> SendEmailAsync(EmailRequest emailRequest);
}