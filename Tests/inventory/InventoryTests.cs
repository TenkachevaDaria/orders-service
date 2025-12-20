extern alias InventoryApi;
using System.Net;
using System.Text;
using System.Text.Json;
using InventoryApi::InventoryService.Application.Common;
using InventoryApi::InventoryService.Application.DTOs;
using InventoryApi::InventoryService.Domain.Entities;
using InventoryApi::InventoryService.Infrastructure.Persistence;
using Newtonsoft.Json;

using Microsoft.Extensions.DependencyInjection;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Tests.inventory;

public class InventoryTests : IClassFixture<InventoryWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly IServiceProvider _services;
    
    public InventoryTests(InventoryWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _services = factory.Services;
    }

    [Fact]
    public async Task GetProducts_Returns_Ok_And_Products()
    {
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<InventoryDbContext>();

            db.Products.RemoveRange(db.Products);
            await db.SaveChangesAsync();
            db.Products.AddRange(
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Product A",
                    Quantity = 10
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Product B",
                    Quantity = 5
                }
            );

            await db.SaveChangesAsync();
        }

 
        var response = await _client.GetAsync("api/products");


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products = JsonConvert.DeserializeObject<Result<List<ProductDto>>>(await response.Content.ReadAsStringAsync());

        Assert.NotNull(products);
        Assert.Equal(2, products!.Data.Count);

    }
    
    [Fact]
    public async Task PatchQuantity_NegativeValue_Returns_BadRequest()
    {
        var productId = Guid.NewGuid();
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<InventoryDbContext>();

            db.Products.AddRange(
                new Product
                {
                    Id = productId,
                    Name = "Product A",
                    Quantity = 10
                }
            );

            await db.SaveChangesAsync();
        }

        var payload = System.Text.Json.JsonSerializer.Serialize(new
        {
            Quantity = -1
        });

        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync(
            $"api/products/{productId}/quantity",
            content
        );
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task PatchQuantity_ValidRequest_UpdatesQuantity()
    {
        // Arrange
        Guid productId;

        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<InventoryDbContext>();

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test product",
                Price = 100,
                Quantity = 5
            };

            db.Products.Add(product);
            await db.SaveChangesAsync();

            productId = product.Id;
        }

        var payload = System.Text.Json.JsonSerializer.Serialize(new
        {
            quantity = 20
        });

        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        
        var response = await _client.PatchAsync(
            $"api/products/{productId}/quantity",
            content
        );
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<InventoryDbContext>();

            var updated = await db.Products.FindAsync(productId);

            Assert.NotNull(updated);
            Assert.Equal(20, updated!.Quantity);
        }
    }
    
    [Fact]
    public async Task CreateProduct_ValidRequest_Returns_Guid()
    {
        var request = new CreateProductRequest
        {
            Name = "New product",
            Price = 500,
            Quantity = 10
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _client.PostAsync("api/products/", content);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseJson = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<Result<Guid>>(
            responseJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotEqual(Guid.Empty, result.Data);
    }
}