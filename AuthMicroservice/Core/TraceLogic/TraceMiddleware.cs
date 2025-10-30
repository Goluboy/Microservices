using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Core.TraceLogic;

public sealed class TraceMiddleware
{
    private readonly RequestDelegate _next;

    public TraceMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var readers = context.RequestServices.GetServices<Core.TraceLogic.Interfaces.ITraceReader>();
        var writers = context.RequestServices.GetServices<Core.TraceLogic.Interfaces.ITraceWriter>();

        foreach (var reader in readers)
        {
            reader.WriteValue(context.Request.Headers[reader.Name].FirstOrDefault());
        }

        await _next(context);

        foreach (var writer in writers)
        {
            var value = writer.GetValue();
            if (!string.IsNullOrWhiteSpace(value))
            {
                context.Response.Headers[writer.Name] = value;
            }
        }
    }
}