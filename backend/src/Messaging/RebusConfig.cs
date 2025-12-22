using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.RabbitMq;

namespace Messaging;

public static class RebusConfig
{
    public static IServiceCollection AddRebusMessaging(this IServiceCollection services, string connectionString, string queueName = "trader-queue")
    {
        services.AddRebus(configure => configure
            .Transport(t => t.UseRabbitMq(connectionString, queueName))
            .Start());

        return services;
    }
}