namespace MyPosTest.Models;

public class BaseRequest
{
    public double Amount { get; set; }
    
    public string Currency { get; set; }
    
    public string OrderId { get; set; }
    
    public string PostUrl { get; set; }
}