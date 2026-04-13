using System;

namespace LegacyRenewalApp;

public class PaymentFeeService
{
    public (decimal paymentFee, string notes) Calculate(string method, decimal amount)
    {
        decimal paymentFee = 0;
        string notes = "";
        if (method == "CARD")
        {
            paymentFee = amount * 0.02m;
            notes = "card payment fee; ";
        }
        else if(method == "BANK_TRANSFER")
        {
            paymentFee = amount * 0.01m;
            notes = "bank transfer fee; ";
        }
        else if(method == "PAYPAL")
        {
            paymentFee = amount * 0.035m;
            notes = "paypal fee; ";
        }
        else if(method == "INVOICE")
        {
            notes = "invoice payment; ";
        }
        else
        {
            throw new ArgumentException("Unsupported payment method");
        }

        return (paymentFee, notes);
    }
}