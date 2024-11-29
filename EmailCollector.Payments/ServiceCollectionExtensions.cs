using EmailCollector.Payments.Interfaces;
using EmailCollector.Payments.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EmailCollector.Payments;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentServices(this IServiceCollection services)
    {
        services.AddScoped<IPaymentProcessor, LemonSqueezyPaymentProcessor>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IPlanService, PlanService>();

        return services;
    }
}