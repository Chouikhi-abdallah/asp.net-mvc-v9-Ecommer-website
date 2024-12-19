using Stripe;
using Stripe.Checkout;
namespace Proj.Services;

public class StripePaymentService
{
    public StripePaymentService()
    {
        StripeConfiguration.ApiKey = "sk_test_51QVhqBAL3cisUWy3XF6Ne7HSN9mFkudY1kq3M76WDDqy0xLkJ0duEa2QCk2c4YZyuB3kBlEZ2HcNji5FP25wnKzh007WvFwLaE";
    }

    public Session CreateCheckoutSession(decimal totalAmount, string successUrl, string cancelUrl)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Order Payment",
                        },
                        UnitAmountDecimal = totalAmount * 100, // Convert to cents
                    },
                    Quantity = 1,
                }
            },
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
        };

        var service = new SessionService();
        return service.Create(options);
    }
    
}