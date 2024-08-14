namespace MyPosTest.Models;

public class MyPosNotifyResponse
{
    public string IPCmethod { get; set; }
    public string SID { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string OrderID { get; set; }
    public string IPC_Trnref { get; set; }
    public string RequestSTAN { get; set; }
    public DateTime RequestDateTime { get; set; }
    public int PaymentMethod { get; set; }
    public string BillingDescriptor { get; set; }
    public string PAN { get; set; }
    public string Signature { get; set; }

}