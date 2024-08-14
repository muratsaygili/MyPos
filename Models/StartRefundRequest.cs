using System.Text.Json.Serialization;

namespace MyPosTest.Models;

public class StartRefundRequest : BaseRequest
{
    public StartRefundRequest()
    {
        IPCMethod = "IPCRefund";
        IPCVersion = "1.4";
        IPCLanguage = "EN";
        KeyIndex = 1; //Indicates which key pair is being used.
        OutputFormat = "JSON";
    }
    [JsonPropertyName("IPC_Trnref")]
    public string IpcTrnRef { get; set; }

    public string OutputFormat { get; set; }
    public string IPCMethod { get; }

    public string IPCVersion { get; }

    public string IPCLanguage { get; }

    public string Sid { get; set; }

    public string WalletNumber { get; set; }

    public int KeyIndex { get; }
}