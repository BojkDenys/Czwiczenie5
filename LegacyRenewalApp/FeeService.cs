using System;

namespace LegacyRenewalApp;

public class FeeService
{
    public (decimal fee, string notes) Calculate(string method, decimal amount)
    {
        decimal fee = 0;
        string notes = "";
        if (method == "CARD")
        {
            fee = amount * 0.02m;
            notes = "card payment fee; ";
        }
        else if(method == "BANK_TRANSFER")
        {
            fee = amount * 0.01m;
            notes = "bank transfer fee; ";
        }
        else if(method == "PAYPAL")
        {
            fee = amount * 0.035m;
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

        return (fee, notes);
    }
}