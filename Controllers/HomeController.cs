using Microsoft.AspNetCore.Mvc;
using MyPosTest.Models;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace MyPosTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
	        var responseText =
		        "IPCmethod=IPCPurchaseNotify&SID=000000000000010&Amount=200.00&Currency=EUR&OrderID=MRT000020240730&IPC_Trnref=410227&RequestSTAN=083107&RequestDateTime=2024-07-30+14%3A49%3A48&PaymentMethod=1&BillingDescriptor=myPOS++%2A+TESTSTORE&PAN=0007&Signature=SfIVV0%2Fus5kXC3Nmcv%2FGYWyPrWOClW7kM40pWIGpk%2FgaZraoJK9ZY%2FkXI9FaiTBsJmVco8SvtpYMa3kx5qg1BggVn%2BLa5Ki1Bu7F5LUo%2FYSBZaNoEsHOnJ4x1oXTOLvJejnL8SuQ3fMLTEepzaZ5K28iymSzOoHohN4KGaefc5s%3D";
	        MyPosNotifyResponse notifyResponse = new MyPosNotifyResponse();
	        notifyResponse = ParseData(responseText);
			return View();
        }
        public static MyPosNotifyResponse ParseData(string data)
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
			        if (decimal.TryParse(value,CultureInfo.InvariantCulture, out var decimalValue))
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
			        if (DateTime.TryParseExact(value,"yyyy-MM-dd+HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dateValue))
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
		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
