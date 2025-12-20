extern alias InventoryApi;
extern alias OrdersApi;
extern alias PaymentApi;
using System.Net.Http.Json;
using System.Text.Json;
using InventoryApi::InventoryService.Domain.Entities;
using InventoryApi::InventoryService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using OrdersApi::OrderService.Application.Common;
using PaymentApi::PaymentService.Domain.Entities;
using PaymentApi::PaymentService.Infrastructure.Persistence;
using Tests.inventory;
using Tests.orders;
using Tests.payment;

namespace Tests.E2E;

public class OrderSagaE2ETests : IAsyncLifetime
{
    private OrderWebApplicationFactory _orders = null!;
    private InventoryWebApplicationFactory _inventory = null!;
    private PaymentWebApplicationFactory _payment = null!;
    private SagaCoordinatorHost _saga = null!;
    private HttpClient _ordersClient = null!;
    private Guid _accountId = Guid.NewGuid();
    private Guid _productId = Guid.NewGuid();

    public async Task InitializeAsync()
    {
        _orders = new OrderWebApplicationFactory();
        _inventory = new InventoryWebApplicationFactory();
        _payment = new PaymentWebApplicationFactory();
        _saga = new SagaCoordinatorHost();

        _ordersClient = _orders.CreateClient();

        await SeedInventory();
    }
    
    private async Task SeedInventory()
    {
        
        using (var _paymentScope = _payment.Services.CreateScope())
        {
            var paymentdb = _paymentScope.ServiceProvider
                .GetRequiredService<PaymentDbContext>();

            paymentdb.Accounts.RemoveRange(paymentdb.Accounts);
            await paymentdb.SaveChangesAsync();
            paymentdb.Accounts.AddRange(
                new Account()
                {
                    Balance = 1000,
                    Id = _accountId,
                    FullName = "Aboba"
                }
            );

            await paymentdb.SaveChangesAsync();
        }
        
        using var scope = _inventory.Services.CreateScope();
        var db = scope.ServiceProvider
            .GetRequiredService<InventoryDbContext>();

        db.Products.Add(new Product
        {
            Id = _productId,
            Quantity = 10,
            Name = "Test Product",
            Price = 1
        });

        await db.SaveChangesAsync();
    }
    
    
    [Fact]
    public async Task Order_Should_End_With_Success_Status()
    {
        
        var request = new
        {
            accountId = _accountId,
            items = new[]
            {
                new
                {
                    productId = _productId,
                    quantity = 1
                }
            }
        };
        
        
        
        // POST order
        var response = await _ordersClient.PostAsJsonAsync(
            "api/orders",
            request);

        var responseJson = await response.Content.ReadAsStringAsync();

        var orderId = JsonSerializer.Deserialize<Result<Guid>>(
            responseJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // WAIT saga completion
        await WaitUntilAsync(async () =>
        {
            var get = await _ordersClient.GetAsync($"api/orders/{orderId.Data}");
            if (!get.IsSuccessStatusCode) return false;

            var body = await get.Content.ReadAsStringAsync();
            return body.Contains("Completed");
        });

        // FINAL ASSERT
        var final = await _ordersClient.GetAsync($"api/orders/{orderId.Data}");
        var result = await final.Content.ReadAsStringAsync();

        Assert.Contains("Completed", result);
    }

    public Task DisposeAsync()
    {
        _orders.Dispose();
        _inventory.Dispose();
        _payment.Dispose();
        _ = _saga.DisposeAsync();
        return Task.CompletedTask;
    }
    
    private static async Task WaitUntilAsync(
        Func<Task<bool>> condition,
        int timeoutMs = 250000)
    {
        var start = DateTime.UtcNow;

        while (DateTime.UtcNow - start <
               TimeSpan.FromMilliseconds(timeoutMs))
        {
            if (await condition()) return;
            await Task.Delay(100);
        }

        throw new TimeoutException("Saga did not complete");
    }
}