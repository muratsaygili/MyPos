namespace MyPosTest.Models;

public class StartPaymentResponse
{
    public string IpCMethod { get; set; }
    
    public int Status { get; set; }
    
    public string StatusMsg { get; set; }
    
    public string Signature { get; set; }
}