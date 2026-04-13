namespace LegacyRenewalApp;

public class TaxService
{
    public decimal GetTaxRate(string country)
    {
        if (country == "Poland")
        {
            return 0.23m;
        }
        else if(country == "Germany")
        {
            return 0.19m;
        }
        else if (country == "Czech Republic")
        {
            return 0.21m;
        }
        else if (country == "Norway")
        {
            return 0.25m;
        }

        return 0.20m;
    }
}