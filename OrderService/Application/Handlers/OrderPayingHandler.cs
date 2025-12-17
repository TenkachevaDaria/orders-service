using Contracts.Commands;
using OrderService.Application.Interfaces;
using OrderService.Domain.Enums;
using Rebus.Handlers;

namespace OrderService.Application.Handlers;

public class OrderPayingHandler(IOrderService _orderService) : IHandleMessages<OrderPayingCommand>
{
    public async Task Handle(OrderPayingCommand message)
    {
        await _orderService.UpdateOrderStatus(message.OrderId, OrderStatus.Paying);
    }
}
