using EmailCollector.Payments.Interfaces;
using EmailCollector.Payments.Models;

namespace EmailCollector.Payments.Services;

/// <summary>
/// Source of truth for available plans.
/// </summary>
public class PlanService : IPlanService
{
    public Task<IEnumerable<Plan>> GetAllPlansAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Plan> GetPlanByIdAsync(string planId)
    {
        throw new NotImplementedException();
    }
}