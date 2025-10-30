using System.Linq;
using Core.TraceLogic.Interfaces;
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
        var readers = context.RequestServices.GetServices<ITraceReader>();
        var writers = context.RequestServices.GetServices<ITraceWriter>();

        foreach (var reader in readers)
        {
            reader.WriteValue(context.Request.Headers[reader.Name].FirstOrDefault());
        }

        context.Response.OnStarting(() =>
        {
            foreach (var writer in writers)
            {
                var value = writer.GetValue();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    context.Response.Headers[writer.Name] = value;
                }
            }
            return Task.CompletedTask;
        });

        await _next(context);   
    }
}