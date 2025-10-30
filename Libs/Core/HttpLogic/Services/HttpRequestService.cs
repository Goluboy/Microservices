using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Core.HttpLogic.Services;
using Core.HttpLogic.Services.Interfaces;
using Core.TraceLogic.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Microsoft.AspNetCore.Http;

namespace Core.HttpLogic.Services;

public enum ContentType
{
    ///
    Unknown = 0,

    ///
    ApplicationJson = 1,

    ///
    XWwwFormUrlEncoded = 2,

    ///
    Binary = 3,

    ///
    ApplicationXml = 4,

    ///
    MultipartFormData = 5,

    /// 
    TextXml = 6,

    /// 
    TextPlain = 7,

    ///
    ApplicationJwt = 8
}

public record HttpRequestData
{
    /// <summary>
    /// Тип метода
    /// </summary>
    public HttpMethod Method { get; set; }

    /// <summary>
    /// Адрес запроса
    /// </summary>\
    public Uri Uri { set; get; }

    /// <summary>
    /// Тело метода
    /// </summary>
    public object Body { get; set; }

    /// <summary>
    /// content-type, указываемый при запросе
    /// </summary>
    public ContentType ContentType { get; set; } = ContentType.ApplicationJson;

    /// <summary>
    /// Заголовки, передаваемые в запросе
    /// </summary>
    public IDictionary<string, string> HeaderDictionary { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Коллекция параметров запроса
    /// </summary>
    public ICollection<KeyValuePair<string, string>> QueryParameterList { get; set; } =
        new List<KeyValuePair<string, string>>();
}

public record BaseHttpResponse
{
    /// <summary>
    /// Статус ответа
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// Заголовки, передаваемые в ответе
    /// </summary>
    public HttpResponseHeaders Headers { get; set; }

    /// <summary>
    /// Заголовки контента
    /// </summary>
    public HttpContentHeaders ContentHeaders { get; init; }

    /// <summary>
    /// Является ли статус код успешным
    /// </summary>
    public bool IsSuccessStatusCode
    {
        get
        {
            var statusCode = (int)StatusCode;

            return statusCode >= 200 && statusCode <= 299;
        }
    }
}

public record HttpResponse<TResponse> : BaseHttpResponse
{
    /// <summary>
    /// Тело ответа
    /// </summary>
    public TResponse Body { get; set; }
}

/// <inheritdoc />
internal class HttpRequestService : IHttpRequestService
{
    private readonly IHttpConnectionService _httpConnectionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<ITraceWriter> _writers;

    ///
    public HttpRequestService(
        IHttpConnectionService httpConnectionService,
        IServiceProvider serviceProvider,
        IEnumerable<ITraceWriter> writers)
    {
        _httpConnectionService = httpConnectionService;
        _serviceProvider = serviceProvider;
        _writers = writers;
    }

    /// <inheritdoc />
    public async Task<HttpResponse<TResponse>> SendRequestAsync<TResponse>(HttpRequestData requestData,
        HttpConnectionData connectionData)
    {
        var client = _httpConnectionService.CreateHttpClient(connectionData);

        var httpRequestMessage = new HttpRequestMessage
        {
            Method = requestData.Method,
            RequestUri = requestData.Uri
        };

        if (requestData.Body is not null)
        {
            httpRequestMessage.Content = PrepareContent(requestData.Body, requestData.ContentType);
        }

        foreach (var kv in requestData.HeaderDictionary)
        {
            httpRequestMessage.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
        }

        foreach (var traceWriter in _writers)
        {
            var value = traceWriter.GetValue();
            if (!string.IsNullOrWhiteSpace(value))
            {
                httpRequestMessage.Headers.TryAddWithoutValidation(traceWriter.Name, value);
            }
        }

        // Polly 
        var retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .OrResult(msg => ((int)msg.StatusCode) >= 500)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
                );

        var res = await retryPolicy.ExecuteAsync(ct =>
            _httpConnectionService.SendRequestAsync(httpRequestMessage, client, ct),
            connectionData.CancellationToken);

        var response = new HttpResponse<TResponse>
        {
            StatusCode = res.StatusCode,
            Headers = res.Headers,
            ContentHeaders = res.Content?.Headers
        };

        if (res.Content != null)
        {
            var contentString = await res.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                response.Body = JsonConvert.DeserializeObject<TResponse>(contentString);
            }
        }

        return response;
    }

    private static HttpContent PrepareContent(object body, ContentType contentType)
    {
        switch (contentType)
        {
            case ContentType.ApplicationJson:
            {
                if (body is string stringBody)
                {
                    body = JToken.Parse(stringBody);
                }

                var serializeSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };
                var serializedBody = JsonConvert.SerializeObject(body, serializeSettings);
                var content = new StringContent(serializedBody, Encoding.UTF8, MediaTypeNames.Application.Json);
                return content;
            }

            case ContentType.XWwwFormUrlEncoded:
            {
                if (body is not IEnumerable<KeyValuePair<string, string>> list)
                {
                    throw new Exception(
                        $"Body for content type {contentType} must be {typeof(IEnumerable<KeyValuePair<string, string>>).Name}");
                }

                return new FormUrlEncodedContent(list);
            }
            case ContentType.ApplicationXml:
            {
                if (body is not string s)
                {
                    throw new Exception($"Body for content type {contentType} must be XML string");
                }

                return new StringContent(s, Encoding.UTF8, MediaTypeNames.Application.Xml);
            }
            case ContentType.Binary:
            {
                if (body.GetType() != typeof(byte[]))
                {
                    throw new Exception($"Body for content type {contentType} must be {typeof(byte[]).Name}");
                }

                return new ByteArrayContent((byte[])body);
            }
            case ContentType.TextXml:
            {
                if (body is not string s)
                {
                    throw new Exception($"Body for content type {contentType} must be XML string");
                }

                return new StringContent(s, Encoding.UTF8, MediaTypeNames.Text.Xml);
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null);
        }
    }
}