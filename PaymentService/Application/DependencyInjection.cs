using PaymentService.Application.Interfaces;
using PaymentService.Application.Services;

namespace PaymentService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IPaymentService, Services.PaymentService>();
        return services;
    }
}
