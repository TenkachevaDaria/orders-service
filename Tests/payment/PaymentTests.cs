extern alias OrdersApi;
extern alias PaymentApi;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PaymentApi::PaymentService.Application.DTOs;
using PaymentApi::PaymentService.Domain.Common;
using PaymentApi::PaymentService.Domain.Entities;
using PaymentApi::PaymentService.Infrastructure.Persistence;

namespace Tests.payment;

public class PaymentTests : IClassFixture<PaymentWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly IServiceProvider _services;
    
    public PaymentTests(PaymentWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _services = factory.Services;
    }

    [Fact]
    public async Task GetAccounts_Returns_Ok_And_Accounts()
    {
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<PaymentDbContext>();

            db.Accounts.RemoveRange(db.Accounts);
            await db.SaveChangesAsync();
            db.Accounts.AddRange(
                new Account()
                {
                    Balance = 1000,
                    Id = Guid.NewGuid(),
                    FullName = "Aboba"
                }
            );

            await db.SaveChangesAsync();
        }

        var response = await _client.GetAsync("api/accounts");


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products =
            JsonConvert.DeserializeObject<Result<List<AccountDto>>>(await response.Content.ReadAsStringAsync());

        Assert.NotNull(products);
        Assert.Equal(1, products!.Data.Count);
    }
}