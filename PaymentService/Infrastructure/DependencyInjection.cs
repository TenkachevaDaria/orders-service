using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Interfaces;
using PaymentService.Infrastructure.Persistence;

namespace PaymentService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddContext(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConnectionString = configuration.GetConnectionString("PaymentDb");
        if (string.IsNullOrWhiteSpace(databaseConnectionString))
            throw new ArgumentException("Database connection string is not initialized");
        services.AddDbContext<PaymentDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PaymentDb")));
        services.AddScoped<IPaymentDbContext, PaymentDbContext>();
        return services;
    }
}
