using System;
using System.Configuration;
using System.Web.Mvc;
using SystemWeb.Models;

namespace SystemWeb.Controllers
{
    public class PayPalController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RedirectFromPaypal()
        {
            return View();
        }
        public ActionResult CancelFromPaypal()
        {
            return View();
        }
        public ActionResult NotifyFromPaypal()
        {
            return View();
        }
        public ActionResult ValidateCommand(string product, string totalPrice)
        {
            bool useSandbox = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSandbox"]);
            var paypal = new PayPalModels(useSandbox)
            {
                ItemName = product,
                Amount = totalPrice
            };
            return View(paypal);
        }
    }
}
