using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using MyPosTest.Helpers;
using MyPosTest.Models;
using Newtonsoft.Json;
using RestSharp;

namespace MyPosTest.Controllers
{
    [Route("mypos")]
    public class MyPosController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var postUrl = "https://www.mypos.com/vmp/checkout-test";
            var model = new StartPaymentRequest()
            {
                Sid = "000000000000010",
                WalletNumber = "61938166610",
                Amount = (double)200,
                Currency = "EUR",
                OrderId = "MRT000"+DateTime.Now.Ticks,
                UrlOk = Url.ActionLink("OkResult"),
                UrlCancel = Url.ActionLink("CancelResult"),
				UrlNotify = Url.ActionLink("NotifyResult"),
				PaymentMethod = 1,
                Note = "test",
                Customer = new Customer()
                {
                    FirstName = "Murat",
                    FamilyName = "Saygili",
                    Email = "muratsaygili1@gmail.com",
                },
                Carts = new List<Cart>()
                {
                    new ()
                    {
                        Article = "Cart Item",
                        Quantity = 2,
                        Price = (double)100,
                        Currency = "EUR"
                    }
                }
            };
            object htmlForm = MyPosServiceHelper.PrepareMyPosHtmlForm(model, postUrl);
            return View(htmlForm);
        }

        [Route("notify")]
        public IActionResult NotifyResult()
        {
	        var response = MapToMyPosNotifyResponse(Request);
            //process payment with response data
			return Ok("OK");
        }

        [Route("ok")]
        public async Task<IActionResult> OkResult()
        {
	        var response = MapToMyPosNotifyResponse(Request);
			TempData["Message"] = "Successful";

            //try refund
            var postUrl = "https://www.mypos.com/vmp/checkout-test";
            var refundRequest = new StartRefundRequest()
            {
                Sid = "000000000000010",
                WalletNumber = "61938166610",
                Amount = (double)200,
                Currency = "EUR",
                OrderId = response.OrderID,
                IpcTrnRef = response.IPC_Trnref
            };
            var refundRequestStr = MyPosServiceHelper.PrepareRefundRequest(refundRequest);
            using var client = new RestClient();
            var request = new RestRequest(postUrl, Method.Post);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddStringBody(refundRequestStr, DataFormat.None);
            var refundResponse = await client.ExecuteAsync(request);
            object refundResponseContent = refundResponse.Content;
            TempData["response"] = JsonConvert.SerializeObject(refundResponseContent);
            TempData["requestStr"] = refundRequestStr;

            return RedirectToAction("Index", "Home");
        }
        [Route("cancel")]
        public IActionResult CancelResult()
        {
	        var response = MapToMyPosNotifyResponse(Request);
	        TempData["Message"] = "Failed";
			return RedirectToAction("Index", "Home");
        }
        
        private MyPosNotifyResponse MapToMyPosNotifyResponse(HttpRequest pMessage)
        {
	        var reader = new StreamReader(pMessage.Body);
	        var content = reader.ReadToEndAsync().Result;
	        var data = ParseData(content);
	        return data;
        }
		private static MyPosNotifyResponse ParseData(string data)
		{
			var properties = typeof(MyPosNotifyResponse).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var transaction = new MyPosNotifyResponse();
			var keyValuePairs = data.Split('&');

			foreach (var pair in keyValuePairs)
			{
				var keyValue = pair.Split('=');
				if (keyValue.Length != 2) continue;

				var key = keyValue[0];
				var value = Uri.UnescapeDataString(keyValue[1]);

				var property = Array.Find(properties, p => p.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));
				if (property == null) continue;

				if (property.PropertyType == typeof(decimal))
				{
					if (decimal.TryParse(value, CultureInfo.InvariantCulture, out var decimalValue))
					{
						property.SetValue(transaction, decimalValue);
					}
				}
				else if (property.PropertyType == typeof(int))
				{
					if (int.TryParse(value, out var intValue))
					{
						property.SetValue(transaction, intValue);
					}
				}
				else if (property.PropertyType == typeof(DateTime))
				{
					if (DateTime.TryParseExact(value, "yyyy-MM-dd+HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dateValue))
					{
						dateValue = dateValue.ToLocalTime();
						property.SetValue(transaction, dateValue);
					}
				}
				else
				{
					property.SetValue(transaction, value);
				}
			}

			return transaction;
		}
	}
}
