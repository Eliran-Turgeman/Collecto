namespace EmailCollector.Payments.Models;

public class Invoice
{
    public string InvoiceId { get; set; }
    public string UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } // e.g., Paid, Pending, Failed
}