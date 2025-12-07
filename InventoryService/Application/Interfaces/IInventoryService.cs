using InventoryService.Application.Common;
using InventoryService.Application.DTOs;
using InventoryService.Domain.Entities;

namespace InventoryService.Application.Interfaces;

public interface IInventoryService
{
    public Task<Result<List<ProductDto>>> GetProductsAsync();
    public Task<Result<Guid>> CreateProductAsync(CreateProductRequest request);
    public Task<Result<None>> UpdateProductQuantity(UpdateQuantityRequest request, Guid productId);
}