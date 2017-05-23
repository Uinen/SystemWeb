using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using SystemWeb.Service.Static;
using Syncfusion.EJ.Export;
using Syncfusion.XlsIO;
using Syncfusion.JavaScript.Models;
using System.Web.Script.Serialization;
using System.Reflection;
using Microsoft.AspNet.Identity.Owin;
using SystemWeb.Services;
using SystemWeb.Database.Entity;
using SystemWeb.Database.Repository.Interface;
using System.Data.Entity;
using System.IO;
using SystemWeb.Mail;
using System.Configuration;
using RazorEngine;

namespace SystemWeb.Controllers
{
    public class CartissimaController : Controller
    {
        #region Inizializzatori
        private readonly MyDbContext _db = new MyDbContext();
        private readonly iCartissimaRepository _cartissimaRepository;
        private readonly IBrowserConfigService _browserConfigService;
        private readonly IFeedService _feedService;
        private readonly IManifestService _manifestService;
        private readonly IOpenSearchService _openSearchService;
        private readonly IRobotsService _robotsService;
        private readonly ISitemapService _sitemapService;

        public CartissimaController(IBrowserConfigService browserConfigService,
            IFeedService feedService,
            IManifestService manifestService,
            IOpenSearchService openSearchService,
            IRobotsService robotsService,
            ISitemapService sitemapService)
        {
            //_cartissimaRepository = new CartissimaRepository(new MyDbContext());
            this._browserConfigService = browserConfigService;
            this._feedService = feedService;
            this._manifestService = manifestService;
            this._openSearchService = openSearchService;
            this._robotsService = robotsService;
            this._sitemapService = sitemapService;
        }

        public CartissimaController(ApplicationUserManager userManager)
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
        #endregion
        
        [Authorize(Roles = "Administrator")]
        [Route("inserzioni/")]
        public ActionResult Inserzioni()
        {
            var dataSource = _db.Cartissima.OrderBy(o => o.sCartCreateDate).ToList();
            ViewBag.datasource = dataSource;

            IEnumerable dataSource2 = _db.Pv.ToList();
            ViewBag.datasource2 = dataSource2;

            return View();
        }

        [Route("inserzioni/businesscard/", Name = CartissimaControllerRoute.GetBusinessCard)]
        [HttpGet]
        public ActionResult BusinessCard()
        {
            return View();
        }

        [Route("inserzioni/businesscard/", Name = CartissimaControllerRoute.PostBusinessCard)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BusinessCard(Cartissima value)
        {
            if (ModelState.IsValid)
            {
                _db.Cartissima.Add(value);
                _db.SaveChanges();

                if (value.sCartEmail != null)
                {
                    string body;

                    using (var sr = new StreamReader(Server.MapPath("/App_Data/Templates/Business-Card-Success.txt")))
                    {
                        body = sr.ReadToEnd();
                    }

                    var sCartName = HttpUtility.UrlEncode(value.sCartName);
                    var sCartSurname = HttpUtility.UrlEncode(value.sCartSurname);
                    var sCartCompany = HttpUtility.UrlEncode(value.sCartCompany);
                    var sCartIva = HttpUtility.UrlEncode(value.sCartIva.ToString());
                    var sCartVeichleType = HttpUtility.UrlEncode(value.sCartVeichleType);
                    var sCartVeichle = HttpUtility.UrlEncode(value.sCartVeichle.ToString());
                    var sCartId = HttpUtility.UrlEncode(value.sCartId.ToString());

                    var sender = ConfigurationManager.AppSettings["SenderEmail"];
                    var emailSubject = "Gestioni Dirette - La tua Richiesta";
                    string messageBody = Razor.Parse(body, value);
                    var mesagge = new Message
                    {
                        Sender = sender,
                        Recipient = value.sCartEmail,
                        RecipientCC = sender,
                        Subject = emailSubject,
                        AttachmentFile = null,
                        Body = messageBody
                    };

                    mesagge.Send();

                    return RedirectToAction("RichiestaInviata", new { key = value.sCartId });
                }

                else
                {
                    return RedirectToAction("RichiestaInviata", new { key = value.sCartId });
                }
            }

            return View();
        }

        [Route("inserzioni/businesscard/richiestainviata", Name = CartissimaControllerRoute.GetRichiestaInviata)]
        public ActionResult RichiestaInviata(Guid key)
        {
            var _getCode = (from a in _db.Cartissima
                            where key == a.sCartId
                           select a.sCartId).SingleOrDefault();

            ViewBag.Cod = _getCode;

            return View();
        }

        [Authorize(Roles = "Administrator")]
        [Route("inserzioni/update")]
        public ActionResult Update(Cartissima value)
        {
            _db.Entry(value).State = EntityState.Modified;
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        
        [Authorize(Roles = "Administrator")]
        [Route("inserzioni/insert")]
        public ActionResult Insert(Cartissima value)
        {
            _db.Cartissima.Add(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        
        [Authorize(Roles = "Administrator")]
        [Route("inserzioni/remove")]
        public ActionResult Remove(Guid key)
        {
            _db.Cartissima.Remove(_db.Cartissima.Single(o => o.sCartId == key));
            _db.SaveChanges();
            var data = _db.Cartissima.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator")]
        [Route("inserzioni/excell")]
        public ActionResult Excell(string gridModel)
        {
            var exp = new ExcelExport();
            var context = new MyDbContext();
            var now = Guid.NewGuid();

            IEnumerable dataSource = context.Cartissima.OrderBy(o => o.sCartCreateDate).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.Pv.ToList();

            exp.Export(obj, dataSource, now + " - Cartissima.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }
        
        [Authorize(Roles = "Administrator")]
        [Route("inserzioni/word")]
        public ActionResult Word(string gridModel)
        {
            var context = new MyDbContext();
            var now = Guid.NewGuid();
            var exp = new WordExport();

            IEnumerable dataSource = context.Cartissima.OrderBy(o => o.sCartCreateDate).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.Pv.ToList();

            exp.Export(obj, dataSource, now + " - Cartissima.docx", false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }
        
        [Authorize(Roles = "Administrator")]
        [Route("inserzioni/pdf")]
        public ActionResult Pdf(string gridModel)
        {
            var context = new MyDbContext();
            var now = Guid.NewGuid();
            var exp = new PdfExport();

            IEnumerable dataSource = context.Cartissima.OrderBy(o => o.sCartCreateDate).ToList(); 

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.Pv.ToList();

            exp.Export(obj, dataSource, now + " - Cartissima.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }
        private GridProperties ConvertGridObject(string gridProperty)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            IEnumerable div = (IEnumerable)serializer.Deserialize(gridProperty, typeof(IEnumerable));
            GridProperties gridProp = new GridProperties();
            foreach (KeyValuePair<string, object> ds in div)
            {
                var property = gridProp.GetType().GetProperty(ds.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    Type type = property.PropertyType;
                    string serialize = serializer.Serialize(ds.Value);
                    object value = serializer.Deserialize(serialize, type);
                    property.SetValue(gridProp, value, null);
                }
            }
            return gridProp;
        }
    }
}