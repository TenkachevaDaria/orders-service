using Contracts.Commands;
using Contracts.Events;
using InventoryService.Application.DTOs;
using InventoryService.Application.Handlers;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.API;

[Tags("Заказы")]
[ApiController]
[Route("api/products")]
public class InventoryController : Controller
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _inventoryService.GetProductsAsync();
        return Ok(products);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProducts([FromBody] CreateProductRequest request)
    {
        var result = await _inventoryService.CreateProductAsync(request);
        return result.Succeeded ? Ok(result) : StatusCode(422,result);
    }
    
    [HttpPatch("{productId:guid}/quantity")]
    public async Task<IActionResult> UpdateQuantity(Guid productId, [FromBody] UpdateQuantityRequest request)
    {
        var result = await _inventoryService.UpdateProductQuantity(request, productId);
        return result.Succeeded ? NoContent()  : StatusCode(422,result);
    }
}