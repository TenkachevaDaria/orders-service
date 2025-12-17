namespace Contracts.Commands;

public record PayOrderCommand(Guid OrderId, decimal TotalPrice);
