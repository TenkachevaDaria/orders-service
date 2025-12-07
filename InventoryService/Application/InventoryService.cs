using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    
    public InventoryService(IInventoryDbContext inventoryDbContext, IMapper mapper)
    {
        _inventoryDbContext = inventoryDbContext;
        _mapper = mapper;
    }

    public async Task<Result<List<ProductDto>>> GetProductsAsync()
    {
        var products = await _inventoryDbContext.Products.ProjectTo<ProductDto>(_mapper.ConfigurationProvider).ToListAsync();
        return await Result<List<ProductDto>>.SuccessAsync(products);
    }

    public async Task<Result<Guid>> CreateProductAsync(CreateProductRequest request)
    {
        try
        {
            var product = _mapper.Map<Product>(request);
            await _inventoryDbContext.Products.AddAsync(product);
            await _inventoryDbContext.SaveChangesAsync();
            return await Result<Guid>.SuccessAsync(product.Id);
        }
        catch (Exception ex)
        {
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
            return await Result<None>.SuccessAsync();
        }
        catch (Exception ex)
        {
            return await Result<None>.FailureAsync(ex.Message);
        }
    }
}