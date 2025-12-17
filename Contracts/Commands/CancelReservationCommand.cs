using Contracts.Events;

namespace Contracts.Commands;

public record CancelReservationCommand(Guid OrderId, IReadOnlyList<OrderItemDto> Items);
