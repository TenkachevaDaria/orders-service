
extern alias OrdersApi;
extern alias InventoryApi;
using Contracts.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrdersApi::OrderService.Application.Handlers;
using OrdersApi::OrderService.Application.Interfaces;
using OrdersApi::OrderService.Infrastructure.Persistence;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;
using Tests.E2E;
using Program = OrdersApi::Program;


namespace Tests.orders;

public class OrderWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<OrdersDbContext>) ||
                    d.ServiceType == typeof(OrdersDbContext) ||
                    d.ImplementationType == typeof(OrdersDbContext))
                .ToList();

            foreach (var d in descriptors)
                services.Remove(d);


            services.AddDbContext<OrdersDbContext>(options =>
            {
                options.UseInMemoryDatabase("OrdersTestDb");
            });


            services.AddScoped<IOrderDbContext>(sp =>
                sp.GetRequiredService<OrdersDbContext>());
            
            RebusReplacements.ReplaceRebus(services, cfg => cfg
                .Transport(t =>
                    t.UseInMemoryTransport(
                        TestRebusNetwork.Network,
                        "order_queue"))
                .Routing(r => r.TypeBased()
                    .Map<OrderCreatedEvent>("saga_queue"))
            );
            services.AutoRegisterHandlersFromAssemblyOf<OrderSucceededHandler>();
            services.AutoRegisterHandlersFromAssemblyOf<CancelOrderHandler>();
            services.AutoRegisterHandlersFromAssemblyOf<OrderPayingHandler>();
        });
    }
}