using Contracts.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;
using SagaCoordinator;

namespace Tests.E2E;

public sealed class SagaCoordinatorHost : IAsyncDisposable
{
    private readonly IHost _host;

    public SagaCoordinatorHost()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // --------------------
                // Logging
                // --------------------
                services.AddLogging(cfg =>
                {
                    cfg.AddConsole();
                    cfg.SetMinimumLevel(LogLevel.Information);
                });

                // --------------------
                // Rebus (IN-MEMORY)
                // --------------------
                services.AddRebus(configure => configure
                    .Transport(t =>
                        t.UseInMemoryTransport(
                            TestRebusNetwork.Network,
                            "saga_queue"))
                    .Routing(r => r.TypeBased()
                        .Map<ReserveItemsCommand>("inventory_queue")
                        .Map<CancelReservationCommand>("inventory_queue")
                        .Map<CancelOrderCommand>("order_queue")
                        .Map<OrderPayingCommand>("order_queue")
                        .Map<OrderSucceededCommand>("order_queue")
                        .Map<PayOrderCommand>("payment_queue"))
                    .Sagas(s => s.StoreInMemory())
                    .Options(o =>
                    {
                        o.LogPipeline();
                        o.SetNumberOfWorkers(1);
                        o.SetMaxParallelism(1);
                    }),
                    isDefaultBus: true
                );

                // --------------------
                // Register Saga
                // --------------------
                services.AutoRegisterHandlersFromAssemblyOf<OrderSaga>();
            })
            .Build();

        _host.Start();
    }

    public async ValueTask DisposeAsync()
    {
        await _host.StopAsync();
        _host.Dispose();
    }
}