using Contracts.Commands;
using PaymentService.Application.Interfaces;
using Rebus.Handlers;

namespace PaymentService.Application.Handlers;

public class PayOrderHandler(IPaymentService payments) : IHandleMessages<PayOrderCommand>
{
    public async Task Handle(PayOrderCommand message)
    {
        await payments.PayAsync(message.AccountId, message.TotalPrice);
    }
}
