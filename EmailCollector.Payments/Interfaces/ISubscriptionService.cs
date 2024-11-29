using EmailCollector.Payments.Models;

namespace EmailCollector.Payments.Interfaces;

public interface ISubscriptionService
{
    Task SubscribeAsync(string userId, string planId);
    Task CancelSubscriptionAsync(string userId);
    Task UpgradeSubscriptionAsync(string userId, string newPlanId);
    Task DowngradeSubscriptionAsync(string userId, string newPlanId);
    Task<UserPlan> GetSubscriptionAsync(string userId);
}