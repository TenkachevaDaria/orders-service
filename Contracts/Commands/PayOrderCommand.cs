namespace Contracts.Commands;

public record PayOrderCommand(Guid OrderId, Guid AccountId, decimal TotalPrice);
