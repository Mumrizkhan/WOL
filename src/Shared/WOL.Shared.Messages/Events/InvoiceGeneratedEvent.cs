namespace WOL.Shared.Messages.Events;

public class InvoiceGeneratedEvent
{
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string PaymentTerms { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? PdfPath { get; set; }
    public DateTime Timestamp { get; set; }
}
