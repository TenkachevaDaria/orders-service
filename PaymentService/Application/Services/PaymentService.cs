using Contracts.Events;
using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Common;
using PaymentService.Domain.Entities;

namespace PaymentService.Application.Services;

public class PaymentService(IPaymentDbContext context, ILogger<PaymentService> logger) : IPaymentService
{
    public async Task<Result<None>> PayAsync(Guid accountId, decimal price, Guid orderId)
    {
        try
        {
            var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null)
            {
                logger.LogWarning($"Account with id {accountId} not found");
                AddSagaFailureEvent(orderId,$"Account with id {accountId} not found");
                await context.SaveChangesAsync();
                return await Result<None>.FailureAsync("Account not found");
            }
            if (account.Balance < price)
            {
                logger.LogWarning($"On account with id {accountId} not enough {price - account.Balance} money");
                AddSagaFailureEvent(orderId,$"On account with id {accountId} not enough {price - account.Balance} money");
                return await Result<None>.FailureAsync("On account with id {accountId} not enough money");
            }
            account.Balance = account.Balance - price;
            AddSagaSuccessEvent(orderId);
            await context.SaveChangesAsync();
            logger.LogInformation($"Account with id {accountId} balance changed to {account.Balance}");
            return await Result<None>.SuccessAsync();
        }
        catch (Exception ex)
        {
            AddSagaFailureEvent(orderId,ex.Message);
            logger.LogWarning($"Account with id {accountId} failed: {ex.Message}");
            await context.SaveChangesAsync();
            return await Result<None>.FailureAsync(ex.Message);
        }
    }
    
    private void AddSagaSuccessEvent(Guid orderId)
    {
        context.OutboxMessages.Add(
            OutboxMessage.ForSend(new PaymentSucceededEvent(orderId), "saga"));
    }

    private void AddSagaFailureEvent(Guid orderId, string reason)
    {
        context.OutboxMessages.Add(
            OutboxMessage.ForSend(
                new PaymentFailedEvent(orderId, reason),
                destination: "saga_queue"
            )
        );
    }
}
