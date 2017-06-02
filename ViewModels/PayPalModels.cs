using System.Configuration;

namespace GestioniDirette.Models
{
    public class PayPalModels
    {
        public string Cmd { get; set; }
        public string Business { get; set; }
        public string NoShipping { get; set; }
        public string Return { get; set; }
        public string CancelReturn { get; set; }
        public string NotifyUrl { get; set; }
        public string CurrencyCode { get; set; }
        public string ItemName { get; set; }
        public string Amount { get; set; }
        public string ActionUrl { get; set; }
        public PayPalModels(bool useSandbox)
        {
            Cmd = "_xclick";
            Business = ConfigurationManager.AppSettings["business"];
            CancelReturn = ConfigurationManager.AppSettings[" cancel_return"];
            Return = ConfigurationManager.AppSettings["return"];
            ActionUrl = useSandbox ? ConfigurationManager.AppSettings["test_url"] : ConfigurationManager.AppSettings["Prod_url"];
            NotifyUrl = ConfigurationManager.AppSettings["notify_url"];
            CurrencyCode = ConfigurationManager.AppSettings["currency_code"];
        }
    }
}
