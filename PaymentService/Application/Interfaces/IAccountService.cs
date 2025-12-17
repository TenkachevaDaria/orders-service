using PaymentService.Application.DTOs;
using PaymentService.Domain.Common;

namespace PaymentService.Application.Interfaces;

public interface IAccountService
{
    public Task<Result<List<AccountDto>>> GetAccountsAsync();
    public Task<Result<None>> AddMoneyAsync(Guid accountId, decimal money);
    public Task<Result<None>> DeleteAccountAsync(Guid id);
}
