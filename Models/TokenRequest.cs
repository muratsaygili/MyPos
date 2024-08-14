namespace MyPosTest.Models;

public class TokenRequest : BaseRequest
{
    public string ClientId { get; set; }
    
    public string ClientPassword { get; set; }
}