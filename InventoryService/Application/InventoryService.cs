using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts.Events;
using InventoryService.Application.Common;
using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Application;

public class InventoryService : IInventoryService
{
    private readonly IInventoryDbContext _inventoryDbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(IInventoryDbContext inventoryDbContext, IMapper mapper, ILogger<InventoryService> logger)
    {
        _inventoryDbContext = inventoryDbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<ProductDto>>> GetProductsAsync()
    {
        var products = await _inventoryDbContext.Products.ProjectTo<ProductDto>(_mapper.ConfigurationProvider).ToListAsync();
        _logger.LogInformation($"Returning {products.Count} products");
        return await Result<List<ProductDto>>.SuccessAsync(products);
    }

    public async Task<Result<Guid>> CreateProductAsync(CreateProductRequest request)
    {
        try
        {
            var product = _mapper.Map<Product>(request);
            await _inventoryDbContext.Products.AddAsync(product);
            await _inventoryDbContext.SaveChangesAsync();
            _logger.LogInformation($"Created product: {product.Id}");
            return await Result<Guid>.SuccessAsync(product.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to create product: {ex.Message}");
            return await Result<Guid>.FailureAsync(ex.Message);
        }
    }

    public async Task<Result<None>> UpdateProductQuantity(UpdateQuantityRequest request, Guid productId)
    {
        try
        {
            var product = _inventoryDbContext.Products.Find(productId);
            if (product == null)
            {
                return await Result<None>.FailureAsync("Product not found");
            }
            product.Quantity = request.Quantity;
            await _inventoryDbContext.SaveChangesAsync();
            _logger.LogInformation($"Updated product: {product.Id}");
            return await Result<None>.SuccessAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to update product: {ex.Message}");
            return await Result<None>.FailureAsync(ex.Message);
        }
    }

    public async Task<Result<decimal>> ReserveItemsAsync(IReadOnlyList<OrderItemDto> itemDtos)
    {
        try
        {
            decimal totalPrice = 0;
            foreach (var item in itemDtos)
            {
                var itemResult = await ReserveItemAsync(item);
                if (!itemResult.Succeeded)
                {
                    _logger.LogError("Failed to reserve item");
                    return await Result<decimal>.FailureAsync($"Ошибка {string.Join(',', itemResult.Errors)}");
                }
                totalPrice += itemResult.Data;
            }
            await _inventoryDbContext.SaveChangesAsync();
            return await Result<decimal>.SuccessAsync(totalPrice);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to reserve items: {ex.Message}");
            return await Result<decimal>.FailureAsync(ex.Message);
        }
    }
    
    private async Task<Result<decimal>> ReserveItemAsync(OrderItemDto request)
    {
        var product = _inventoryDbContext.Products.Find(request.ProductId);
        if (product == null)
        {
            return await Result<decimal>.FailureAsync("Product not found");
        }

        if (product.Quantity >= request.Quantity)
        {
            product.Quantity -= request.Quantity;
        }
        else
        {
            return await Result<decimal>.FailureAsync("Quantity too low");
        }
        return await Result<decimal>.SuccessAsync(product.Price * request.Quantity);
    }
}