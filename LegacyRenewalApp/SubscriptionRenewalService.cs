using System;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly CustomerRepository _customerRepository;
        private readonly SubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly DiscountService _discountService;
        private readonly PaymentFeeService _paymentFeeService;
        private readonly TaxService _taxService;
        private readonly InvoiceGenerator _invoiceGenerator;
        private readonly IBillingGateway _billingGateway;
        private readonly SupportFeeService _supportFeeService;

        public SubscriptionRenewalService()
        {
            _customerRepository = new CustomerRepository();
            _subscriptionPlanRepository = new SubscriptionPlanRepository();
            _discountService = new DiscountService();
            _paymentFeeService = new PaymentFeeService();
            _taxService = new TaxService();
            _invoiceGenerator = new InvoiceGenerator();
            _billingGateway = new BillingGatewayImpl();
            _supportFeeService = new SupportFeeService();
        }
        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            if (customerId <= 0)
            {
                throw new ArgumentException("Customer id must be positive");
            }

            if (string.IsNullOrWhiteSpace(planCode))
            {
                throw new ArgumentException("Plan code is required");
            }

            if (seatCount <= 0)
            {
                throw new ArgumentException("Seat count must be positive");
            }

            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                throw new ArgumentException("Payment method is required");
            }

            string planCodeNorm = planCode.Trim().ToUpper();
            string paymentMethodNorm = paymentMethod.Trim().ToUpper();
            Customer customer = _customerRepository.GetById(customerId);
            SubscriptionPlan subscriptionPlan = _subscriptionPlanRepository.GetByCode(planCodeNorm);
            if (!customer.IsActive)
            {
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");
            }

            decimal baseAmount = (subscriptionPlan.MonthlyPricePerSeat * seatCount * 12m) + subscriptionPlan.SetupFee;
            var (discount, notes) =
                _discountService.CalculateDiscount(customer, subscriptionPlan, seatCount, baseAmount, useLoyaltyPoints);
            decimal subtotal = baseAmount - discount;
            if (subtotal < 300)
            {
                subtotal = 300;
                notes += "minimum discounted subtotal applied; ";
            }

            decimal supportFee = 0;
            if (includePremiumSupport)
            {
                supportFee = _supportFeeService.GetSupportFee(planCodeNorm);
                notes += "premium support included; ";
            }

            subtotal += supportFee;
            var (paymentFee, feeNotes) = _paymentFeeService.Calculate(paymentMethodNorm, subtotal);
            notes += feeNotes;
            decimal taxRate = _taxService.GetTaxRate(customer.Country);
            decimal taxBase = subtotal + supportFee + paymentFee;
            decimal taxAmount = taxBase * taxRate;
            decimal finalAmount = taxBase + taxAmount;
            if (finalAmount < 500)
            {
                finalAmount = 500;
                notes += "minimum invoice amount applied; ";
            }

            RenewalInvoice renewalInvoice = _invoiceGenerator.Generate(
                customer,
                planCodeNorm,
                paymentMethodNorm,
                seatCount,
                baseAmount,
                discount,
                supportFee,
                paymentFee,
                taxAmount,
                finalAmount,
                notes
            );
            _billingGateway.SaveInvoice(renewalInvoice);
            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                
                string subject = "Subscription renewal invoice";
                string body =
                    $"Hello {customer.FullName}, your renewal for plan {planCodeNorm} " +
                    $"has been prepared. Final amount: {finalAmount:F2}.";
                _billingGateway.SendEmail(customer.Email,subject, body);
            }

            return renewalInvoice;
        }
    }
}
