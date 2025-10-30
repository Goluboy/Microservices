using System.Net.Http.Json;
using ConnectionLib.ConnectionServices.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ConnectionLib.ConnectionServices.DtoModels.Email;
using Core.HttpLogic.Services;
using Core.HttpLogic.Services.Interfaces;
namespace ConnectionLib.ConnectionServices;

public class ProfileConnectionService : IProfileConnectionService
{
    private readonly IHttpRequestService _requestService;

    public ProfileConnectionService(IHttpRequestService requestService)
    {
        _requestService = requestService;
    }

    public async Task<EmailResponse> SendEmailAsync(EmailRequest emailRequest)
    {
        var requestData = new HttpRequestData
        {
            Uri = new Uri("https://localhost:7004/api/Notification/send-email"),
            Method = HttpMethod.Post,
            Body = emailRequest
        };

        var response = await _requestService.SendRequestAsync<EmailResponse>(requestData);

        return response.Body;
    }
}
