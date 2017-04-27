using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SystemWeb.Models;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using SystemWeb.Repository.Interface;
using SystemWeb.Repository;
using System.Data;
using System.Collections;
using Syncfusion.Linq;
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript.Models;
using Syncfusion.XlsIO;
using System.Web.Script.Serialization;
using System.Reflection;

namespace SystemWeb.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        #region Inizializzatori
        private MyDbContext db = new MyDbContext();
        private iCaricoRepository _CaricoRepository;
        private iPvRepository _PvRepository;
        private iPvErogatoriRepository _PvErogatoriRepository;
        private iPvCaliRepository _PvCaliRepository;
        private iPvDeficienzeRepository _PvDeficienzeRepository;
        public string ly { get; set; }

        public UserController()
        {
            this._CaricoRepository = new CaricoRepository(new MyDbContext());
            this._PvRepository = new PvRepository(new MyDbContext());
            this._PvErogatoriRepository = new PvErogatoriRepository(new MyDbContext());
            this._PvCaliRepository = new PvCaliRepository(new MyDbContext());
            this._PvDeficienzeRepository = new PvDeficienzeRepository(new MyDbContext());
        }

        public UserController(ApplicationUserManager userManager)
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

        #region Index 
        [Route("user")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, DateTime? dateFrom, DateTime? dateTo, DateTime? dateFrom2, DateTime? dateTo2, Guid? id)
        {
            #region List
            UserIndexViewModel list = new UserIndexViewModel();

            list.carico = db.Carico
                .Include(i => i.Pv)
                .Include(i => i.Year)
                .ToList();

            list.pverogatori = db.PvErogatori
                .Include(i => i.Pv)
                .Include(i => i.Dispenser)
                .Include(i => i.Product)
                .ToList();

            list.pv = db.Pv
                .Include(i => i.Flag)
                .Include(i => i.Carico)
                .Include(i => i.PvTank)
                .Include(i => i.ApplicationUser)
                .ToList();

            list.pvprofile = db.PvProfile
                .Include(i => i.Pv)
                .ToList();

            list.pvtank = db.PvTank
                .Include(i => i.Product)
                .Include(i => i.Pv)
                .Include(i => i.PvCali)
                .Include(i => i.PvDeficienze)
                .ToList();

            list.dispenser = db.Dispenser
                .Include(i => i.PvErogatori)
                .Include(i => i.PvTank)
                .ToList();

            list.company = db.Company
                .Include(i => i.ApplicationUser)
                .Include(i => i.RagioneSociale)
                .ToList();

            list.companytask = db.CompanyTask
                .Include(i => i.ApplicationUser)
                .ToList();

            list.userarea = db.UserArea
                .Include(i => i.ApplicationUser)
                .ToList();

            list.applicationuser = db.Users
                .Include(i => i.Pv)
                .Include(i => i.Company)
                .Include(i => i.FilePaths)
                .Include(i => i.Pv)
                .Include(i => i.UserArea)
                .Include(i => i.UserProfiles)
                .Include(i => i.CompanyTask)
                .Include(i => i.Roles)
                .Include(i => i.Logins)
                .Include(i => i.Claims)
                .ToList();

            list.usersimage = db.UsersImage
                .Include(i => i.UserProfiles)
                .ToList();

            #endregion

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            var getPId = from a in db.UserProfiles
                         where a.ProfileId == currentUser.ProfileId
                         select a;
            
            ViewBag.profileId = getPId;

            var somequalsD1 = (from PvProfile in db.PvProfile where currentUser.pvID == PvProfile.pvID select PvProfile.Indirizzo).SingleOrDefault();
            var somequalsD2 = (from PvProfile in db.PvProfile where currentUser.pvID == PvProfile.pvID select PvProfile.Città).SingleOrDefault();
            var somequalsD3 = (from ApplicationUser in db.Users where currentUser.pvID == ApplicationUser.Pv.pvID select ApplicationUser.Pv.pvName).SingleOrDefault();
            var somequalsD4 = (from ApplicationUser in db.Users where currentUser.pvID == ApplicationUser.Pv.pvID select ApplicationUser.Pv.Flag.Nome).SingleOrDefault();
            var somequalsD5 = db.Users.Include(s => s.UserProfiles).Where(s => s.Id == currentUser.Id).Select(s => s.UserProfiles.ProfileCity + ", " + s.UserProfiles.ProfileAdress).SingleOrDefault(); 
            var somequalsD6 = db.Users.Include(s => s.UserProfiles).Where(s => s.Id == currentUser.Id).Select(s => s.UserProfiles.ProfileName + " " + s.UserProfiles.ProfileSurname).SingleOrDefault();
            var somequalsD7 = db.Users.Include(s => s.Company).Where(s => s.Id == currentUser.Id).Select(s => s.Company.Name).SingleOrDefault();
            //ViewBag.ProfileName = currentUser.UserProfiles.ProfileName;
            //ViewBag.ProfileSurname = currentUser.UserProfiles.ProfileSurname;
            ViewBag.FullAdress = somequalsD5;
            ViewBag.ProfileFullName = somequalsD6;
            //ViewBag.ProfileAdress = currentUser.UserProfiles.ProfileAdress;
            //ViewBag.ProfileCity = currentUser.UserProfiles.ProfileCity;
            //ViewBag.ProfileZipCode = currentUser.UserProfiles.ProfileZipCode;
            //ViewBag.ProfileNation = currentUser.UserProfiles.ProfileNation;
            //ViewBag.ProfileInfo = currentUser.UserProfiles.ProfileInfo;
            ViewBag.Flag = somequalsD4;
            ViewBag.PvNamee = somequalsD3;
            ViewBag.PvInd = somequalsD1;
            ViewBag.PvCity = somequalsD2;
            ViewBag.CompanyId = somequalsD7;

            UserProfiles profile = db.UserProfiles.Include(s => s.UsersImage).Include(s => s.ApplicationUser).SingleOrDefault(s => s.ProfileId == id);
            list.userprofiles = profile;

            #region CaricoCreate

            int thisYear;
            thisYear = DateTime.Now.Year;

            IQueryable<Pv> pv = db.Pv
                .Where(c => currentUser.pvID == c.pvID);
            var sql = pv.ToList();

            IQueryable<Year> year = db.Year
                .Where(c => c.Anno.Year == thisYear);
            var sql2 = year.ToList();

            ViewBag.pvID = new SelectList(pv, "pvID", "pvName");
            ViewBag.yearId = new SelectList(year, "yearId", "Anno");

            #endregion

            #region TotaleContatori

            dateFrom = new DateTime(2016, 12, 31);
            dateTo = DateTime.Now;
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();

            var getAll = from a in _PvErogatoriRepository.GetPvErogatori()
                        
                          where (Convert.ToDateTime(a.FieldDate) >= dateFrom)
                                   && (Convert.ToDateTime(a.FieldDate) <= dateTo)
                         //where (a.FieldDate.Year.ToString().Contains(ly))
                         select a;

            int maxB = getAll
                .Where(z => currentUser.pvID == z.pvID
                && (z.Product.Nome.Contains("B")))
                .Max(row => row.Value);
            int minB = getAll
                .Where(z => currentUser.pvID == z.pvID
                && (z.Product.Nome.Contains("B")))
                .Min(row => row.Value);

            int maxG = getAll
                .Where(z => currentUser.pvID == z.pvID
                && (z.Product.Nome.Contains("G")))
                .Max(row => row.Value);
            int minG = getAll
                .Where(z => currentUser.pvID == z.pvID
                && (z.Product.Nome.Contains("G")))
                .Min(row => row.Value);

            ViewBag.SSPBTotalAmount = maxB - minB;
            ViewBag.DieselTotalAmount = maxG - minG;
            ViewBag.TotalAmount = maxB - minB + maxG - minG;
            #endregion

            #region TotaleContatoriPrecedente

            dateFrom2 = new DateTime(2015, 12, 31);
            dateTo2 = DateTime.Now.AddYears(-1);

            var getAll2 = from a in _PvErogatoriRepository.GetPvErogatori()
                          where (Convert.ToDateTime(a.FieldDate) >= dateFrom2)
                                  && (Convert.ToDateTime(a.FieldDate) <= dateTo2)
                         //where (a.FieldDate.Year.ToString().Contains(ly))
                         select a;

            int maxB2 = getAll2
                .Where(z => currentUser.pvID == z.pvID
                && (z.Product.Nome.Contains("B")))
                .Max(row => row.Value);
            int minB2 = getAll2
                .Where(z => currentUser.pvID == z.pvID
                && (z.Product.Nome.Contains("B")))
                .Min(row => row.Value);

            int maxG2 = getAll2
                .Where(z => currentUser.pvID == z.pvID
                && (z.Product.Nome.Contains("G")))
                .Max(row => row.Value);
            int minG2 = getAll2
                .Where(z => currentUser.pvID == z.pvID
                && (z.Product.Nome.Contains("G")))
                .Min(row => row.Value);

            ViewBag.SSPBTotalAmount2 = maxB2 - minB2;
            ViewBag.DieselTotalAmount2 = maxG2 - minG2;
            ViewBag.TotalAmount2 = maxB2 - minB2 + maxG2 - minG2;
            #endregion

            #region Difference

            ViewBag.TotalAmountDifference = ((maxB - minB) + (maxG - minG)) - ((maxB2 - minB2) + (maxG2 - minG2));

            #endregion

            return View(list);
        }

        #endregion

        #region FilePaths

        // GET: FilePaths
        public ActionResult File()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            var myBoard = from a in db.FilePaths.Include(f => f.ApplicationUser)
                          where currentUser.Id == a.UserID
                          select a;

            return View(myBoard.ToList());
        }

        // GET: FilePaths/Details/5
        public ActionResult FileDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilePath filePath = db.FilePaths.Find(id);
            if (filePath == null)
            {
                return HttpNotFound();
            }
            return View(filePath);
        }

        public ActionResult FileCreate([Bind(Include = "FilePathID,FileName,FileType,UploadDate,UserID")] FilePath filePath, ApplicationUser user, IEnumerable<HttpPostedFileBase> upload)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                foreach (var file in upload)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string filename = Path.GetFileName(Guid.NewGuid() + "." + file.FileName);
                        var filepath = Path.Combine(Server.MapPath("~/Uploads/Documents/"), filename);
                        file.SaveAs(filepath);

                        var path = "/Uploads/Documents/" + filename;
                        var FileDocumentPath = string.Format("{0}", path);

                        var document = new FilePath
                        {
                            FileName = FileDocumentPath,
                            FileType = FileType.Document,
                            UploadDate = DateTime.Now.Date,
                            UserID = currentUser.Id
                        };
                        db.FilePaths.Add(document);
                        db.SaveChanges();
                    }
                }
            }

            ViewBag.UserID = new SelectList(db.Users, "Id", "UserName", filePath.UserID);

            return Content("");
        }

        // GET: FilePaths/Edit/5
        public ActionResult FileEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilePath filePath = db.FilePaths.Find(id);
            if (filePath == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "Id", "UserName", filePath.UserID);
            return View(filePath);
        }

        // POST: FilePaths/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FileEdit([Bind(Include = "FilePathID,FileName,FileType,UploadDate,UserID")] FilePath filePath)
        {
            if (ModelState.IsValid)
            {
                db.Entry(filePath).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.Users, "Id", "UserName", filePath.UserID);
            return View(filePath);
        }

        // GET: FilePaths/Delete/5
        public ActionResult FileDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilePath filePath = db.FilePaths.Find(id);
            if (filePath == null)
            {
                return HttpNotFound();
            }
            return View(filePath);
        }

        // POST: FilePaths/Delete/5
        [HttpPost, ActionName("FileDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult FileDeleteConfirmed(Guid id)
        {
            FilePath filePath = db.FilePaths.Find(id);
            db.FilePaths.Remove(filePath);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        #endregion

        #region Map Designer
        public ActionResult PvMap()
        {
            return View(new MapDesignerViewModel { X = 20, Y = 20 });
        }
        public ActionResult MapDesigner()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            return Json(db.Dispenser.ToList()
                .Where(c => currentUser.pvID == c.PvTank.pvID)
                .Select(c => new
                {
                    mod = c.Modello
                }),
            JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Carico
        [Route("user/ordini")]
        public ActionResult Carico(DateTime? dateFrom, DateTime? dateTo)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            var DataSource = new MyDbContext().Carico.Where(c => currentUser.pvID == c.pvID && (c.Year.Anno.Year.ToString().Contains(ly))).OrderBy(o => o.Ordine).ToList();
            ViewBag.datasource = DataSource;

            IEnumerable DataSource2 = new MyDbContext().Year.Where(a => (a.Anno.Year.ToString().Contains(ly))).ToList();
            ViewBag.datasource2 = DataSource2;

            IEnumerable DataSource3 = new MyDbContext().Pv.Where(c => currentUser.pvID == c.pvID).ToList();
            ViewBag.datasource3 = DataSource3;
            
            #region AmmountByDateFrom
            // Totale Carico Benzina secondo il parametro di ricerca specificato
            ViewBag.SSPBTotalAmountFrom = DataSource.ToList()
                .Where(o => /*currentUser.pvID == o.pvID &&*/ Convert.ToDateTime(o.cData) >= dateFrom && Convert.ToDateTime(o.cData) <= dateTo)
                .Sum(o => (decimal?)o.Benzina);

            // Totale Carico Gasolio secondo il parametro di ricerca specificato
            ViewBag.DieselTotalAmountFrom = DataSource.ToList()
                .Where(o => /*currentUser.pvID == o.pvID &&*/ Convert.ToDateTime(o.cData) >= dateFrom && Convert.ToDateTime(o.cData) <= dateTo)
                .Sum(o => (decimal?)o.Gasolio);
            #endregion

            #region Total Ammount ViewBag
            // Totale Carico annuo benzina.
            ViewBag.SSPBTotalAmount = DataSource.Sum(o => (decimal?)o.Benzina);

            // Totale Carico annuo gasolio. 
            ViewBag.DieselTotalAmount = DataSource.Sum(o => (decimal?)o.Gasolio);
            #endregion

            return View();
        }
        /*
        [Route("user/carico/caricogetdata")]
        public ActionResult CaricoGetData(DataManager dm)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            IEnumerable DataSource = OrderRepository.GetAllRecords().Where(c => c.pvID == currentUser.pvID && c.yearId.ToString().Contains(ly)).OrderBy(c => c.Ordine).ToList();
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

        public List<object> PVID
        {
            get
            {
                #region Initial var
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var currentUser = userManager.FindById(User.Identity.GetUserId());
                #endregion
                
                var pvID = OrderRepository.GetAllRecords().Where(a => a.pvID == currentUser.pvID).Select(s => s.pvName).Distinct().ToList();
                var PVID = new List<object>();
                foreach (var id in pvID)
                {
                    PVID.Add(new { value = id, text = id });
                }
                return PVID;
            }
        }

        public List<object> YearID
        {
            get
            {
                #region Initial var
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var currentUser = userManager.FindById(User.Identity.GetUserId());
                lastYear = DateTime.Today.Year;
                ly = lastYear.ToString();
                #endregion
                
                var yearId = OrderRepository.GetAllRecords().Where(c => c.yearId.ToString().Contains(ly)).Select(s => s.yearId).Distinct().ToList();
                var YearID = new List<object>();
                foreach (var id in yearId)
                {
                    YearID.Add(new { value = id, text = id });
                }
                return YearID;
            }
        }

        [Route("user/carico/batchupdate")]
        public ActionResult BatchUpdate(List<CaricoDto> changed, List<CaricoDto> added, List<CaricoDto> deleted)
        {
            if (changed != null)
                OrderRepository.Update(changed);
            if (deleted != null)
                OrderRepository.Delete(deleted);
            if (added != null)
                OrderRepository.Add(added);
            var data = OrderRepository.GetAllRecords();
            return Json(data, JsonRequestBehavior.AllowGet);
        }*/
        [Route("user/ordini/update")]
        public ActionResult UpdateCarico(Carico value)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            OrderRepository.Update(value);
            var data = context.Carico.Include(i => i.Pv).Include(i => i.Year).Where(c => currentUser.pvID == c.pvID && c.Year.Anno.Year.ToString().Contains(ly));
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        [Route("user/ordini/insert")]
        public ActionResult InsertCarico(Carico value)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            OrderRepository.Add(value);
            var data = context.Carico.Include(i => i.Pv).Include(i => i.Year).Where(c => currentUser.pvID == c.pvID && c.Year.Anno.Year.ToString().Contains(ly));
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/ordini/remove")]
        public ActionResult RemoveCarico(Guid key)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion
            
            MyDbContext context = new MyDbContext();
            context.Carico.Remove(context.Carico.Single(o => o.Id == key));
            context.SaveChanges();

            var data = context.Carico.Where(c => currentUser.pvID == c.pvID && c.Year.Anno.Year.ToString().Contains(ly));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("user/ordini/caricoexcell")]
        public ActionResult CaricoExcell(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            ExcelExport exp = new ExcelExport();
            MyDbContext context = new MyDbContext();
            Guid now = Guid.NewGuid();
            IEnumerable DataSource = context.Carico.Where(c => currentUser.pvID == c.pvID && (c.Year.Anno.Year.ToString().Contains(ly))).OrderBy(o => o.Ordine).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[2].DataSource = context.Year.Where(a => a.Anno.Year.ToString().Contains(ly)).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Ordini.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/ordini/caricoword")]
        public ActionResult CaricoWord(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            Guid now = Guid.NewGuid();
            WordExport exp = new WordExport();

            IEnumerable DataSource = context.Carico.Where(c => currentUser.pvID == c.pvID && (c.Year.Anno.Year.ToString().Contains(ly))).OrderBy(o => o.Ordine).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[2].DataSource = context.Year.Where(a => a.Anno.Year.ToString().Contains(ly)).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Ordini.docx", false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/ordini/caricopdf")]
        public ActionResult CaricoPdf(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();

            Guid now = Guid.NewGuid();

            PdfExport exp = new PdfExport();

            IEnumerable DataSource = context.Carico.Where(c => currentUser.pvID == c.pvID && (c.Year.Anno.Year.ToString().Contains(ly))).OrderBy(o => o.Ordine).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[2].DataSource = context.Year.Where(a => a.Anno.Year.ToString().Contains(ly)).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Ordini.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult CaricoChart()
        {
            Carico objCaricoModel = new Carico();
            objCaricoModel.sspb = "Benzina";
            objCaricoModel.dsl = "Gasolio";

            return View(objCaricoModel);
        }

        public ActionResult CaricoGetChart()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();

            var getall = from order in _CaricoRepository.GetOrders()
                         where (currentUser.pvID == order.pvID && order.Year.Anno.Year.ToString().Contains(ly))
                         select order;

            return Json(getall.ToList()
                .OrderBy(c => c.Ordine)
                .Select(c => new
                {
                    sspb = c.Benzina,
                    dsl = c.Gasolio,
                    ord = (c.Ordine).ToString(),
                    dty = (c.cData.Year),
                    dtm = (c.cData.Month - 1),
                    dtd = (c.cData.Day)
                }),
            JsonRequestBehavior.AllowGet);
        }
        /*
        public ActionResult CaricoDetails(Guid? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carico carico = _CaricoRepository.GetOrdersById(Id);
            if (carico == null)
            {
                return HttpNotFound();
            }
            return View(carico);
        }
        */
        public ActionResult CaricoCreate()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            int thisYear;
            thisYear = DateTime.Now.Year;

            var getall = from order in _PvRepository.GetPvs()
                         where (currentUser.pvID == order.pvID)
                         select order;

            IQueryable<Year> year = db.Year
                .Where(c => c.Anno.Year == thisYear);

            var sql2 = year.ToList();
            ViewBag.pvID = new SelectList(getall, "pvID", "pvName");
            ViewBag.yearId = new SelectList(year, "yearId", "Anno");
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CaricoCreate(Carico insertOrder)
        {
            if (ModelState.IsValid)
            {
                _CaricoRepository.InsertOrder(insertOrder);
                _CaricoRepository.Save();
                return RedirectToAction("Carico");
            }

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            int thisYear;
            thisYear = DateTime.Now.Year;

            var getall = from order in _PvRepository.GetPvs()
                         where (currentUser.pvID == order.pvID)
                         select order;

            IQueryable<Year> year = db.Year
                .Where(c => c.Anno.Year == thisYear);
            var sql2 = year.ToList();
            ViewBag.pvID = new SelectList(getall, "pvID", "pvName");
            ViewBag.yearId = new SelectList(year, "yearId", "Anno");
            return View(insertOrder);
        }

        #region Old result
        /*
        public ActionResult CaricoEdit(Guid? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carico carico = _CaricoRepository.GetOrdersById(Id);

            if (carico == null)
            {
                return HttpNotFound();
            }

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            var getall = from order in _PvRepository.GetPvs()
                         where (currentUser.pvID == order.pvID)
                         select order;

            IQueryable<Year> year = db.Year;
            var sql2 = year.ToList();
            ViewBag.pvID = new SelectList(getall, "pvID", "pvName", carico.pvID);
            ViewBag.yearId = new SelectList(year, "yearId", "Anno");
            return View(carico);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CaricoEdit(Carico updateOrder)
        {
            if (ModelState.IsValid)
            {
                _CaricoRepository.UpdateOrder(updateOrder);
                _CaricoRepository.Save();
                return RedirectToAction("Carico");
            }
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName", updateOrder.pvID);
            ViewBag.yearId = new SelectList(db.Year, "yearId", "Anno", updateOrder.yearId);
            return View(updateOrder);
        }
        
        public ActionResult CaricoDelete(Guid? Id, bool? saveChangesError)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Unable to save changes. Try again, and if the problem persists contact your system administrator.";
            }
            Carico carico = _CaricoRepository.GetOrdersById(Id);

            return View(carico);
        }
        
        [HttpPost, ActionName("CaricoDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult CaricoDeleteConfirmed(Guid Id)
        {
            try
            {
                Carico carico = _CaricoRepository.GetOrdersById(Id);
                _CaricoRepository.DeleteOrder(Id);
                _CaricoRepository.Save();
            }
            catch (DataException)
            {
                return RedirectToAction("Delete",
                   new System.Web.Routing.RouteValueDictionary {
        { "id", Id },
        { "saveChangesError", true } });
            }
            return RedirectToAction("Carico");
        }
        */
        #endregion

        #endregion

        #region PvErogatori
        [Route("user/contatori")]
        public ActionResult PvErogatori(DateTime? dateFrom, DateTime? dateTo)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            var DataSource = new MyDbContext().PvErogatori.Where(c => currentUser.pvID == c.pvID && (c.FieldDate.Year.ToString().Contains(ly)) && c.Dispenser.isActive == true).OrderBy(o => o.FieldDate).ToList();
            ViewBag.datasource = DataSource;

            IEnumerable DataSource2 = new MyDbContext().Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            ViewBag.datasource2 = DataSource2;

            IEnumerable DataSource3 = new MyDbContext().Product.ToList();
            ViewBag.datasource3 = DataSource3;

            IEnumerable DataSource4 = new MyDbContext().Dispenser/*.Include(c => c.PvTank)*/.Where(a => currentUser.pvID == a.PvTank.pvID && a.isActive == true).ToList();
            ViewBag.datasource4 = DataSource4;

            #region IF dateFrom == null | dateTo == null
            if (dateFrom == null | dateTo == null)
            {
                var myDate = DateTime.Now;
                var newDate = myDate.AddYears(-1);

                DateTime da;
                DateTime al;

                //da = new DateTime((newDate.Year), 12, 31);
                da = new DateTime(2016,12,31);
                al = DateTime.Now;

                ViewBag.ShowLastYear = da.ToString();
                lastYear = DateTime.Today.Year;
                ly = lastYear.ToString();

                var getall = from a in _PvErogatoriRepository.GetPvErogatori()
                             where (currentUser.pvID == a.pvID && (Convert.ToDateTime(a.FieldDate) >= da)
                                   && (Convert.ToDateTime(a.FieldDate) <= al))
                             select a;

                int maxB = getall
                    .Where(z =>(z.Product.Nome.Contains("B")))
                    .Max(row => row.Value);
                int minB = getall
                    .Where(z => (z.Product.Nome.Contains("B")))
                    .Min(row => row.Value);

                int maxG = getall
                    .Where(z => (z.Product.Nome.Contains("G")))
                    .Max(row => row.Value);
                int minG = getall
                    .Where(z => (z.Product.Nome.Contains("G")))
                    .Min(row => row.Value);

                ViewBag.SSPBTotalAmount = maxB - minB;
                ViewBag.DieselTotalAmount = maxG - minG;
            }

            else
            {
                ViewBag.SSPBTotalAmount = "E' stato impostato un parametro";
                ViewBag.DieselTotalAmount = "E' stato impostato un parametro";
            }
            #endregion

            #region IF dateFrom != null | dateTo |= null
            if (dateFrom != null | dateTo != null)
            {
                var getAllfromParam = from a in _PvErogatoriRepository.GetPvErogatori()
                             where (currentUser.pvID == a.pvID
                                   && (Convert.ToDateTime(a.FieldDate) >= dateFrom)
                                   && (Convert.ToDateTime(a.FieldDate) <= dateTo))
                             select a;

                int maxB = getAllfromParam
                    .Where(z => (z.Product.Nome.Contains("B")))
                    .Max(row => row.Value);
                int minB = getAllfromParam
                    .Where(z => (z.Product.Nome.Contains("B")))
                    .Min(row => row.Value);

                int maxG = getAllfromParam
                    .Where(z => (z.Product.Nome.Contains("G")))
                    .Max(row => row.Value);
                int minG = getAllfromParam
                    .Where(z => (z.Product.Nome.Contains("G")))
                    .Min(row => row.Value);

                ViewBag.SSPBTotalAmountFrom = maxB - minB;
                ViewBag.DieselTotalAmountFrom = maxG - minG;
            }
            #endregion

            return View();
        }

        [Route("user/contatori/update")]
        public ActionResult UpdateErogatori(PvErogatori value)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            ContatoriRepository.Update(value);
            var data = context.PvErogatori/*.Include(i => i.Pv).Include(i => i.Product).Include(i => i.Dispenser)*/.Where(c => currentUser.pvID == c.pvID && c.FieldDate.Year.ToString().Contains(ly));
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        [Route("user/contatori/insert")]
        public ActionResult InsertErogatori(PvErogatori value)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            ContatoriRepository.Add(value);
            var data = context.PvErogatori/*.Include(i => i.Pv).Include(i => i.Product).Include(i => i.Dispenser)*/.Where(c => currentUser.pvID == c.pvID && c.FieldDate.Year.ToString().Contains(ly));
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/contatori/remove")]
        public ActionResult RemoveErogatori(Guid key)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            context.PvErogatori.Remove(context.PvErogatori.Single(o => o.PvErogatoriId == key));
            context.SaveChanges();

            var data = context.PvErogatori.Where(c => currentUser.pvID == c.pvID && c.FieldDate.Year.ToString().Contains(ly));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("user/contatori/erogatoriexcell")]
        public ActionResult PvErogatoriExcell(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            ExcelExport exp = new ExcelExport();
            MyDbContext context = new MyDbContext();
            Guid now = Guid.NewGuid();
            IEnumerable DataSource = new MyDbContext().PvErogatori.Where(c => currentUser.pvID == c.pvID && (c.FieldDate.Year.ToString().Contains(ly))).OrderBy(o => o.FieldDate).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[2].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[3].DataSource = context.Product.ToList();
            obj.Columns[4].DataSource = context.Dispenser/*.Include(c => c.PvTank)*/.Where(a => currentUser.pvID == a.PvTank.pvID).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Contatori.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/contatori/erogatoriword")]
        public ActionResult PvErogatoriWord(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            Guid now = Guid.NewGuid();
            WordExport exp = new WordExport();

            IEnumerable DataSource = new MyDbContext().PvErogatori.Where(c => currentUser.pvID == c.pvID && (c.FieldDate.Year.ToString().Contains(ly))).OrderBy(o => o.FieldDate).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[2].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[3].DataSource = context.Product.ToList();
            obj.Columns[4].DataSource = context.Dispenser/*.Include(c => c.PvTank)*/.Where(a => currentUser.pvID == a.PvTank.pvID).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Contatori.docx", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/contatori/erogatoripdf")]
        public ActionResult PvErogatoriPdf(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();

            Guid now = Guid.NewGuid();

            PdfExport exp = new PdfExport();

            IEnumerable DataSource = new MyDbContext().PvErogatori.Where(c => currentUser.pvID == c.pvID && (c.FieldDate.Year.ToString().Contains(ly))).OrderBy(o => o.FieldDate).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[2].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[3].DataSource = context.Product.ToList();
            obj.Columns[4].DataSource = context.Dispenser/*.Include(c => c.PvTank)*/.Where(a => currentUser.pvID == a.PvTank.pvID).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Contatori.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult PvErogatoriChart()
        {
            PvErogatori objPvErogatoriModel = new PvErogatori();
            objPvErogatoriModel.sspb = "Benzina";
            objPvErogatoriModel.dsl = "Gasolio";

            return View(objPvErogatoriModel);
        }

        #region PvErogatoriGetChart()
        /*
        public ActionResult PvErogatoriGetChart()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            int i = 1;
            DateTime dateFrom = new DateTime(2016, 01, 01);
            dateFrom.AddDays(i++);
            DateTime dateTo = DateTime.Now.AddDays(i--);

            var getAll = from a in _PvErogatoriRepository.GetPvErogatori()
                         where (currentUser.pvID == a.pvID
                         && (Convert.ToDateTime(a.FieldDate) >= dateFrom)
                         && (Convert.ToDateTime(a.FieldDate) <= dateTo))
                         select a;

            int maxB = getAll
                .Where(z => (z.Product.Nome.Contains("B")))
                .Max(row => row.Value);
            int minB = getAll
                .Where(z => (z.Product.Nome.Contains("B")))
                .Min(row => row.Value);

            int maxG = getAll
                .Where(z => (z.Product.Nome.Contains("G")))
                .Max(row => row.Value);
            int minG = getAll
                .Where(z => (z.Product.Nome.Contains("G")))
                .Min(row => row.Value);

            return Json(_PvErogatoriRepository.GetPvErogatori()
                .Where(c => currentUser.pvID == c.pvID)
                .OrderBy(c => c.FieldDate)
                .Select(c => new
                {
                    sspb = (maxB - minB),
                    dsl = (maxG - minG),
                    disp = (c.Dispenser.Modello),
                    dty = (c.FieldDate.Year),
                    dtm = (c.FieldDate.Month - 1),
                    dtd = (c.FieldDate.Day)
                }),
            JsonRequestBehavior.AllowGet);
        }
        */
        #endregion

        public ActionResult PvErogatoriDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PvErogatori pvErogatori = _PvErogatoriRepository.GetPvErogatoriById(id);
            if (pvErogatori == null)
            {
                return HttpNotFound();
            }
            return View(pvErogatori);
        }

        public ActionResult PvErogatoriCreate()
        {
            ViewBag.DispenserId = new SelectList(db.Dispenser, "DispenserId", "Modello");
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Nome");
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            var getall = from a in _PvRepository.GetPvs()
                         where (currentUser.pvID == a.pvID)
                         select a;

            ViewBag.pvID = new SelectList(getall, "pvID", "pvName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvErogatoriCreate(PvErogatori pvErogatori)
        {
            if (ModelState.IsValid)
            {
                pvErogatori.PvErogatoriId = Guid.NewGuid();
                _PvErogatoriRepository.InsertPvErogatori(pvErogatori);
                _PvErogatoriRepository.Save();
                return RedirectToAction("PvErogatori");
            }

            ViewBag.DispenserId = new SelectList(db.Dispenser, "DispenserId", "Modello", pvErogatori.DispenserId);
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Nome", pvErogatori.ProductId);
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName", pvErogatori.pvID);
            return View(pvErogatori);
        }

        public ActionResult PvErogatoriEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PvErogatori pvErogatori = _PvErogatoriRepository.GetPvErogatoriById(id);
            if (pvErogatori == null)
            {
                return HttpNotFound();
            }
            ViewBag.DispenserId = new SelectList(db.Dispenser, "DispenserId", "Modello", pvErogatori.DispenserId);
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Nome", pvErogatori.ProductId);
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            var getall = from a in _PvRepository.GetPvs()
                         where (currentUser.pvID == a.pvID)
                         select a;

            ViewBag.pvID = new SelectList(getall, "pvID", "pvName", pvErogatori.pvID);
            return View(pvErogatori);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvErogatoriEdit(PvErogatori pvErogatori)
        {
            if (ModelState.IsValid)
            {
                _PvErogatoriRepository.UpdatePvErogatori(pvErogatori);
                db.SaveChanges();
                return RedirectToAction("PvErogatori");
            }
            ViewBag.DispenserId = new SelectList(db.Dispenser, "DispenserId", "Modello", pvErogatori.DispenserId);
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Nome", pvErogatori.ProductId);
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName", pvErogatori.pvID);
            return View(pvErogatori);
        }

        public ActionResult PvErogatoriDelete(Guid? id, bool? saveChangesError)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Unable to save changes. Try again, and if the problem persists contact your system administrator.";
            }
            PvErogatori pvErogatori = _PvErogatoriRepository.GetPvErogatoriById(id);

            return View(pvErogatori);
        }

        [HttpPost, ActionName("PvErogatoriDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PvErogatoriDeleteConfirmed(Guid id)
        {
            try
            {
                PvErogatori pvErogatori = _PvErogatoriRepository.GetPvErogatoriById(id);
                _PvErogatoriRepository.DeletePvErogatori(id);
                _CaricoRepository.Save();
            }
            catch (DataException)
            {
                return RedirectToAction("Delete",
                   new System.Web.Routing.RouteValueDictionary {
        { "id", id },
        { "saveChangesError", true } });
            }
            return RedirectToAction("PvErogatori");
        }
        #endregion

        #region Pv

        public ActionResult Pv()
        {
            var pv = _PvRepository.GetPvs();
            return View(pv);
        }
  
        public ActionResult PvDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pv pv = _PvRepository.GetPvsById(id);
            if (pv == null)
            {
                return HttpNotFound();
            }
            return View(pv);
        }
  
        public ActionResult PvCreate()
        {
            ViewBag.pvFlagId = new SelectList(db.Flag, "pvFlagId", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvCreate(Pv pv)
        {
            if (ModelState.IsValid)
            {
                pv.pvID = Guid.NewGuid();
                _PvRepository.InsertPv(pv);
                _PvRepository.Save();
                return RedirectToAction("Pv");
            }

            ViewBag.pvFlagId = new SelectList(db.Flag, "pvFlagId", "Nome", pv.pvFlagId);
            return View(pv);
        }

        public ActionResult PvEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pv pv = _PvRepository.GetPvsById(id);
            if (pv == null)
            {
                return HttpNotFound();
            }
            ViewBag.pvFlagId = new SelectList(db.Flag, "pvFlagId", "Nome", pv.pvFlagId);
            return View(pv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvEdit(Pv pv)
        {
            if (ModelState.IsValid)
            {
                _PvRepository.UpdatePv(pv);
                _PvRepository.Save();
                return RedirectToAction("Pv");
            }
            ViewBag.pvFlagId = new SelectList(db.Flag, "pvFlagId", "Nome", pv.pvFlagId);
            return View(pv);
        }
      
        public ActionResult PvDelete(Guid? id, bool? saveChangesError)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Unable to save changes. Try again, and if the problem persists contact your system administrator.";
            }
            Pv pv = _PvRepository.GetPvsById(id);
            return View(pv);
        }

        [HttpPost, ActionName("PvDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PvDeleteConfirmed(Guid id)
        {
            try
            {
                Pv pv = _PvRepository.GetPvsById(id);
                _PvRepository.DeletePv(id);
                _PvRepository.Save();
            }
            catch (DataException)
            {
                return RedirectToAction("Delete",
                   new System.Web.Routing.RouteValueDictionary {
        { "id", id },
        { "saveChangesError", true } });
            }
            return RedirectToAction("Pv");
        }
        #endregion

        #region PvProfiles
       
        public ActionResult PvProfiles()
        {
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName");
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            IQueryable<SystemWeb.Models.PvProfile> pvProfile = db.PvProfile
                .Where(c => currentUser.pvID == c.pvID)
                .Include(p => p.Pv);
            var sql = pvProfile.ToString();
            return View(pvProfile.ToList());
        }

        public ActionResult PvProfilesDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvProfile pvProfile = db.PvProfile.Find(id);
            if (pvProfile == null)
            {
                return HttpNotFound();
            }
            return View(pvProfile);
        }
    
        public ActionResult PvProfilesCreate()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            IQueryable<SystemWeb.Models.Pv> pv = db.Pv
                .Where(c => currentUser.pvID == c.pvID);
            var sql = pv.ToList();
            ViewBag.pvID = new SelectList(pv, "pvID", "pvName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvProfilesCreate([Bind(Include = "PvProfileId,pvID,Indirizzo,Città,Nazione,Cap")] SystemWeb.Models.PvProfile pvProfile)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            var pvp = (from PvProfile in db.PvProfile where currentUser.pvID == PvProfile.pvID select PvProfile.PvProfileId).ToString();

            if (ModelState.IsValid)
            {
                pvProfile.PvProfileId = Guid.NewGuid();
                db.PvProfile.Add(pvProfile);
                db.SaveChanges();
                return RedirectToAction("PvProfiles");
            }
            return View(pvProfile);
        }
    
        public ActionResult PvProfilesEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvProfile pvProfile = db.PvProfile.Find(id);
            if (pvProfile == null)
            {
                return HttpNotFound();
            }
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName", pvProfile.pvID);
            return View(pvProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvProfilesEdit([Bind(Include = "PvProfileId,pvID,Indirizzo,Città,Nazione,Cap")] PvProfile pvProfile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pvProfile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("PvProfiles");
            }
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName", pvProfile.pvID);
            return View(pvProfile);
        }
      
        public ActionResult PvProfilesDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvProfile pvProfile = db.PvProfile.Find(id);
            if (pvProfile == null)
            {
                return HttpNotFound();
            }
            return View(pvProfile);
        }

        [HttpPost, ActionName("PvProfilesDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PvProfilesDeleteConfirmed(Guid id)
        {
            SystemWeb.Models.PvProfile pvProfile = db.PvProfile.Find(id);
            db.PvProfile.Remove(pvProfile);
            db.SaveChanges();
            return RedirectToAction("PvProfiles");
        }
        #endregion

        #region PvTanks
        [Route("user/tank")]
        public ActionResult PvTanks()
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            var DataSource = new MyDbContext().PvTank.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.Modello).ToList();
            ViewBag.datasource = DataSource;

            IEnumerable DataSource2 = new MyDbContext().Pv.Where(c => currentUser.pvID == c.pvID).ToList();
            ViewBag.datasource2 = DataSource2;

            IEnumerable DataSource3 = new MyDbContext().Product.ToList();
            ViewBag.datasource3 = DataSource3;

            return View();
        }

        [Route("user/tank/update")]
        public ActionResult UpdatePvTanks(PvTank value)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            TankRepository.Update(value);
            var data = context.PvTank.Where(c => currentUser.pvID == c.pvID);
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        [Route("user/tank/insert")]
        public ActionResult InsertPvTanks(PvTank value)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            TankRepository.Add(value);
            var data = context.PvTank.Where(c => currentUser.pvID == c.pvID);
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/tank/remove")]
        public ActionResult RemovePvTanks(Guid key)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            context.PvTank.Remove(context.PvTank.Single(o => o.PvTankId == key));
            context.SaveChanges();

            var data = context.PvTank.Where(c => currentUser.pvID == c.pvID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("user/tank/tanksexcell")]
        public ActionResult PvTanksExcell(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            ExcelExport exp = new ExcelExport();
            MyDbContext context = new MyDbContext();
            Guid now = Guid.NewGuid();
            IEnumerable DataSource = context.PvTank.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.Modello).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[2].DataSource = context.Product.ToList();

            exp.Export(obj, DataSource, now.ToString() + " - PvTank.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/tank/tanksword")]
        public ActionResult PvTanksWord(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            Guid now = Guid.NewGuid();
            WordExport exp = new WordExport();

            IEnumerable DataSource = context.PvTank.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.Modello).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[2].DataSource = context.Product.ToList();

            exp.Export(obj, DataSource, now.ToString() + " - PvTank.docx", false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/tank/tankspdf")]
        public ActionResult PvTanksPdf(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();

            Guid now = Guid.NewGuid();

            PdfExport exp = new PdfExport();

            IEnumerable DataSource = context.PvTank.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.Modello).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[2].DataSource = context.Product.ToList();

            exp.Export(obj, DataSource, now.ToString() + " - PvTank.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PvTanksDesc
    
        public ActionResult PvTanksDesc()
        {
            var pvTankDescs = db.PvTankDesc.Include(p => p.PvTank);
            return View(pvTankDescs.ToList());
        }
 
        public ActionResult PvTanksDescDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvTankDesc pvTankDesc = db.PvTankDesc.Find(id);
            if (pvTankDesc == null)
            {
                return HttpNotFound();
            }
            return View(pvTankDesc);
        }
    
        public ActionResult PvTanksDescCreate()
        {
            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Modello");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        internal ActionResult PvTanksDescCreate([Bind(Include = "PvTankDescId,PvTankId,PvTankCM,PvTankLT")] SystemWeb.Models.PvTankDesc pvTankDesc)
        {
            if (ModelState.IsValid)
            {
                pvTankDesc.PvTankDescId = Guid.NewGuid();
                db.PvTankDesc.Add(pvTankDesc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Modello", pvTankDesc.PvTankId);
            return View(pvTankDesc);
        }
     
        public ActionResult PvTanksDescEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvTankDesc pvTankDesc = db.PvTankDesc.Find(id);
            if (pvTankDesc == null)
            {
                return HttpNotFound();
            }
            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Modello", pvTankDesc.PvTankId);
            return View(pvTankDesc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvTanksDescEdit([Bind(Include = "PvTankDescId,PvTankId,PvTankCM,PvTankLT")] SystemWeb.Models.PvTankDesc pvTankDesc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pvTankDesc).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Modello", pvTankDesc.PvTankId);
            return View(pvTankDesc);
        }
     
        public ActionResult PvTanksDescDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvTankDesc pvTankDesc = db.PvTankDesc.Find(id);
            if (pvTankDesc == null)
            {
                return HttpNotFound();
            }
            return View(pvTankDesc);
        }

        [HttpPost, ActionName("PvTanksDescDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PvTanksDescDeleteConfirmed(Guid id)
        {
            SystemWeb.Models.PvTankDesc pvTankDesc = db.PvTankDesc.Find(id);
            db.PvTankDesc.Remove(pvTankDesc);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion

        #region PvDeficienze
   
        public ActionResult PvDeficienze(DateTime? dateFrom, DateTime? dateTo)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            IEnumerable DataSource = new MyDbContext().PvDeficienze.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(ly))).OrderBy(o => o.FieldDate).ToList();
            ViewBag.datasource = DataSource;

            IEnumerable DataSource2 = new MyDbContext().PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            ViewBag.dataSource2 = DataSource2;

            if (dateFrom == null | dateTo == null)
            {
                dateFrom = new DateTime(2016, 12, 31);
                dateTo = DateTime.Now;

                var getAll = from a in _PvDeficienzeRepository.GetRecords().ToList()
                             where (Convert.ToDateTime(a.FieldDate) >= dateFrom)
                                   && (Convert.ToDateTime(a.FieldDate) <= dateTo)
                             select a;

                int SumG = getAll
                    .Where(z => currentUser.pvID == z.PvTank.pvID
                    && z.PvTank.Modello.Contains("MC-10993"))
                    .Sum(row => row.Value);
                
                int SumB = getAll
                    .Where(z => currentUser.pvID == z.PvTank.pvID
                    && z.PvTank.Modello.Contains("MC-10688"))
                    .Sum(row => row.Value);


                ViewBag.PvDeficienzeSumG = SumG;
                ViewBag.PvDeficienzeSumB = SumB;
            }

            if (dateFrom != null | dateTo != null)
            {
                var getAll = from a in _PvDeficienzeRepository.GetRecords().ToList()
                             where (currentUser.pvID == a.PvTank.pvID
                                   && (Convert.ToDateTime(a.FieldDate) >= dateFrom)
                                   && (Convert.ToDateTime(a.FieldDate) <= dateTo))
                             select a;

                int SumG = getAll
                    .Where(z => z.PvTank.Modello.Contains("MC-10993"))
                    .Sum(row => row.Value);

                int SumB = getAll
                    .Where(z => z.PvTank.Modello.Contains("MC-10688"))
                    .Sum(row => row.Value);

                ViewBag.PvDeficienzeSumGByDate = SumG;
                ViewBag.PvDeficienzeSumBByDate = SumB;
            }

            return View();
        }


        [Route("user/deficienze/update")]
        public ActionResult UpdateDeficienze(PvDeficienze value)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            DeficienzeRepository.Update(value);
            var data = context.PvDeficienze.Include(i => i.PvTank).Where(c => currentUser.pvID == c.PvTank.pvID && c.FieldDate.ToString().Contains(ly));
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/deficienze/insert")]
        public ActionResult InsertDeficienze(PvDeficienze value)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            DeficienzeRepository.Add(value);
            var data = context.PvDeficienze.Include(i => i.PvTank).Where(c => currentUser.pvID == c.PvTank.pvID && c.FieldDate.ToString().Contains(ly));
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/deficienze/remove")]
        public ActionResult RemoveDeficienze(Guid key)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            context.PvDeficienze.Remove(context.PvDeficienze.Single(o => o.PvDefId == key));
            context.SaveChanges();

            var data = context.PvDeficienze.Where(c => currentUser.pvID == c.PvTank.pvID && c.FieldDate.ToString().Contains(ly));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("user/deficienze/deficienzeexcell")]
        public ActionResult DeficienzeExcell(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            ExcelExport exp = new ExcelExport();
            MyDbContext context = new MyDbContext();
            Guid now = Guid.NewGuid();
            IEnumerable DataSource = context.PvDeficienze.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(ly))).OrderBy(o => o.FieldDate).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Deficienze.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/deficienze/deficienzeword")]
        public ActionResult DeficienzeWord(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            Guid now = Guid.NewGuid();
            WordExport exp = new WordExport();

            IEnumerable DataSource = context.PvDeficienze.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(ly))).OrderBy(o => o.FieldDate).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Deficienze.docx", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/deficienze/deficienzepdf")]
        public ActionResult DeficienzePdf(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();

            Guid now = Guid.NewGuid();

            PdfExport exp = new PdfExport();

            IEnumerable DataSource = context.PvDeficienze.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(ly))).OrderBy(o => o.FieldDate).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Deficienze.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult PvDeficienzeCreate()
        {
            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Modello");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvDeficienzeCreate([Bind(Include = "PvDefId,PvTankId,Value,FieldDate")] SystemWeb.Models.PvDeficienze pvDeficienze)
        {
            if (ModelState.IsValid)
            {
                pvDeficienze.PvDefId = Guid.NewGuid();
                db.PvDeficienze.Add(pvDeficienze);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Modello", pvDeficienze.PvTankId);
            return View(pvDeficienze);
        }
        
        #endregion

        #region PvCali
        [Route("user/cali")]
        public ActionResult PvCali(DateTime? dateFrom, DateTime? dateTo)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            
            IEnumerable DataSource = new MyDbContext().PvCali.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(ly))).OrderBy(o => o.FieldDate).ToList();
            ViewBag.datasource = DataSource;

            IEnumerable DataSource2 = new MyDbContext().PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            ViewBag.dataSource2 = DataSource2;

            #region DateFrom == Null
            if (dateFrom == null | dateTo == null)
            {
                dateFrom = new DateTime(2016, 12, 31);
                dateTo = DateTime.Now;

                var getAll = from a in _PvCaliRepository.GetRecords()
                             where (currentUser.pvID == a.PvTank.pvID
                                   && Convert.ToDateTime(a.FieldDate) >= dateFrom)
                                   && (Convert.ToDateTime(a.FieldDate) <= dateTo)
                             select a;

                int SumG = getAll
                    .Where(z => currentUser.pvID == z.PvTank.pvID
                    && z.PvTank.Modello.Contains("MC-10993"))
                    .Sum(row => row.Value);

                int SumB = getAll
                    .Where(z => currentUser.pvID == z.PvTank.pvID
                    && z.PvTank.Modello.Contains("MC-10688"))
                    .Sum(row => row.Value);


                ViewBag.PvCaliSumG = SumG;
                ViewBag.PvCaliSumB = SumB;
            }
            #endregion

            #region DateFrom != null
            if (dateFrom != null | dateTo != null)
            {
                var getAll = from a in _PvCaliRepository.GetRecords()
                             where (currentUser.pvID == a.PvTank.pvID
                                   && (Convert.ToDateTime(a.FieldDate) >= dateFrom)
                                   && (Convert.ToDateTime(a.FieldDate) <= dateTo))
                             select a;

                int SumG = getAll
                    .Where(z => z.PvTank.Modello.Contains("MC-10993"))
                    .Sum(row => row.Value);

                int SumB = getAll
                    .Where(z => z.PvTank.Modello.Contains("MC-10688"))
                    .Sum(row => row.Value);

                ViewBag.PvCaliSumGByDate = SumG;
                ViewBag.PvCaliSumBByDate = SumB;
            }
            #endregion 
            
            return View();
        }
       
        [Route("user/cali/update")]
        public ActionResult UpdateCalo(PvCali value)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            CaliRepository.Update(value);
            var data = context.PvCali.Include(i=>i.PvTank).Where(c => currentUser.pvID == c.PvTank.pvID && c.FieldDate.ToString().Contains(ly));
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/cali/insert")]
        public ActionResult InsertCalo(PvCali value)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            CaliRepository.Add(value);
            var data = context.PvCali.Include(i => i.PvTank).Where(c => currentUser.pvID == c.PvTank.pvID && c.FieldDate.ToString().Contains(ly));
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/cali/remove")]
        public ActionResult RemoveCalo(Guid key)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            context.PvCali.Remove(context.PvCali.Single(o => o.PvCaliId == key));
            context.SaveChanges();

            var data = context.PvCali.Where(c => currentUser.pvID == c.PvTank.pvID && c.FieldDate.ToString().Contains(ly));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("user/cali/caliexcell")]
        public ActionResult CaliExcell(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            ExcelExport exp = new ExcelExport();
            MyDbContext context = new MyDbContext();
            Guid now = Guid.NewGuid();
            IEnumerable DataSource = context.PvCali.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(ly))).OrderBy(o => o.FieldDate).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Cali.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/cali/caliword")]
        public ActionResult CaliWord(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            Guid now = Guid.NewGuid();
            WordExport exp = new WordExport();

            IEnumerable DataSource = context.PvCali.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(ly))).OrderBy(o => o.FieldDate).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Cali.docx", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/cali/calipdf")]
        public ActionResult CaliPdf(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();

            Guid now = Guid.NewGuid();

            PdfExport exp = new PdfExport();

            IEnumerable DataSource = context.PvCali.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(ly))).OrderBy(o => o.FieldDate).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Cali.pdf", false, false, "flat-saffron");

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
        
        public ActionResult PvCaliCreate()
        {
            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Modello");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvCaliCreate(SystemWeb.Models.PvCali pvCali)
        {
            if (ModelState.IsValid)
            {
                _PvCaliRepository.InsertRecords(pvCali);
                _PvCaliRepository.Save();
                return RedirectToAction("Index");
            }

            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Modello", pvCali.PvTankId);
            return View(pvCali);
        }

        #endregion 

        #region Manage
  
        public async Task<ActionResult> Manage(ManageMessageId? message, Pv pv)
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var somequals = (from Pv in db.Pv where currentUser.pvID == Pv.pvID select Pv.pvName).SingleOrDefault();
            ViewBag.ProfileName = currentUser.UserProfiles.ProfileName;
            ViewBag.ProfileSurname = currentUser.UserProfiles.ProfileSurname;
            ViewBag.ProfileAdress = currentUser.UserProfiles.ProfileAdress;
            ViewBag.ProfileCity = currentUser.UserProfiles.ProfileCity;
            ViewBag.ProfileZipCode = currentUser.UserProfiles.ProfileZipCode;
            ViewBag.ProfileNation = currentUser.UserProfiles.ProfileNation;
            ViewBag.ProfileInfo = currentUser.UserProfiles.ProfileInfo;
            ViewBag.PvId = somequals;
            ViewBag.CompanyId = currentUser.Company.Name;
            
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two factor provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "The phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(User.Identity.GetUserId()),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(User.Identity.GetUserId()),
                Logins = await UserManager.GetLoginsAsync(User.Identity.GetUserId()),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(User.Identity.GetUserId())
            };
            return View(model);
        }

        public ActionResult ManageRemoveLogin()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return View(linkedAccounts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageRemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }
   
        public ActionResult ManageAddPhoneNumber()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageAddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("ManageVerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        [HttpPost]
        public ActionResult ManageRememberBrowser()
        {
            var rememberBrowserIdentity = AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(User.Identity.GetUserId());
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, rememberBrowserIdentity);
            return RedirectToAction("Manage", "User");
        }

        [HttpPost]
        public ActionResult ManageForgetBrowser()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            return RedirectToAction("Manage", "User");
        }

        [HttpPost]
        public async Task<ActionResult> ManageEnableTFA()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Manage", "User");
        }

        [HttpPost]
        public async Task<ActionResult> ManageDisableTFA()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Manage", "User");
        }
   
        public async Task<ActionResult> ManageVerifyPhoneNumber(string phoneNumber)
        {
            // This code allows you exercise the flow without actually sending codes
            // For production use please register a SMS provider in IdentityConfig and generate a code here.
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            ViewBag.Status = "For DEMO purposes only, the current code is " + code;
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageVerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Manage", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        public async Task<ActionResult> ManageRemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.RemovePhoneSuccess });
        }
     
        public ActionResult ManageChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }
       
        public ActionResult ManageSetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageSetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInAsync(user, isPersistent: false);
                    }
                    return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
 
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageLinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }
 
        public async Task<ActionResult> ManageLinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

            #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";
        private int lastYear;

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion

        #endregion

        #region Dispenser
        [Route("user/dispenser")]
        public ActionResult Dispenser()
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            var DataSource = new MyDbContext().Dispenser.Where(c => currentUser.pvID == c.PvTank.pvID).OrderBy(o => o.Modello).ToList();
            ViewBag.datasource = DataSource;

            IEnumerable DataSource2 = new MyDbContext().PvTank.Where(c => currentUser.pvID == c.pvID).ToList();
            ViewBag.datasource2 = DataSource2;
            
            return View();
        }

        [Route("user/dispenser/update")]
        public ActionResult UpdateDispenser(Dispenser value)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            DispenserRepository.Update(value);
            var data = context.Dispenser.Where(c => currentUser.pvID == c.PvTank.pvID);
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        [Route("user/dispenser/insert")]
        public ActionResult InsertDispenser(Dispenser value)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            DispenserRepository.Add(value);
            var data = context.Dispenser.Where(c => currentUser.pvID == c.PvTank.pvID);
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/dispenser/remove")]
        public ActionResult RemoveDispenser(Guid key)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            context.Dispenser.Remove(context.Dispenser.Single(o => o.DispenserId == key));
            context.SaveChanges();

            var data = context.Dispenser.Where(c => currentUser.pvID == c.PvTank.pvID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("user/dispenser/dispenserexcell")]
        public ActionResult DispenserExcell(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            ExcelExport exp = new ExcelExport();
            MyDbContext context = new MyDbContext();
            Guid now = Guid.NewGuid();
            IEnumerable DataSource = context.Dispenser.Where(c => currentUser.pvID == c.PvTank.pvID).OrderBy(o => o.Modello).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Dispenser.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/dispenser/dispenserword")]
        public ActionResult DispenserWord(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            Guid now = Guid.NewGuid();
            WordExport exp = new WordExport();

            IEnumerable DataSource = context.Dispenser.Where(c => currentUser.pvID == c.PvTank.pvID).OrderBy(o => o.Modello).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Dispenser.docx", false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/dispenser/dispenserpdf")]
        public ActionResult DispenserPdf(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();

            Guid now = Guid.NewGuid();

            PdfExport exp = new PdfExport();

            IEnumerable DataSource = context.Dispenser.Where(c => currentUser.pvID == c.PvTank.pvID).OrderBy(o => o.Modello).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Dispenser.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        /*
        public ActionResult DispenserDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Dispenser dispenser = db.Dispenser.Find(id);
            if (dispenser == null)
            {
                return HttpNotFound();
            }
            return View(dispenser);
        }
   
        public ActionResult DispenserCreate()
        {
            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Descrizione");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DispenserCreate([Bind(Include = "DispenserId,Modello,PvTankId")] Dispenser dispenser)
        {
            if (ModelState.IsValid)
            {
                dispenser.DispenserId = Guid.NewGuid();
                dispenser.isActive = true;
                db.Dispenser.Add(dispenser);
                db.SaveChanges();
                return RedirectToAction("Dispenser");
            }

            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Descrizione", dispenser.PvTankId);
            return View(dispenser);
        }
     
        public ActionResult DispenserEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dispenser dispenser = db.Dispenser.Find(id);
            if (dispenser == null)
            {
                return HttpNotFound();
            }
            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Descrizione", dispenser.PvTankId);
            return View(dispenser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DispenserEdit([Bind(Include = "DispenserId,Modello,Descrizione")] SystemWeb.Models.Dispenser dispenser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dispenser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Dispenser");
            }
            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Descrizione", dispenser.PvTankId);
            return View(dispenser);
        }
   
        public ActionResult DispenserDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Dispenser dispenser = db.Dispenser.Find(id);
            if (dispenser == null)
            {
                return HttpNotFound();
            }
            return View(dispenser);
        }

        [HttpPost, ActionName("DispenserDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DispenserDeleteConfirmed(Guid id)
        {
            SystemWeb.Models.Dispenser dispenser = db.Dispenser.Find(id);
            db.Dispenser.Remove(dispenser);
            db.SaveChanges();
            return RedirectToAction("Dispenser");
        }*/

        #endregion

        #region Companies
     
        public ActionResult Companies()
        {
            var company = db.Company.Include(c => c.RagioneSociale);
            return View(company.ToList());
        }
       
        public ActionResult CompaniesDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Company company = db.Company.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }
     
        public ActionResult CompaniesCreate()
        {
            ViewBag.RagioneSocialeId = new SelectList(db.RagioneSociale, "RagioneSocialeId", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompaniesCreate([Bind(Include = "CompanyId,Name,PartitaIva,RagioneSocialeId")] SystemWeb.Models.Company company)
        {
            if (ModelState.IsValid)
            {
                company.CompanyId = Guid.NewGuid();
                db.Company.Add(company);
                db.SaveChanges();
                return RedirectToAction("Companies");
            }

            ViewBag.RagioneSocialeId = new SelectList(db.RagioneSociale, "RagioneSocialeId", "Nome", company.RagioneSocialeId);
            return View(company);
        }

        public ActionResult CompaniesEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Company company = db.Company.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            ViewBag.RagioneSocialeId = new SelectList(db.RagioneSociale, "RagioneSocialeId", "Nome", company.RagioneSocialeId);
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompaniesEdit([Bind(Include = "CompanyId,Name,PartitaIva,RagioneSocialeId")] SystemWeb.Models.Company company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Companies");
            }
            ViewBag.RagioneSocialeId = new SelectList(db.RagioneSociale, "RagioneSocialeId", "Nome", company.RagioneSocialeId);
            return View(company);
        }
 
        public ActionResult CompaniesDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Company company = db.Company.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        [HttpPost, ActionName("CompaniesDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            SystemWeb.Models.Company company = db.Company.Find(id);
            db.Company.Remove(company);
            db.SaveChanges();
            return RedirectToAction("Companies");
        }
        #endregion

        #region Task
 
        public ActionResult Task(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Erogatori_Desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            IQueryable<SystemWeb.Models.CompanyTask> companyTask = (from r in db.CompanyTask
                                                   select r)
                .Include(c => c.ApplicationUser)
                .OrderBy(q => (q.FieldDate));
            if (!String.IsNullOrEmpty(searchString))
            {
                companyTask = companyTask.Where(s => s.FieldDate.ToString().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "date_desc":
                    companyTask = companyTask.OrderBy(s => s.FieldDate);
                    break;
                default:
                    companyTask = companyTask.OrderByDescending(s => s.FieldDate);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(companyTask.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult TaskDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.CompanyTask companyTask = db.CompanyTask.Find(id);
            if (companyTask == null)
            {
                return HttpNotFound();
            }
            return View(companyTask);
        }
 
        public ActionResult TaskCreate()
        {
            ViewBag.UsersId = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaskCreate([Bind(Include = "CompanyTaskId,UsersId,FieldChiusura,FieldDate,FieldResult")] SystemWeb.Models.CompanyTask companyTask)
        {
            if (ModelState.IsValid)
            {
                companyTask.CompanyTaskId = Guid.NewGuid();
                db.CompanyTask.Add(companyTask);
                db.SaveChanges();
                return RedirectToAction("Task");
            }

            ViewBag.UsersId = new SelectList(db.Users, "Id", "UserName", companyTask.UsersId);
            return View(companyTask);
        }
     
        public ActionResult TaskEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.CompanyTask companyTask = db.CompanyTask.Find(id);
            if (companyTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.UsersId = new SelectList(db.Users, "Id", "UserName", companyTask.UsersId);
            return View(companyTask);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaskEdit([Bind(Include = "CompanyTaskId,UsersId,FieldChiusura,FieldDate,FieldResult")] SystemWeb.Models.CompanyTask companyTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companyTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Task");
            }
            ViewBag.UsersId = new SelectList(db.Users, "Id", "UserName", companyTask.UsersId);
            return View(companyTask);
        }

        public ActionResult TaskDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.CompanyTask companyTask = db.CompanyTask.Find(id);
            if (companyTask == null)
            {
                return HttpNotFound();
            }
            return View(companyTask);
        }

        [HttpPost, ActionName("TaskDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult TaskDeleteConfirmed(Guid id)
        {
            SystemWeb.Models.CompanyTask companyTask = db.CompanyTask.Find(id);
            db.CompanyTask.Remove(companyTask);
            db.SaveChanges();
            return RedirectToAction("Task");
        }
        #endregion

        #region Area

        [Authorize(Roles = "Administrator,User")]
        public ActionResult Area()
        {
            var userArea = db.UserArea.Include(u => u.ApplicationUser);
            return View(userArea.ToList());
        }

        [Authorize(Roles = "Administrator,User")]
        public ActionResult AreaDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.UserArea userArea = db.UserArea.Find(id);
            if (userArea == null)
            {
                return HttpNotFound();
            }
            return View(userArea);
        }

        [Authorize(Roles = "Administrator,User")]
        public ActionResult AreaCreate()
        {
            ViewBag.UsersId = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        [Authorize(Roles = "Administrator,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AreaCreate([Bind(Include = "UserAreaId,UsersId,UserFieldAccount,UserFieldUsername,UserFieldPassword,CreateDate")] SystemWeb.Models.UserArea userArea)
        {
            if (ModelState.IsValid)
            {
                userArea.UserAreaId = Guid.NewGuid();
                userArea.CreateDate = DateTime.Now;
                userArea.UsersId = User.Identity.GetUserId();
                db.UserArea.Add(userArea);
                db.SaveChanges();
                return RedirectToAction("Area");
            }

            ViewBag.UsersId = new SelectList(db.Users, "Id", "UserName", userArea.UsersId);
            return View(userArea);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult AreaEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.UserArea userArea = db.UserArea.Find(id);
            if (userArea == null)
            {
                return HttpNotFound();
            }
            ViewBag.UsersId = new SelectList(db.Users, "Id", "UserName", userArea.UsersId);
            return View(userArea);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AreaEdit([Bind(Include = "UserAreaId,UsersId,UserFieldAccount,UserFieldUsername,UserFieldPassword,CreateDate")] SystemWeb.Models.UserArea userArea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userArea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Area");
            }
            ViewBag.UsersId = new SelectList(db.Users, "Id", "UserName", userArea.UsersId);
            return View(userArea);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult AreaDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.UserArea userArea = db.UserArea.Find(id);
            if (userArea == null)
            {
                return HttpNotFound();
            }
            return View(userArea);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("AreaDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult AreaDeleteConfirmed(Guid id)
        {
            SystemWeb.Models.UserArea userArea = db.UserArea.Find(id);
            db.UserArea.Remove(userArea);
            db.SaveChanges();
            return RedirectToAction("Area");
        }
        #endregion

        #region UserProfiles
        /*
        public ActionResult Index()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            var myProfile = from a in db.UserProfiles
                            where currentUser.ProfileId == a.ProfileId
                            select a;

            return View(myProfile.ToList());
        }
        */
        // GET: Profiles/Details/5
        public ActionResult Profiles(Guid? id)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserProfiles profile = db.UserProfiles.Include(s => s.UsersImage).SingleOrDefault(s => s.ProfileId == id);

            if (profile == null)
            {
                return HttpNotFound();
            }

            return View(profile);
        }

            /*
            // GET: Profiles/Create
            public ActionResult Create()
            {
                return View();
            }

            // POST: Profiles/Create
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create([Bind(Include = "ProfileID,ProfileName,ProfileSurname,ProfileCF")] Profile profile, HttpPostedFileBase upload)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (upload != null && upload.ContentLength > 0)
                        {
                            var avatar = new File
                            {
                                FileName = string.Format(Guid.NewGuid() + "-" + System.IO.Path.GetFileName(upload.FileName)),
                                FileType = FileType.Avatar,
                                ContentType = upload.ContentType,
                                UploadDate = DateTime.Now.Date
                            };
                            using (var reader = new System.IO.BinaryReader(upload.InputStream))
                            {
                                avatar.Content = reader.ReadBytes(upload.ContentLength);
                            }
                            profile.File = new List<File> { avatar };
                        }
                        db.Profiles.Add(profile);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                catch (RetryLimitExceededException)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
                return View(profile);
            }
            */
            // GET: Profiles/Edit/5
            public ActionResult EditProfiles(Guid? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                UserProfiles profile = db.UserProfiles.Include(s => s.UsersImage).SingleOrDefault(s => s.ProfileId == id);
                if (profile == null)
                {
                    return HttpNotFound();
                }
                return View(profile);
            }

            // POST: Profiles/Edit/5
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost, ActionName("EditProfiles")]
            [ValidateAntiForgeryToken]
            public ActionResult EditPost(Guid? id, HttpPostedFileBase upload)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var profileToUpdate = db.UserProfiles.Find(id);
                if (TryUpdateModel(profileToUpdate, "",
                    new string[] { "ProfileName", "ProfileSurname", "ProfileAdress", "ProfileCity", "ProfileZipCode", "ProfileNation", "ProfileInfo" }))
                {
                    try
                    {
                        if (upload != null && upload.ContentLength > 0)
                        {
                            if (profileToUpdate.UsersImage.Any(f => f.FileType == FileType.Avatar))
                            {
                                db.UsersImage.Remove(profileToUpdate.UsersImage.First(f => f.FileType == FileType.Avatar));
                            }
                            var avatar = new UsersImage
                            {
                                UsersImageName = string.Format(Guid.NewGuid() + "-" + System.IO.Path.GetFileName(upload.FileName)),
                                FileType = FileType.Avatar,
                                ContentType = upload.ContentType,
                                UploadDate = DateTime.Now.Date
                            };
                            using (var reader = new System.IO.BinaryReader(upload.InputStream))
                            {
                                avatar.Content = reader.ReadBytes(upload.ContentLength);
                            }
                            profileToUpdate.UsersImage = new List<UsersImage> { avatar };
                        }
                        db.Entry(profileToUpdate).State = EntityState.Modified;
                        db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    catch (RetryLimitExceededException /* dex */)
                    {
                        //Log the error (uncomment dex variable name and add a line here to write a log.
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    }
                }
                return View(profileToUpdate);
            }

            [Authorize(Roles = "Administrators")]
            // GET: Profiles/Delete/5
            public ActionResult DeleteProfiles(Guid? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                UserProfiles profile = db.UserProfiles.Find(id);
                if (profile == null)
                {
                    return HttpNotFound();
                }
                return View(profile);
            }

            // POST: Profiles/Delete/5
            [Authorize(Roles = "Administrators")]
            [HttpPost, ActionName("DeleteProfiles")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteProfilesConfirmed(Guid id)
            {
                UserProfiles profile = db.UserProfiles.Find(id);
                db.UserProfiles.Remove(profile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

        #endregion

        protected override void Dispose(bool disposing)
        {
            MyDbContext db = new MyDbContext();
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);

        }
    }
}