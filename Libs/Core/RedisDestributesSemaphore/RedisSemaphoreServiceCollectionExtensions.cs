using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.RedisDestributesSemaphore
{
    public static class RedisSemaphoreServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisDistributedSemaphore(
            this IServiceCollection services,
            string semaphoreKey = "example_lock",
            int maxCount = 5)
        {
            var multiplexer = ConnectionMultiplexer.Connect("localhost:6379");

            services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            services.AddScoped<IDistributedSemaphore>(sp =>
            {
                var db = multiplexer.GetDatabase();
                return new RedisDistributedSemaphore(semaphoreKey, maxCount, db);
            });

            return services;
        }
    }
}
