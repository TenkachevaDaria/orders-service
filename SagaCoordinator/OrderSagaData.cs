using Contracts.Events;
using Rebus.Sagas;

namespace SagaCoordinator;

public class OrderSagaData : ISagaData
{
    public Guid Id { get; set; }
    public int Revision { get; set; }

    public Guid OrderId { get; set; }
    public Guid AccountId { get; set; }
    public decimal? TotalPrice { get; set; }
    public IReadOnlyList<OrderItemDto> Items { get; set; }
}