namespace LegacyRenewalApp;

public class SupportFeeService
{
    public decimal GetSupportFee(string planCode)
    {
        decimal supportFee = 0;
        if (planCode == "START")
        {
            supportFee = 250;
        }
        else if(planCode == "Pro")
        {
            supportFee = 400;
        }
        else if(planCode == "ENTERPRISE")
        {
            supportFee = 700;
        }

        return supportFee;
    }
}