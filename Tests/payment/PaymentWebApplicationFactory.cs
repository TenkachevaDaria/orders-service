extern alias InventoryApi;
extern alias PaymentApi;
using Contracts.Events;
using InventoryApi::InventoryService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentApi::PaymentService.Application.Handlers;
using PaymentApi::PaymentService.Application.Interfaces;
using PaymentApi::PaymentService.Infrastructure.Persistence;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;
using Tests.E2E;

namespace Tests.payment;

extern alias PaymentApi;

public class PaymentWebApplicationFactory : WebApplicationFactory<PaymentApi::Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<PaymentDbContext>) ||
                    d.ServiceType == typeof(PaymentDbContext) ||
                    d.ImplementationType == typeof(PaymentDbContext))
                .ToList();

            foreach (var d in descriptors)
                services.Remove(d);


            services.AddDbContext<PaymentDbContext>(options => { options.UseInMemoryDatabase("OrdersTestDb"); });


            services.AddScoped<IPaymentDbContext>(sp =>
                sp.GetRequiredService<PaymentDbContext>()
            );

            RebusReplacements.ReplaceRebus(services, cfg => cfg
                .Transport(t =>
                    t.UseInMemoryTransport(
                        TestRebusNetwork.Network,
                        "payment_queue"))
                .Routing(r => r.TypeBased()
                    .Map<PaymentFailedEvent>("saga_queue")
                    .Map<PaymentSucceededEvent>("saga_queue")
                )
            );
            
            services.AutoRegisterHandlersFromAssemblyOf<PayOrderHandler>();
        });
    }
}