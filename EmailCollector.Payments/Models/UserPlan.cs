namespace EmailCollector.Payments.Models;

public class UserPlan
{
    public string UserId { get; set; }
    public string PlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public UserPlanStatus Status { get; set; }
}