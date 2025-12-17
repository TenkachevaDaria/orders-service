using Contracts.Commands;
using InventoryService.Application.Interfaces;
using Rebus.Handlers;

namespace InventoryService.Application.Handlers
{
    public class CancelReservationHandler(IInventoryService inventoryService) : IHandleMessages<CancelReservationCommand>
    {
        public async Task Handle(CancelReservationCommand message)
        {
            await inventoryService.CancelReservationAsync(message.Items);
        }
    }
}
