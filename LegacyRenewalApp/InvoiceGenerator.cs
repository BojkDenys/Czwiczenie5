using System;

namespace LegacyRenewalApp;

public class InvoiceGenerator
{
    public RenewalInvoice Generate(
        Customer customer,
        string planCode,
        string paymentMethod,
        int seatCount,
        decimal baseAmount,
        decimal discount,
        decimal supportFee,
        decimal paymentFee,
        decimal taxAmount,
        decimal finalAmount,
        string notes
        )
    {
        return new RenewalInvoice
        {
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customer.Id}-{planCode}",
            CustomerName = customer.FullName,
            PlanCode = planCode,
            PaymentMethod = paymentMethod,
            SeatCount = seatCount,
            BaseAmount = Math.Round(baseAmount, 2),
            DiscountAmount = Math.Round(discount, 2),
            SupportFee = supportFee,
            PaymentFee = paymentFee,
            TaxAmount = taxAmount,
            FinalAmount = finalAmount,
            Notes = notes.Trim(),
            GeneratedAt = DateTime.UtcNow
        };
    }
}