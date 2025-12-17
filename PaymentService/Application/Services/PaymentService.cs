using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Common;

namespace PaymentService.Application.Services;

public class PaymentService(IPaymentDbContext context, ILogger<PaymentService> logger) : IPaymentService
{
    public async Task<Result<None>> PayAsync(Guid accountId, decimal price)
    {
        try
        {
            var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null)
            {
                logger.LogWarning($"Account with id {accountId} not found");
                return await Result<None>.FailureAsync("Account not found");
            }
            if (account.Balance < price)
            {
                logger.LogWarning($"On account with id {accountId} not enough {price - account.Balance} money");
                return await Result<None>.FailureAsync("On account with id {accountId} not enough money");
            }
            account.Balance = account.Balance - price;
            await context.SaveChangesAsync();
            logger.LogInformation($"Account with id {accountId} balance changed to {account.Balance}");
            return await Result<None>.SuccessAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning($"Account with id {accountId} failed: {ex.Message}");
            return await Result<None>.FailureAsync(ex.Message);
        }
    }
}
