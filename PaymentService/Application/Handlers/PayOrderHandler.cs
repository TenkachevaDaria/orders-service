using Contracts.Commands;
using Contracts.Events;
using PaymentService.Application.Interfaces;
using Rebus.Bus;
using Rebus.Handlers;

namespace PaymentService.Application.Handlers;

public class PayOrderHandler(IPaymentService payments) : IHandleMessages<PayOrderCommand>
{
    public async Task Handle(PayOrderCommand message)
    {
        try
        {
            await payments.PayAsync(message.AccountId, message.TotalPrice, message.OrderId);
//            await bus.Send(new PaymentSucceededEvent(message.OrderId));
        }
        catch (Exception e)
        {
//            await bus.Send(new PaymentFailedEvent(message.OrderId, e.Message));
        }
    }
}
