using ConnectionLib.ConnectionServices.DtoModels.Email;
using ConnectionLib.ConnectionServices.Interfaces;
using Core.Messaging.Contracts;
using MassTransit;

namespace ConnectionLib.ConnectionServices;

public class ProfileConnectionService(IPublishEndpoint publishEndpoint) : IProfileConnectionService
{
    public async Task<EmailResponse> SendEmailAsync(EmailRequest emailRequest)
    {

        return null;
    }
}
