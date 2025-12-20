extern alias OrdersApi;
extern alias InventoryApi;
using System.Net;
using InventoryApi::InventoryService.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OrdersApi::OrderService.Application.Common;
using OrdersApi::OrderService.Application.Services.DTOs;
using OrdersApi::OrderService.Domain.Entities;
using OrdersApi::OrderService.Infrastructure.Persistence;

namespace Tests.orders;

public class OrdersTests : IClassFixture<OrderWebApplicationFactory>
{
    
    private readonly HttpClient _client;
    private readonly IServiceProvider _services;
    
    public OrdersTests(OrderWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _services = factory.Services;
    }

    [Fact]
    public async Task GetOrders_Returns_Ok_And_Orders()
    {
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<OrdersDbContext>();

            db.Orders.RemoveRange(db.Orders);
            await db.SaveChangesAsync();
            db.Orders.AddRange(
                new Order()
                {
                    Id = Guid.NewGuid(),
                    AccountId = Guid.NewGuid(),
                    Comment = "Some comment",
                    Items = new List<OrderItem>()
                }
            );

            await db.SaveChangesAsync();
        }

        var response = await _client.GetAsync("api/orders");


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products =
            JsonConvert.DeserializeObject<List<OrderDto>>(await response.Content.ReadAsStringAsync());

        Assert.NotNull(products);
        Assert.Equal(1, products!.Count);
    }
}