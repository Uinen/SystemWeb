using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript.Models;
using Syncfusion.XlsIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SystemWeb.Models;
using SystemWeb.Repository;
using SystemWeb.Repository.Interface;

namespace SystemWeb.Controllers
{
    public class CartissimaController : Controller
    {
        #region Inizializzatori
        private readonly MyDbContext _db = new MyDbContext();
        private readonly iCartissimaRepository _cartissimaRepository;

        public CartissimaController()
        {
            _cartissimaRepository = new CartissimaRepository(new MyDbContext());
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
        public ActionResult List()
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            #endregion

            var dataSource = new MyDbContext().Cartissima.OrderBy(o => o.sCartCreateDate).ToList();
            ViewBag.datasource = dataSource;

            IEnumerable dataSource2 = new MyDbContext().Pv.ToList();
            ViewBag.datasource2 = dataSource2;

            return View();
        }

        public ActionResult Send()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send(Cartissima value)
        {
            if (ModelState.IsValid)
            {
                _cartissimaRepository.Insert(value);
                _cartissimaRepository.Save();
                return RedirectToAction("Success", new { key = value.sCartId });
            }

            return View();
        }
        
        public ActionResult Success(Guid key)
        {
            var _getCode = (from a in _cartissimaRepository.GetRecords()
                            where key == a.sCartId
                           select a.sCartId).SingleOrDefault();

            ViewBag.Cod = _getCode;

            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Update(Cartissima value)
        {
            CartissimaRepositorySync.Update(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        
        [Authorize(Roles = "Administrator")]
        public ActionResult Insert(Cartissima value)
        {
            CartissimaRepositorySync.Add(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        
        [Authorize(Roles = "Administrator")]
        public ActionResult Remove(Guid key)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            #endregion

            MyDbContext context = new MyDbContext();
            context.Cartissima.Remove(context.Cartissima.Single(o => o.sCartId == key));
            context.SaveChanges();

            var data = context.Cartissima.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator")]
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