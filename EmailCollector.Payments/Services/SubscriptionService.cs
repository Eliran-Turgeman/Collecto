using EmailCollector.Payments.Interfaces;
using EmailCollector.Payments.Models;

namespace EmailCollector.Payments.Services;

/// <summary>
/// Handles user interactions in regard to subscriptions
/// </summary>
public class SubscriptionService : ISubscriptionService
{
    public Task SubscribeAsync(string userId, string planId)
    {
        throw new NotImplementedException();
    }

    public Task CancelSubscriptionAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task UpgradeSubscriptionAsync(string userId, string newPlanId)
    {
        throw new NotImplementedException();
    }

    public Task DowngradeSubscriptionAsync(string userId, string newPlanId)
    {
        throw new NotImplementedException();
    }

    public Task<UserPlan> GetSubscriptionAsync(string userId)
    {
        throw new NotImplementedException();
    }
}