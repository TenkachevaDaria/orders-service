using Contracts.Commands;
using Contracts.Events;
using InventoryService.Application.Interfaces;
using Rebus.Bus;
using Rebus.Handlers;

namespace InventoryService.Application.Handlers;

public class OrderCreatedHandler : IHandleMessages<ReserveItemsCommand>
{
    private readonly IInventoryService _inventoryService;
    private readonly IBus _bus;
    private readonly ILogger<OrderCreatedHandler> _logger;

    public OrderCreatedHandler(IInventoryService inventoryService, IBus bus, ILogger<OrderCreatedHandler> logger)
    {
        _inventoryService = inventoryService;
        _bus = bus;
        _logger = logger;
    }

    public async Task Handle(ReserveItemsCommand message)
    {
        var result = await _inventoryService.ReserveItemsAsync(message.Items, message.OrderId);

        if (result.Succeeded)
        {
            _logger.LogInformation(
                "Items reserved successfully for OrderId={OrderId}. ReservedItemsCount={Count}",
                message.OrderId,
                message.Items.Count
            );
            
//            await _bus.Send(evt);
        }
        else
        {
            _logger.LogWarning(
                "Failed to reserve items for OrderId={OrderId}. Errors: {Errors}",
                message.OrderId,
                string.Join(", ", result.Errors)
            );
            

            //            await _bus.Send(evt);
        }
    }
}