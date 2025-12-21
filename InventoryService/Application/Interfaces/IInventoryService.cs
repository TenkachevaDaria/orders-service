using Contracts.Events;
using InventoryService.Application.Common;
using InventoryService.Application.DTOs;

namespace InventoryService.Application.Interfaces;

public interface IInventoryService
{
    public Task<Result<List<ProductDto>>> GetProductsAsync();
    public Task<Result<Guid>> CreateProductAsync(CreateProductRequest request);
    public Task<Result<None>> UpdateProductQuantity(UpdateQuantityRequest request, Guid productId);
    public Task<Result<decimal>> ReserveItemsAsync(IReadOnlyList<OrderItemDto> itemDtos, Guid orderId);
    public Task<Result<None>> CancelReservationAsync(IReadOnlyList<OrderItemDto> itemDtos);
}