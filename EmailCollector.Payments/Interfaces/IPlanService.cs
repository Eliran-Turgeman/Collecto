using EmailCollector.Payments.Models;

namespace EmailCollector.Payments.Interfaces;

public interface IPlanService
{
    Task<IEnumerable<Plan>> GetAllPlansAsync();
    Task<Plan> GetPlanByIdAsync(string planId);
}