namespace Contracts.Events;

public record PaymentSucceededEvent(Guid OrderId) : IEvent;
