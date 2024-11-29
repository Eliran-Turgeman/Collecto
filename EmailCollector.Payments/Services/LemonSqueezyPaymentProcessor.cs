using EmailCollector.Payments.Interfaces;
using EmailCollector.Payments.Models;

namespace EmailCollector.Payments.Services;

/// <summary>
/// Processing subscription payments using lemonsqueezy.
/// </summary>
public class LemonSqueezyPaymentProcessor : IPaymentProcessor
{
    public Task<string> ProcessPaymentAsync(string userId, Plan plan)
    {
        throw new NotImplementedException();
    }

    public Task CancelSubscriptionAsync(string userId)
    {
        throw new NotImplementedException();
    }
}