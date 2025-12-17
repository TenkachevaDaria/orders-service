namespace Contracts.Commands;

public record PayOrderCommand(Guid AccountId, decimal TotalPrice);
