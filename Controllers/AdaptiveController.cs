using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity;
using SystemWeb.Models.Entities;
using SystemWeb.Models;
using Syncfusion.JavaScript;
using System.Collections;
using Syncfusion.JavaScript.DataSources;
using SystemWeb.Models.Repository;
using System.Net;

namespace SystemWeb.Controllers
{
    public class AdaptiveController : Controller
    {
        //
        // GET: /Adaptive/

        public ActionResult Adaptive()
        {
            /*
            #region Initialize
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            MyDbContext db = new MyDbContext();
            #endregion

            #region Operation
            DateTime currentYear = DateTime.Now;

            var getdata = from a in db.Carico.ToList()
                          where (currentUser.pvID == a.pvID && currentYear.Year == a.Year.Anno.Year)
                          select a;

            int SSPBTotalAmount = getdata.Sum(s => s.Benzina);
            int DieselTotalAmount = getdata.Sum(s => s.Gasolio);

            ViewBag.TotalAmount = SSPBTotalAmount + DieselTotalAmount;

            #endregion
            */
            return View();
        }

        public ActionResult AdaptiveGrid(DataManager dm, Guid? Id)
        {
            #region Initialize

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            MyDbContext db = new MyDbContext();

            #endregion

            #region Query

            var getall = from a in db.Carico
                         select a;

            var getall2 = from a in db.Carico
                         select a;
            
            IEnumerable DataSource = CaricoRepository.GetAllRecords();
            IEnumerable DataSource2 = getall2;
            //int currentYear = DateTime.Today.Year;
            //DateTime currentYearConverted = Convert.ToDateTime(currentYear);
            DataSource = db.Carico/*.Where(c => currentUser.pvID == c.pvID)*/.Include("Pv").Include("Year").OrderBy(p => p.Ordine).ToList();
            
            BatchDataResult result = new BatchDataResult();
            DataOperations obj = new DataOperations();
            List<string> str = new List<string>();

            if (dm.Aggregates != null)
            {
                for (var i = 0; i < dm.Aggregates.Count; i++)
                    str.Add(dm.Aggregates[i].Field);
                result.aggregate = obj.PerformSelect(DataSource, str);
            }

            if (dm.Skip != 0)
            {
                DataSource = obj.PerformSkip(DataSource, dm.Skip);
            }
            if (dm.Take != 0)
            {
                DataSource = obj.PerformTake(DataSource, dm.Take);
            }

            result.value = "Sono presenti";
            result.count = CaricoRepository.GetAllRecords().Count();
            result.value2 = "Ordini";
            result.result = DataSource.Cast<SystemWeb.Models.Carico>().Select(x => new
            {
                ID = x.Id,
                ORDINE = x.Ordine,
                PVNAME = x.Pv.pvName,
                YEARSDATE = x.Year.Anno.Year,
                CDATA = x.cData,
                DOCUMENTO = x.Documento,
                NUMERO = x.Numero,
                RDATA = x.rData,
                EMITTENTE = x.Emittente,
                BENZINA = x.Benzina,
                GASOLIO = x.Gasolio,
                NOTE = x.Note
            });

            #endregion

            #region Operation

            DateTime currentYear = DateTime.Now;

            var getdata = from a in db.Carico.ToList()
                          where (/*currentUser.pvID == a.pvID &&*/ currentYear.Year == a.Year.Anno.Year)
                          select a;

            int SSPBTotalAmount = getdata.Sum(s => s.Benzina);
            int DieselTotalAmount = getdata.Sum(s => s.Gasolio);

            ViewBag.TotalAmount = SSPBTotalAmount + DieselTotalAmount;

            #endregion

            #region Update

            BatchDataResult year = new BatchDataResult();
            BatchDataResult pv = new BatchDataResult();

            pv.pv = DataSource2.Cast<SystemWeb.Models.Carico>().ToList()
                .Select(x => new
                {
                    PVNAME = x.Pv.pvName
                });

            year.year = DataSource2.Cast<SystemWeb.Models.Carico>().ToList()
                .Select(x => new
                {
                    YEARSDATE = x.Year.Anno.Year
                });

            SystemWeb.Models.Carico carico = db.Carico.Find(Id);

            ViewBag.year = year;
            ViewBag.pv = pv;

            #endregion

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public class BatchDataResult
        {
            public IEnumerable result { get; set; }
            public IEnumerable pv { get; set; }
            public IEnumerable year { get; set; }
            public int count { get; set; }
            public IEnumerable aggregate { get; set; }
            public IEnumerable groupDs { get; set; }
            public string value { get; set; }
            public string value2 { get; set; } 
        }

        public ActionResult Update(SystemWeb.Models.Carico value, Guid? id)
        {
            MyDbContext db = new MyDbContext();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SystemWeb.Models.Carico carico = db.Carico.Find(id);

            if (carico == null)
            {
                return HttpNotFound();
            }
    
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            CaricoRepository.Update(value);
        
            return Json(carico, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Insert(SystemWeb.Models.Carico value)
        {
            CaricoRepository.Add(value);
            var data = CaricoRepository.GetAllRecords();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Remove(Guid key)
        {
            CaricoRepository.Delete(key);
            var data = CaricoRepository.GetAllRecords();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}