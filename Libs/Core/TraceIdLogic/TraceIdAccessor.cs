using Core.TraceLogic.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog.Context;

namespace Core.TraceIdLogic;

public interface ITraceIdAccessor
{
    
}

public static class StartUpTraceId
{
    public static IServiceCollection TryAddTraceId(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<TraceIdAccessor>();
        serviceCollection
            .TryAddScoped<ITraceWriter>(provider => provider.GetRequiredService<TraceIdAccessor>());
        serviceCollection
            .TryAddScoped<ITraceReader>(provider => provider.GetRequiredService<TraceIdAccessor>());
        serviceCollection
            .TryAddScoped<ITraceIdAccessor>(provider => provider.GetRequiredService<TraceIdAccessor>());

        return serviceCollection;
    }
}

internal class TraceIdAccessor : ITraceReader, ITraceWriter, ITraceIdAccessor, IDisposable
{
    public string Name => "TraceId";

    private string _value;
    private IDisposable? _logContextDisposable;

    public string GetValue()
    {
        return _value;
    }

    public void WriteValue(string value)
    {
        // на случай если это первый в цепочке сервис и до этого не было traceId
        if (string.IsNullOrWhiteSpace(value))
        {
            value = Guid.NewGuid().ToString();
        }

        _value = value;

        _logContextDisposable?.Dispose();
        _logContextDisposable = LogContext.PushProperty("TraceId", value);
    }

    public void Dispose()
    {
        _logContextDisposable?.Dispose();
        _logContextDisposable = null;
    }
}