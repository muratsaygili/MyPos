using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using MyPosTest.ExtensionMethods;
using MyPosTest.Models;

namespace MyPosTest.Helpers;

public static class MyPosServiceHelper
{
    public static string PrepareRefundRequest(StartRefundRequest model)
    {
        var signatureDataDict = new Dictionary<string, string>();
        signatureDataDict.AddOrUpdate("IPCmethod", model.IPCMethod);
        signatureDataDict.AddOrUpdate("IPCVersion", model.IPCVersion);
        signatureDataDict.AddOrUpdate("IPCLanguage", model.IPCLanguage);
        signatureDataDict.AddOrUpdate("SID", model.Sid);
        signatureDataDict.AddOrUpdate("WalletNumber", model.WalletNumber);
        signatureDataDict.AddOrUpdate("KeyIndex", model.KeyIndex.ToString());
        signatureDataDict.AddOrUpdate("IPC_Trnref", model.IpcTrnRef);
        signatureDataDict.AddOrUpdate("Amount", model.Amount.ToString("F2", CultureInfo.InvariantCulture));
        signatureDataDict.AddOrUpdate("Currency", model.Currency);
        signatureDataDict.AddOrUpdate("OutputFormat", model.OutputFormat);
		
        StringBuilder request = new();
        var signature = CreateSignature(signatureDataDict, GetPrivateKey());
        request.Append(string.Join("&", signatureDataDict.Select(x => $"{x.Key}={HttpUtility.UrlEncode(x.Value)}")));
        request.Append($"&Signature={HttpUtility.UrlEncode(signature)}");

        return request.ToString();
    }
    public static string PrepareMyPosHtmlForm(StartPaymentRequest model,string postUrl)
	{
		var signatureDataDict = new Dictionary<string, string>();
		signatureDataDict.AddOrUpdate("IPCmethod", model.IPCMethod);
		signatureDataDict.AddOrUpdate("IPCVersion", model.IPCVersion);
		signatureDataDict.AddOrUpdate("IPCLanguage", model.IPCLanguage);
		signatureDataDict.AddOrUpdate("SID", model.Sid);
		signatureDataDict.AddOrUpdate("WalletNumber", model.WalletNumber);
		signatureDataDict.AddOrUpdate("Amount", model.Amount.ToString("F2", CultureInfo.InvariantCulture));
		signatureDataDict.AddOrUpdate("Currency", model.Currency);
		signatureDataDict.AddOrUpdate("OrderID", model.OrderId);
		signatureDataDict.AddOrUpdate("URL_OK", model.UrlOk);
		signatureDataDict.AddOrUpdate("URL_Cancel", model.UrlCancel);
		signatureDataDict.AddOrUpdate("URL_Notify", model.UrlNotify);
		signatureDataDict.AddOrUpdate("CardTokenRequest", model.CardTokenRequest.ToString());
		signatureDataDict.AddOrUpdate("PaymentParametersRequired", model.PaymentParametersRequired.ToString());
		signatureDataDict.AddOrUpdate("KeyIndex", model.KeyIndex.ToString());
		signatureDataDict.AddOrUpdate("ExpiresIn", model.ExpiresIn.ToString("F2", CultureInfo.InvariantCulture));
		signatureDataDict.AddOrUpdate("Note", model.Note);
		signatureDataDict.AddOrUpdate("CartItems", model.CartItems.ToString());
		if (model.CartItems > 0)
		{
			for (var i = 0; i < model.Carts.Count; i++)
			{
				signatureDataDict.AddOrUpdate($"Article_{i + 1}", model.Carts[i].Article);
				signatureDataDict.AddOrUpdate($"Quantity_{i + 1}", model.Carts[i].Quantity.ToString());
				signatureDataDict.AddOrUpdate($"Price_{i + 1}", model.Carts[i].Price.ToString("F2", CultureInfo.InvariantCulture));
				signatureDataDict.AddOrUpdate($"Amount_{i + 1}", model.Carts[i].Amount.ToString("F2", CultureInfo.InvariantCulture));
				signatureDataDict.AddOrUpdate($"Currency_{i + 1}", model.Carts[i].Currency);
			}
		}
		var signature = CreateSignature(signatureDataDict, GetPrivateKey());
		signatureDataDict.AddOrUpdate("Signature", signature);
		return ToHtmlForm(signatureDataDict, postUrl);
	}
	internal static string ToHtmlForm(this Dictionary<string, string> keyValuePairs, string link)
	{
		var input = "";

		foreach (var item in keyValuePairs)
		{
			input += $@"<input type=""hidden"" name=""{item.Key}"" value=""{item.Value}"" />" + Environment.NewLine;
		}

        return $"""
                <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
                <html xmlns="http://www.w3.org/1999/xhtml">
                <head><title>Redirecting...</title></head>
                <body>
                	<form action="{link}" name="3DFormPost" id="3DFormPost" method="post">
                	{input}
                	</form>
                	<script type="text/javascript">
                		document.getElementById('3DFormPost').submit();
                	</script>
                </body>
                </html>
                """;
	}
	private static string CreateSignature(Dictionary<string, string> signatureData, string privateKey)
	{
        var signature = Base64Encode(string.Join("-", signatureData.Select(x => x.Value)));
        var rsa = RSA.Create();
        using (var key = new MemoryStream(Encoding.ASCII.GetBytes(privateKey)))
        {
            using var pem = new PemUtils.PemReader(key);
            var rsaParameters = pem.ReadRsaKey();
            rsa.ImportParameters(rsaParameters);
        }
        var signDataByte = rsa.SignData(Encoding.UTF8.GetBytes(signature), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        return Convert.ToBase64String(signDataByte, 0, signDataByte.Length);
    }
	private static string Base64Encode(string plainText)
	{
		var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
		return Convert.ToBase64String(plainTextBytes);
	}
	private static string GetPrivateKey()
	{
		return "-----BEGIN RSA PRIVATE KEY-----MIICXAIBAAKBgQCf0TdcTuphb7X+Zwekt1XKEWZDczSGecfo6vQfqvraf5VPzcnJ2Mc5J72HBm0u98EJHan+nle2WOZMVGItTa/2k1FRWwbt7iQ5dzDh5PEeZASg2UWehoR8L8MpNBqH6h7ZITwVTfRS4LsBvlEfT7Pzhm5YJKfM+CdzDM+L9WVEGwIDAQABAoGAYfKxwUtEbq8ulVrD3nnWhF+hk1k6KejdUq0dLYN29w8WjbCMKb9IaokmqWiQ5iZGErYxh7G4BDP8AW/+M9HXM4oqm5SEkaxhbTlgks+E1s9dTpdFQvL76TvodqSyl2E2BghVgLLgkdhRn9buaFzYta95JKfgyKGonNxsQA39PwECQQDKbG0Kp6KEkNgBsrCq3Cx2od5OfiPDG8g3RYZKx/O9dMy5CM160DwusVJpuywbpRhcWr3gkz0QgRMdIRVwyxNbAkEAyh3sipmcgN7SD8xBG/MtBYPqWP1vxhSVYPfJzuPU3gS5MRJzQHBzsVCLhTBY7hHSoqiqlqWYasi81JzBEwEuQQJBAKw9qGcZjyMH8JU5TDSGllr3jybxFFMPj8TgJs346AB8ozqLL/ThvWPpxHttJbH8QAdNuyWdg6dIfVAa95h7Y+MCQEZgjRDl1Bz7eWGO2c0Fq9OTz3IVLWpnmGwfW+HyaxizxFhV+FOj1GUVir9hylV7V0DUQjIajyv/oeDWhFQ9wQECQCydhJ6NaNQOCZh+6QTrH3TC5MeBA1Yeipoe7+BhsLNrcFG8s9sTxRnltcZl1dXaBSemvpNvBizn0Kzi8G3ZAgc=-----END RSA PRIVATE KEY-----";
	}
}