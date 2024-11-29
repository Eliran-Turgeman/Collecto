using EmailCollector.Payments.Models;

namespace EmailCollector.Payments.Interfaces;

public interface IPaymentProcessor
{
    Task<string> ProcessPaymentAsync(string userId, Plan plan);
    Task CancelSubscriptionAsync(string userId);
}