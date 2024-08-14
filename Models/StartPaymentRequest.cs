namespace MyPosTest.Models;

public class StartPaymentRequest : BaseRequest
{
    // https://developers.mypos.com/en/doc/online_payments/v1_4/21-purchase-with-payment-card-(api-call--ipcpurchase)
    public StartPaymentRequest()
    {
        IPCMethod = "IPCPurchase";
        IPCVersion = "1.4";
        IPCLanguage = "EN";
        KeyIndex = 1; //Indicates which key pair is being used.
        CardTokenRequest = 0; // Do not request a payment card token
        PaymentParametersRequired = 3; // Simplified request & Simplified payment page
        ExpiresIn = 60 * 10; // 10 minutes, Custom set time (in seconds) how long the session lifetime should be for the payment page
    }
    public string IPCMethod { get; }

    public string IPCVersion { get; }

    public string IPCLanguage { get; }

    public string Sid { get; set; }

    public string WalletNumber { get; set; }

    public int KeyIndex { get; }

    public string UrlOk { get; set; }

    public string UrlCancel { get; set; }

    public string UrlNotify { get; set; }

    public int PaymentMethod { get; set; }

    /// <summary>
    /// **CardTokenRequest**
    /// 0 – Do not request a payment card token
    /// 1 – Store new card and request a token. Not applicable if PaymentMethod = 2. Disables PaymentMethod = 3. 
    /// 2 – Pay with a card and request a token. Not applicable if PaymentMethod = 2. Disables PaymentMethod = 3. 
    ///
    /// Token will be available in IPCPurchaseNotify callback.
    /// </summary>
    public int CardTokenRequest { get; }

    public int PaymentParametersRequired { get; set; }

    public Customer Customer { get; set; }

    public double ExpiresIn { get; }

    public string Note { get; set; }

    public int CartItems => Carts?.Count ?? 0;

    public List<Cart> Carts { get; set; }

}

public class Customer
{
    public string Email { get; set; }

    public string FirstName { get; set; }

    public string FamilyName { get; set; }
}

public class Cart
{
    public string Article { get; set; }

    public int Quantity { get; set; }

    public double Price { get; set; }

    public double Amount => Quantity * Price;

    public string Currency { get; set; }
}

