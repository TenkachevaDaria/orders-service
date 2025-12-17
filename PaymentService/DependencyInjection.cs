using Contracts.Events;
using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Handlers;
using PaymentService.Application.Interfaces;
using PaymentService.Infrastructure.Persistence;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using System.Text.Json.Serialization;

namespace PaymentService;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.AddEndpointsApiExplorer().AddSwaggerGen();
        return services;
    }

    public static IServiceCollection AddTransport(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddRebus(configure => configure
            .Routing(r => r.TypeBased().Map<PaymentFailedEvent>("saga_queue").Map<PaymentSucceededEvent>("saga_queue"))
            .Transport(t => t.UseRabbitMq(configuration["Rabbit:ConnectionString"]!, "payment_queue"))
            .Options(o =>
            {
                o.LogPipeline();
                o.SetNumberOfWorkers(1);
                o.SetMaxParallelism(1);
            }))
            .AutoRegisterHandlersFromAssemblyOf<PayOrderHandler>();
        return services;
    }
}
