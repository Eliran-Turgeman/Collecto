namespace EmailCollector.Payments.Models;

public class Plan
{
    public string PlanId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int MaxMonthlySubmissions { get; set; }
    public int MaxForms { get; set; }
}