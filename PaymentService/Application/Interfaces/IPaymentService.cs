using PaymentService.Domain.Common;

namespace PaymentService.Application.Interfaces;

public interface IPaymentService
{
    public Task<Result<None>> PayAsync(Guid accountId, decimal price);
}
