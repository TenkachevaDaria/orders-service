extern alias InventoryApi;
using Contracts.Events;
using InventoryApi::InventoryService.Application.Handlers;
using InventoryApi::InventoryService.Infrastructure.Persistence;
using InventoryProgram = InventoryApi::Program;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;
using Tests.E2E;


namespace Tests.inventory;

public class InventoryWebApplicationFactory
    : WebApplicationFactory<InventoryProgram>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {

            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<InventoryDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);


            services.AddDbContext<InventoryDbContext>(options =>
                options.UseInMemoryDatabase("InventoryTestDb"));
            
            RebusReplacements.ReplaceRebus(services, cfg => cfg
                .Transport(t =>
                    t.UseInMemoryTransport(
                        TestRebusNetwork.Network,
                        "inventory_queue"))
                .Routing(r => r.TypeBased()
                    .Map<ItemsReservedEvent>("saga_queue")
                    .Map<ItemsReservationFailedEvent>("saga_queue"))
            );
            
            services.AutoRegisterHandlersFromAssemblyOf<OrderCreatedHandler>();
            services.AutoRegisterHandlersFromAssemblyOf<CancelReservationHandler>();
        });
    }
}