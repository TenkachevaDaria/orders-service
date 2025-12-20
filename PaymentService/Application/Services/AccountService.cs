using Microsoft.EntityFrameworkCore;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Common;
using PaymentService.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PaymentService.Application.Services;

public class AccountService(IPaymentDbContext context, ILogger<AccountService> logger) : IAccountService
{
    public async Task<Result<None>> AddMoneyAsync(Guid accountId, decimal money)
    {
        try
        {
            var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null)
            {
                logger.LogWarning($"Account with id {accountId} not found");
                return await Result<None>.FailureAsync("Account not found");
            }
            account.Balance = account.Balance + money;
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

    public async Task<Result<None>> DeleteAccountAsync(Guid id)
    {
        try
        {
            var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (account == null)
            {
                logger.LogWarning($"Account with id {id} not found");
                return await Result<None>.FailureAsync("Account not found");
            }
            context.Accounts.Remove(account);
            await context.SaveChangesAsync();
            logger.LogInformation($"Account with id {id} deleted");
            return await Result<None>.SuccessAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning($"Account with id {id} failed: {ex.Message}");
            return await Result<None>.FailureAsync(ex.Message);
        }
    }

    public async Task<Result<Guid>> CreateAccountAsync(CreateAccountRequest accountRequest)
    {
        var account =  new Account()
        {
            FullName = accountRequest.FullName,
            Balance = accountRequest.Balance,
        };
        
        context.Accounts.Add(account);
        
        await context.SaveChangesAsync();
        return  await Result<Guid>.SuccessAsync(account.Id);
    }

    public async Task<Result<List<AccountDto>>> GetAccountsAsync()
    {
        var data = await context.Accounts.ToListAsync();
        logger.LogInformation($"Returning {data.Count} accounts");
        return Result<List<AccountDto>>.Success(data.Select(a => new AccountDto() { Id = a.Id, FullName = a.FullName, Balance = a.Balance }).ToList());
    }
}
