using WOL.Shared.Common.Domain;

namespace WOL.Payment.Domain.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public DateTime InvoiceDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public string PaymentTerms { get; private set; } = string.Empty;
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Status { get; private set; } = "Unpaid";
    public DateTime? PaidAt { get; private set; }
    public string? PdfPath { get; private set; }

    private readonly List<InvoiceLineItem> _lineItems = new();
    public IReadOnlyCollection<InvoiceLineItem> LineItems => _lineItems.AsReadOnly();

    private Invoice() { }

    public static Invoice Create(
        Guid customerId,
        string paymentTerms,
        decimal taxRate = 0.15m)
    {
        var invoice = new Invoice
        {
            InvoiceNumber = GenerateInvoiceNumber(),
            CustomerId = customerId,
            InvoiceDate = DateTime.UtcNow,
            PaymentTerms = paymentTerms,
            Status = "Unpaid"
        };

        invoice.DueDate = paymentTerms switch
        {
            "NET30" => invoice.InvoiceDate.AddDays(30),
            "NET60" => invoice.InvoiceDate.AddDays(60),
            "DUE_ON_RECEIPT" => invoice.InvoiceDate,
            _ => invoice.InvoiceDate.AddDays(30)
        };

        return invoice;
    }

    public void AddLineItem(Guid bookingId, string description, decimal amount)
    {
        _lineItems.Add(new InvoiceLineItem
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            Description = description,
            Amount = amount
        });

        CalculateTotals();
        SetUpdatedAt();
    }

    public void MarkPaid()
    {
        Status = "Paid";
        PaidAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void SetPdfPath(string pdfPath)
    {
        PdfPath = pdfPath;
        SetUpdatedAt();
    }

    private void CalculateTotals()
    {
        SubTotal = _lineItems.Sum(li => li.Amount);
        TaxAmount = SubTotal * 0.15m;
        TotalAmount = SubTotal + TaxAmount;
    }

    private static string GenerateInvoiceNumber()
    {
        return $"INV{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
    }
}

public class InvoiceLineItem
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
