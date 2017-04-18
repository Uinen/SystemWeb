using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemWeb.Models;
using SystemWeb.Repository;
using SystemWeb.Repository.Interface;

namespace SystemWeb.Controllers
{
    public class AdaptiveController : Controller
    {
        private iCaricoRepository _CaricoRepository;
        private MyDbContext db = new MyDbContext();
        public string ly { get; set; }
        private int lastYear;

        public AdaptiveController()
        {
            this._CaricoRepository = new CaricoRepository(new MyDbContext());
        }

        public AdaptiveController(ApplicationUserManager userManager)
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

        // GET: Adaptive
        public ActionResult Index()
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            var DataSource2 = new MyDbContext().Pv
                .Where(a=> a.pvID == currentUser.pvID).ToList();

            ViewBag.dataSource2 = DataSource2;

            var DataSource3 = new MyDbContext().Year.Where(c => c.Anno.Year.ToString().Contains(ly)).ToList();
            ViewBag.dataSource3 = DataSource3;

            return View();
        }

        public ActionResult CaricoGetData(/*string sortOrder, string currentFilter, string searchString, int? page, */ DateTime? dateFrom, DateTime? dateTo, DataManager dm)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            /*IEnumerable getall = (from order in db.Carico
                          where (currentUser.pvID == order.pvID && order.Year.Anno.Year.ToString().Contains(ly))
                          select order).ToList();*/

            IEnumerable DataSource = new MyDbContext().Carico.Where(c => c.pvID == currentUser.pvID && c.Year.Anno.Year.ToString().Contains(ly)).OrderBy(c=> c.Ordine).ToList();
            DataResult result = new DataResult();
            DataOperations operation = new DataOperations();
            result.result = DataSource;

            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting 
            {
                result.result = operation.PerformSorting(result.result, dm.Sorted);
            }

            result.count = result.result.AsQueryable().Count();

            if (dm.Skip > 0)  // for paging  

                result.result = operation.PerformSkip(result.result, dm.Skip);

            if (dm.Take > 0)

                result.result = operation.PerformTake(result.result, dm.Take);

            return Json(new { result = result.result, count = result.count }, JsonRequestBehavior.AllowGet);
        }

        public class DataResult
        {
            public IEnumerable result { get; set; }
            public int count { get; set; }

        }

        //Perform file insertion 
        public ActionResult PerformInsert(EditParams param)
        {

            MyDbContext db = new MyDbContext();
            db.Carico.Add(param.value);
            db.SaveChanges();

            return RedirectToAction("GetOrderData");
        }

        //Perform update
        public ActionResult PerformUpdate(EditParams param)
        {

            MyDbContext db = new MyDbContext();
            Carico table = db.Carico.Single(o => o.Id == param.value.Id);

            db.Entry(table).CurrentValues.SetValues(param.value);
            db.Entry(table).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("GetOrderData");
        }

        //Perform delete
        public ActionResult PerformDelete(Guid key, string keyColumn)
        {

            MyDbContext db = new MyDbContext();
            db.Carico.Remove(db.Carico.Single(o => o.Id == key));
            db.SaveChanges();
            return RedirectToAction("GetOrderData");
        }
    }
}