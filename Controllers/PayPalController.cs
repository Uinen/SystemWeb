using GestioniDirette.Service.PayPal;
using PayPal.Api;
using System;
using System.Web.Mvc;
using GestioniDirette.Database.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using System.IO;
using PayPal;

namespace GestioniDirette.Controllers
{
    [Authorize]
    public class PayPalController : Controller
    {
        private readonly MyDbContext _db = new MyDbContext();
        /*
        public PayPalController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        */
        public ActionResult Index()
        {
            return View();
        }

        #region Billing Plan and subscription
        
        public ActionResult Subscribe()
        {
            try
            {
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var currentUser = userManager.FindById(User.Identity.GetUserId());

                var _profileInfo = _db.UserProfiles.Where(c => currentUser.ProfileId == c.ProfileId).SingleOrDefault();

                var plan = PayPalSubscriptionsService.CreateBillingPlan("Piano Standard", "Gestioni Dirette: Iscrizione ad un profilo Premium.", GetBaseUrl());

                var subscription = PayPalSubscriptionsService.CreateBillingAgreement(plan.id,
                    /*new ShippingAddress
                    {
                        recipient_name = _profileInfo.ProfileName,
                        city = _profileInfo.ProfileCity,
                        line1 = _profileInfo.ProfileAdress,
                        postal_code = _profileInfo.ProfileZipCode.ToString(),
                        country_code = "IT"

                    }*/ "Divisione Pagamenti - PayPal", "Gestioni Dirette", DateTime.Now);

                Database.Entity.PayPal paypal = new Database.Entity.PayPal()
                {
                    PayPalID = Guid.NewGuid(),
                    PlanID = plan.id,
                    UserID = currentUser.Id,
                    Nome = plan.name,
                    Stato = plan.state,
                    Tipo = plan.type,
                    CreatedDate = DateTime.Today.ToString("yyyy-MM-ddTHH:mm:ss") + "Z",
                    Update = DateTime.Today.ToString("yyyy-MM-ddTHH:mm:ss") + "Z"
                };

                _db.PayPal.Add(paypal);
                _db.SaveChanges();

                return Redirect(subscription.GetApprovalUrl(setUserActionParameter: true));
            }

            catch (ConnectionException ex)
            {
                if (ex is ConnectionException || ex is PayPalException)
                {
                    return View(ex.Response);
                }

                throw;
            }
        }

        public ActionResult SubscribeSuccess(string token)
        {
            try
            {
                PayPalSubscriptionsService.ExecuteBillingAgreement(token);
                if (token.Any())
                {
                    bool isPremium = true;
                    var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    var currentUser = userManager.FindById(User.Identity.GetUserId());

                    var user = _db.Users.Where(c => currentUser.Id == c.Id).SingleOrDefault();
                    user.isPremium = isPremium;

                    _db.Entry(user).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            catch (ConnectionException ex)
            {
                if (ex is ConnectionException || ex is PayPalException)
                {
                    return View(ex.Response);
                }

                throw;
            }
            return View();
        }

        public ActionResult SubscribeCancel(string token)
        {
            // TODO: Handle cancelled payment
            return RedirectToAction("Error");
        }

        /*
        public ActionResult SubscribeDelete(string agreementId)
        {
            PayPalSubscriptionsService.SuspendBillingAgreement(agreementId);
            return View();
        }*/

        #endregion

        #region WebHooks
        public ActionResult Webhook()
        {
            // The APIContext object can contain an optional override for the trusted certificate.
            var apiContext = Configuration.GetAPIContext();

            // Get the received request's headers
            var requestheaders = HttpContext.Request.Headers;

            // Get the received request's body
            var requestBody = string.Empty;
            using (var reader = new StreamReader(RequestBody()))
            {
                requestBody = reader.ReadToEnd();
            }

            dynamic jsonBody = JObject.Parse(requestBody);
            string webhookId = jsonBody.id;
            var ev = WebhookEvent.Get(apiContext, webhookId);

            // We have all the information the SDK needs, so perform the validation.
            // Note: at least on Sandbox environment this returns false.
            // var isValid = WebhookEvent.ValidateReceivedEvent(apiContext, ToNameValueCollection(requestheaders), requestBody, webhookId);

            switch (ev.event_type)
            {
                case "PAYMENT.CAPTURE.COMPLETED":
                    // Handle payment completed
                    break;
                case "PAYMENT.CAPTURE.DENIED":
                    // Handle payment denied
                    break;
                // Handle other webhooks
                default:
                    break;
            }

            return new HttpStatusCodeResult(200);
        }
        #endregion

        #region Helpers
        public string GetBaseUrl()
        {
            return Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/";
        }

        public string RequestBody()
        {
            var bodyStream = new StreamReader(System.Web.HttpContext.Current.Request.InputStream);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyText = bodyStream.ReadToEnd();
            return bodyText;
        }
        #endregion
    }
}