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
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;

namespace SystemWeb.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        #region Inizializzatori
        private MyDbContext db = new MyDbContext();
        private SelectList disp;
        public string ly { get; set; }
        public UserController()
        {
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

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, DateTime? dateFrom, DateTime? dateTo, Guid? id)
        {
            #region List

            UserIndexViewModel list = new UserIndexViewModel();

            list.carico = db.Carico.ToList();

            list.pverogatori = db.PvErogatori.ToList();

            list.pv = db.Pv.ToList();

            list.pvprofile = db.PvProfile.ToList();

            list.pvtank = db.PvTank.ToList();

            list.dispenser = db.Dispenser.ToList();

            list.company = db.Company.ToList();

            list.companytask = db.CompanyTask.ToList();

            list.userarea = db.UserArea.ToList();

            list.applicationuser = db.Users.ToList();

            list.usersimage = db.UsersImage.ToList();

            #endregion

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            var getPId = from a in db.UserProfiles
                         where a.ProfileId == currentUser.ProfileId
                         select a;

            ViewBag.profileId = getPId;

            UserProfiles profile = db.UserProfiles.Include(s => s.UsersImage).SingleOrDefault(s => s.ProfileId == id);
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

            var getAll = from a in db.PvErogatori.ToList()
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
            //ViewBag.PvId = pv;
            ViewBag.ProfileName = currentUser.UserProfiles.ProfileName;
            ViewBag.ProfileSurname = currentUser.UserProfiles.ProfileSurname;
            ViewBag.CompanyId = currentUser.Company.Name;
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

        // GET: FilePaths/Create
        public ActionResult FileCreate()
        {
            ViewBag.UserID = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: FilePaths/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FileCreate([Bind(Include = "FilePathID,FileName,FileType,UploadDate,UserID")] FilePath filePath, ApplicationUser user, HttpPostedFileBase upload)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    string filename = Path.GetFileName(Guid.NewGuid() + "." + upload.FileName);
                    var filepath = Path.Combine(Server.MapPath("~/Uploads/Documents/"), filename);
                    upload.SaveAs(filepath);

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

                return RedirectToAction("File");
            }

            ViewBag.UserID = new SelectList(db.Users, "Id", "UserName", filePath.UserID);
            return View(filePath);
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
        [HttpPost, ActionName("Delete")]
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

        public ActionResult Carico( string sortOrder, string currentFilter, string searchString, int? page, DateTime? dateFrom, DateTime? dateTo)
        {
            // Ritorna tutti gli oggetti in Carico
            var getall = from a in db.Carico.ToList()
                        select a;

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "Ordine_Desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.YearSortParm = sortOrder == "Year" ? "year_desc" : "Year";
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName");
            ViewBag.yearId = new SelectList(db.Year, "yearId", "Anno");

            // Totale Carico Benzina secondo il parametro di ricerca specificato
            ViewBag.SSPBTotalAmountFrom = getall.ToList()
                .Where(o => currentUser.pvID == o.pvID && Convert.ToDateTime(o.cData) >= dateFrom && Convert.ToDateTime(o.cData) <= dateTo)
                .Sum(o => (decimal?) o.Benzina);
            // Totale Carico Gasolio secondo il parametro di ricerca specificato
            ViewBag.DieselTotalAmountFrom = getall.ToList()
                .Where(o => currentUser.pvID == o.pvID && Convert.ToDateTime(o.cData) >= dateFrom && Convert.ToDateTime(o.cData) <= dateTo)
                .Sum(o => (decimal?) o.Gasolio);
            
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            
            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();
            IQueryable<Carico> carico = (from r in db.Carico
                                         select r)
                .Where(c => currentUser.pvID == c.pvID && c.Year.Anno.Year.ToString().Contains(ly))
                .Include(c => c.Pv)
                .Include(c => c.Year);

            if (!String.IsNullOrEmpty(searchString))
            {
                carico = carico.Where(s => s.Ordine.ToString().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "Ordine_Desc":
                    carico = carico.OrderByDescending(s => s.Ordine);
                    break;
                case "year_desc":
                    carico = carico.OrderByDescending(s => s.yearId);
                    break;
                default:
                    carico = carico.OrderBy(s => s.Ordine);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
           
            // Totale Carico annuo benzina.
            ViewBag.SSPBTotalAmount = db.Carico.Where(o => currentUser.pvID == o.pvID && o.Year.Anno.Year.ToString().Contains(ly)).Sum(o => (decimal?) o.Benzina);

            // Totale Carico annuo gasolio. 
            ViewBag.DieselTotalAmount = db.Carico.Where(o => currentUser.pvID == o.pvID && o.Year.Anno.Year.ToString().Contains(ly)).Sum(o => (decimal?) o.Gasolio);

            return View(carico.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult CaricoChart()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            SystemWeb.Models.Carico objCaricoModel = new SystemWeb.Models.Carico();
            objCaricoModel.sspb = "Benzina";
            objCaricoModel.dsl = "Gasolio";
            /*
            ViewBag.Anno = db.Carico.Where(o => currentUser.pvID == o.pvID).Select(o => o.cData.Year).ToString();
            ViewBag.Mese = db.Carico.Where(o => currentUser.pvID == o.pvID).Select(o => o.cData.Month).ToString();
            ViewBag.Giorno = db.Carico.Where(o => currentUser.pvID == o.pvID).Select(o => o.cData.Day).ToString();
            */
            return View(objCaricoModel);
        }

        public ActionResult CaricoGetChart()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();

            return Json(db.Carico.ToList()
                .Where(c => currentUser.pvID == c.pvID && c.Year.Anno.Year.ToString().Contains(ly))
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

        public ActionResult CaricoDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Carico carico = db.Carico.Find(id);
            if (carico == null)
            {
                return HttpNotFound();
            }
            return View(carico);
        }
        
        public ActionResult CaricoCreate()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

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
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CaricoCreate([Bind(Include = "Id,pvID,yearId,Ordine,cData,Documento,Numero,rData,Emittente,Benzina,Gasolio,Note")] Carico carico)
        {
            if (ModelState.IsValid)
            {
                carico.Id = Guid.NewGuid();
                db.Carico.Add(carico);
                db.SaveChanges();
                return RedirectToAction("Carico");
            }

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

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
            return View(carico);
        }
        
        public ActionResult CaricoEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carico carico = db.Carico.Find(id);
            if (carico == null)
            {
                return HttpNotFound();
            }
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            IQueryable<Pv> pv = db.Pv
                .Where(c => currentUser.pvID == c.pvID);
            var sql = pv.ToList();
            IQueryable<Year> year = db.Year;
            var sql2 = year.ToList();
            ViewBag.pvID = new SelectList(pv, "pvID", "pvName", carico.pvID);
            ViewBag.yearId = new SelectList(year, "yearId", "Anno");
            return View(carico);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CaricoEdit([Bind(Include = "Id,pvID,yearId,Ordine,cData,Documento,Numero,rData,Emittente,Benzina,Gasolio,Note")] SystemWeb.Models.Carico carico)
        {
            if (ModelState.IsValid)
            {
                db.Entry(carico).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Carico");
            }
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName", carico.pvID);
            ViewBag.yearId = new SelectList(db.Year, "yearId", "Anno", carico.yearId);
            return View(carico);
        }
        
        public ActionResult CaricoDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carico carico = db.Carico.Find(id);
            if (carico == null)
            {
                return HttpNotFound();
            }
            return View(carico);
        }
        
        [HttpPost, ActionName("CaricoDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult CaricoDeleteConfirmed(Guid id)
        {
            Carico carico = db.Carico.Find(id);
            db.Carico.Remove(carico);
            db.SaveChanges();
            return RedirectToAction("Carico");
        }
        #endregion

        #region PvErogatori

        /*
        public IList<PvErogatoriViewModel> GetPvErogatoriList()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            DateTime da;
            DateTime al;

            da = new DateTime(2016, 12, 31);
            al = DateTime.Now;

            MyDbContext db = new MyDbContext();

            var getByKnownDate = from a in db.PvErogatori.ToList()

                                 where (currentUser.pvID == a.pvID && (Convert.ToDateTime(a.FieldDate) >= da)
                                       && (Convert.ToDateTime(a.FieldDate) <= al))
                                 select a;

            int maxB = getByKnownDate
                .Where(z => (z.Product.Nome.Contains("B")))
                .Max(row => row.Value);
            int minB = getByKnownDate
                .Where(z => (z.Product.Nome.Contains("B")))
                .Min(row => row.Value);

            int maxG = getByKnownDate
                .Where(z => (z.Product.Nome.Contains("G")))
                .Max(row => row.Value);
            int minG = getByKnownDate
                .Where(z => (z.Product.Nome.Contains("G")))
                .Min(row => row.Value);

            var pverogatoriList = (from e in db.PvErogatori
                                   join a in db.Product on e.ProductId equals a.ProductId
                                   join b in db.Pv on e.pvID equals b.pvID
                                   join c in db.Dispenser on e.DispenserId equals c.DispenserId
                                   where (currentUser.pvID == e.pvID && (Convert.ToDateTime(e.FieldDate) >= da)
                                      && (Convert.ToDateTime(e.FieldDate) <= al))
                                   select new PvErogatoriViewModel
                                   {
                                       PVEROGATORIID = e.PvErogatoriId,
                                       PV = b.pvName,
                                       PRODUCT = a.Nome,
                                       DISPENSER = c.Modello,
                                       DATE = e.FieldDate,
                                       VALUE = e.Value,
                                       TOTSSPB = (maxB - minB),
                                       TOTDSL = (maxG - minG)
                                   }).ToList();
            return pverogatoriList;
        }
        */
        public ActionResult PvErogatori(string sortOrder, string currentFilter, string searchString, int? page, DateTime? dateFrom, DateTime? dateTo, [Bind(Include = "DispenserId")] PvErogatori pvEr)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

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

                var getByKnownDate = from a in db.PvErogatori.ToList()

                             where (currentUser.pvID == a.pvID && (Convert.ToDateTime(a.FieldDate) >= da)
                                   && (Convert.ToDateTime(a.FieldDate) <= al))
                             select a;

                int maxB = getByKnownDate
                    .Where(z =>(z.Product.Nome.Contains("B")))
                    .Max(row => row.Value);
                int minB = getByKnownDate
                    .Where(z => (z.Product.Nome.Contains("B")))
                    .Min(row => row.Value);

                int maxG = getByKnownDate
                    .Where(z => (z.Product.Nome.Contains("G")))
                    .Max(row => row.Value);
                int minG = getByKnownDate
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

            #region getCounterByDispenser (Non è strettamente necessario ma necessita di ulteriore lavoro)
            /*
            if (disp == null)
            {
                disp = new SelectList(db.Dispenser, "DispenserId", "Modello");
                ViewBag.SelectDispenser = disp;
                var getCounterByDispenser = from a in db.PvErogatori.ToList()
                                            where (currentUser.pvID == a.pvID
                                            && (disp.ToString() == a.DispenserId.ToString())
                                            && (Convert.ToDateTime(a.FieldDate) >= dateFrom)
                                            && (Convert.ToDateTime(a.FieldDate) <= dateTo))
                                            select a;

                    int maxBdisp = getCounterByDispenser
                        .Where(z => currentUser.pvID == z.pvID
                        && (z.Product.Nome.Contains("B")))
                        .Max(row => row.Value);
                    int minBdisp = getCounterByDispenser
                        .Where(z => currentUser.pvID == z.pvID
                        && (z.Product.Nome.Contains("B")))
                        .Min(row => row.Value);

                    int maxGdisp = getCounterByDispenser
                        .Where(z => currentUser.pvID == z.pvID
                        && (z.Product.Nome.Contains("G")))
                        .Max(row => row.Value);
                    int minGdisp = getCounterByDispenser
                        .Where(z => currentUser.pvID == z.pvID
                        && (z.Product.Nome.Contains("G")))
                        .Min(row => row.Value);

                    ViewBag.SSPBTotalAmountFromDisp = maxBdisp - minBdisp;
                    ViewBag.DieselTotalAmountFromDisp = maxGdisp - minGdisp;
                }
                */
            #endregion

            if (dateFrom != null | dateTo != null)
            {
                var getAll = from a in db.PvErogatori.ToList()
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

                ViewBag.SSPBTotalAmountFrom = maxB - minB;
                ViewBag.DieselTotalAmountFrom = maxG - minG;
            }

            #region Dispenser (necessita di ulteriore lavoro)
            /*
            if (disp != null)
            {
                disp = new SelectList(db.Dispenser, "DispenserId", "Modello", pvEr.DispenserId);
                ViewBag.SelectDispenser = disp;
                var getCounterByDispenser = from a in db.PvErogatori.ToList()
                                            where (currentUser.pvID == a.pvID
                                            && (disp.ToString() == a.DispenserId.ToString())
                                            && (Convert.ToDateTime(a.FieldDate) >= dateFrom)
                                            && (Convert.ToDateTime(a.FieldDate) <= dateTo))
                                            select a;

                int maxBdisp = getCounterByDispenser
                    .Where(z => (z.Product.Nome.Contains("B")))
                    .Max(row => row.Value);
                int minBdisp = getCounterByDispenser
                    .Where(z => (z.Product.Nome.Contains("B")))
                    .Min(row => row.Value);

                int maxGdisp = getCounterByDispenser
                    .Where(z => (z.Product.Nome.Contains("G")))
                    .Max(row => row.Value);
                int minGdisp = getCounterByDispenser
                    .Where(z => (z.Product.Nome.Contains("G")))
                    .Min(row => row.Value);

                ViewBag.SSPBTotalAmountFromDisp = maxBdisp - minBdisp;
                ViewBag.DieselTotalAmountFromDisp = maxGdisp - minGdisp;
            }
             * */
            #endregion

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Erogatori_Desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.DispenserId = new SelectList(db.Dispenser, "DispenserId", "Modello");
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Nome");
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName");

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            IQueryable<PvErogatori> pvErogatori = (from r in db.PvErogatori
                                                   select r)
                .Where(a => currentUser.pvID == a.pvID && a.FieldDate.Year.ToString().Contains(ly))
                .Include(c => c.Dispenser)
                .Include(c => c.Product)
                .Include(c => c.Pv);
            if (!string.IsNullOrEmpty(searchString))
            {
                pvErogatori = pvErogatori.Where(s => s.FieldDate.ToString().Contains(searchString.ToUpper()));
                //pvErogatori = pvErogatori.Where(s => s.Value.ToString().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "date_desc":
                    pvErogatori = pvErogatori.OrderByDescending(s => s.FieldDate);
                    //pvErogatori = pvErogatori.OrderByDescending(s => s.Value);
                    break;
                default:
                    pvErogatori = pvErogatori.OrderBy(s => s.FieldDate);
                    //pvErogatori = pvErogatori.OrderBy(s => s.Value);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            /*
            DateTime fromm;
            DateTime to;

            fromm = new DateTime(2016, 12, 31);
            to = DateTime.Now;
            var exportExcell = from a in db.PvErogatori.ToList()

                                 where (currentUser.pvID == a.pvID && (Convert.ToDateTime(a.FieldDate) >= fromm)
                                       && (Convert.ToDateTime(a.FieldDate) <= to))
                                 select a;

            int maxxB = exportExcell
                .Where(z => (z.Product.Nome.Contains("B")))
                .Max(row => row.Value);
            int minnB = exportExcell
                .Where(z => (z.Product.Nome.Contains("B")))
                .Min(row => row.Value);

            int maxxG = exportExcell
                .Where(z => (z.Product.Nome.Contains("G")))
                .Max(row => row.Value);
            int minnG = exportExcell
                .Where(z => (z.Product.Nome.Contains("G")))
                .Min(row => row.Value);

            var gv = new GridView();
            gv.DataSource = exportExcell;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Totalizzatori.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            */

            return View(pvErogatori.ToPagedList(pageNumber, pageSize));
        }

        /*
        public ActionResult PvErogatoriExportToExcel()
        {
            var gv = new GridView();
            gv.DataSource = this.GetPvErogatoriList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Totalizzatori.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("PvErogatori");
        }
        */
        public ActionResult PvErogatoriChart()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            SystemWeb.Models.PvErogatori objPvErogatoriModel = new SystemWeb.Models.PvErogatori();
            objPvErogatoriModel.sspb = "Benzina";
            objPvErogatoriModel.dsl = "Gasolio";

            return View(objPvErogatoriModel);
        }

        public ActionResult PvErogatoriGetChart()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            int i = 1;
            DateTime dateFrom = new DateTime(2016, 01, 01);
            dateFrom.AddDays(i++);
            DateTime dateTo = DateTime.Now.AddDays(i--);

            var getAll = from a in db.PvErogatori.ToList()
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

            return Json(db.PvErogatori.ToList()
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

        public ActionResult PvErogatoriDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvErogatori pvErogatori = db.PvErogatori.Find(id);
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
            IQueryable<SystemWeb.Models.Pv> pv = db.Pv
                .Where(c => currentUser.pvID == c.pvID);
            var sql = pv.ToList();
            ViewBag.pvID = new SelectList(pv, "pvID", "pvName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvErogatoriCreate([Bind(Include = "PvErogatoriId,pvID,ProductId,DispenserId,Value,FieldDate")] SystemWeb.Models.PvErogatori pvErogatori)
        {
            if (ModelState.IsValid)
            {
                pvErogatori.PvErogatoriId = Guid.NewGuid();
                db.PvErogatori.Add(pvErogatori);
                db.SaveChanges();
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
            SystemWeb.Models.PvErogatori pvErogatori = db.PvErogatori.Find(id);
            if (pvErogatori == null)
            {
                return HttpNotFound();
            }
            ViewBag.DispenserId = new SelectList(db.Dispenser, "DispenserId", "Modello", pvErogatori.DispenserId);
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Nome", pvErogatori.ProductId);
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            IQueryable<SystemWeb.Models.Pv> pv = db.Pv
                .Where(c => currentUser.pvID == c.pvID);
            var sql = pv.ToList();
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName", pvErogatori.pvID);
            return View(pvErogatori);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvErogatoriEdit([Bind(Include = "PvErogatoriId,pvID,ProductId,DispenserId,Value,FieldDate")] SystemWeb.Models.PvErogatori pvErogatori)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pvErogatori).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("PvErogatori");
            }
            ViewBag.DispenserId = new SelectList(db.Dispenser, "DispenserId", "Modello", pvErogatori.DispenserId);
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Nome", pvErogatori.ProductId);
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName", pvErogatori.pvID);
            return View(pvErogatori);
        }

        public ActionResult PvErogatoriDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvErogatori pvErogatori = db.PvErogatori.Find(id);
            if (pvErogatori == null)
            {
                return HttpNotFound();
            }
            return View(pvErogatori);
        }

        [HttpPost, ActionName("PvErogatoriDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PvErogatoriDeleteConfirmed(Guid id)
        {
            SystemWeb.Models.PvErogatori pvErogatori = db.PvErogatori.Find(id);
            db.PvErogatori.Remove(pvErogatori);
            db.SaveChanges();
            return RedirectToAction("PvErogatori");
        }
        #endregion

        #region Pv

        public ActionResult Pv()
        {
            var pv = db.Pv.Include(p => p.Flag);
            return View(pv.ToList());
        }
  
        public ActionResult PvDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Pv pv = db.Pv.Find(id);
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
        public ActionResult PvCreate([Bind(Include = "pvID,pvName,pvFlagId")] SystemWeb.Models.Pv pv)
        {
            if (ModelState.IsValid)
            {
                pv.pvID = Guid.NewGuid();
                db.Pv.Add(pv);
                db.SaveChanges();
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
            SystemWeb.Models.Pv pv = db.Pv.Find(id);
            if (pv == null)
            {
                return HttpNotFound();
            }
            ViewBag.pvFlagId = new SelectList(db.Flag, "pvFlagId", "Nome", pv.pvFlagId);
            return View(pv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvEdit([Bind(Include = "pvID,pvName,pvFlagId")] SystemWeb.Models.Pv pv)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pv).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Pv");
            }
            ViewBag.pvFlagId = new SelectList(db.Flag, "pvFlagId", "Nome", pv.pvFlagId);
            return View(pv);
        }
      
        public ActionResult PvDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Pv pv = db.Pv.Find(id);
            if (pv == null)
            {
                return HttpNotFound();
            }
            return View(pv);
        }

        [HttpPost, ActionName("PvDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PvDeleteConfirmed(Guid id)
        {
            SystemWeb.Models.Pv pv = db.Pv.Find(id);
            db.Pv.Remove(pv);
            db.SaveChanges();
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
      
        public ActionResult PvTanks()
        {
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName");
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            IQueryable<SystemWeb.Models.PvTank> pvTank = db.PvTank
                .Where(c => currentUser.pvID == c.pvID)
                .Include(p => p.Product)
                .Include(p => p.Pv);
            var sql = pvTank.ToString();
            return View(pvTank.ToList());
        }
      
        public ActionResult PvTanksDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvTank pvTank = db.PvTank.Find(id);
            if (pvTank == null)
            {
                return HttpNotFound();
            }
            return View(pvTank);
        }
       
        public ActionResult PvTanksCreate()
        {
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Nome");
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
        public ActionResult PvTanksCreate([Bind(Include = "PvTankId,pvID,ProductId,Modello,Anno,Capienza,Giacenza,Descrizione")] SystemWeb.Models.PvTank pvTank)
        {
            if (ModelState.IsValid)
            {
                pvTank.PvTankId = Guid.NewGuid();
                db.PvTank.Add(pvTank);
                db.SaveChanges();
                return RedirectToAction("PvTanks");
            }

            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Nome", pvTank.ProductId);
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName", pvTank.pvID);
            return View(pvTank);
        }
    
        public ActionResult PvTanksEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvTank pvTank = db.PvTank.Find(id);
            if (pvTank == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Nome", pvTank.ProductId);
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName", pvTank.pvID);
            return View(pvTank);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvTanksEdit([Bind(Include = "PvTankId,pvID,ProductId,Modello,Anno,Capienza,Giacenza,Descrizione")] SystemWeb.Models.PvTank pvTank)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pvTank).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("PvTanks");
            }
            ViewBag.ProductId = new SelectList(db.Product, "ProductId", "Nome", pvTank.ProductId);
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName", pvTank.pvID);
            return View(pvTank);
        }
       
        public ActionResult PvTanksDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvTank pvTank = db.PvTank.Find(id);
            if (pvTank == null)
            {
                return HttpNotFound();
            }
            return View(pvTank);
        }

        [HttpPost, ActionName("PvTanksDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PvTanksDeleteConfirmed(Guid id)
        {
            SystemWeb.Models.PvTank pvTank = db.PvTank.Find(id);
            db.PvTank.Remove(pvTank);
            db.SaveChanges();
            return RedirectToAction("PvTanks");
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
   
        public ActionResult PvDeficienze(string sortOrder, string currentFilter, string searchString, int? page, DateTime? dateFrom, DateTime? dateTo)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            if (dateFrom == null | dateTo == null)
            {
                dateFrom = new DateTime(2016, 12, 31);
                dateTo = DateTime.Now;

                var getAll = from a in db.PvDeficienze.ToList()
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
                var getAll = from a in db.PvDeficienze.ToList()
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

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Deficienze_Desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.PvTank = new SelectList(db.PvTank, "pvID", "pvName");

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();

            IQueryable<SystemWeb.Models.PvDeficienze> pvDeficienze = (from r in db.PvDeficienze
                                                   select r)
                .Where(c => currentUser.pvID == c.PvTank.pvID && c.FieldDate.Year.ToString().Contains(ly))
                .Include(c => c.PvTank)
                .OrderBy(q => (q.FieldDate));
            if (!String.IsNullOrEmpty(searchString))
            {
                pvDeficienze = pvDeficienze.Where(s => s.FieldDate.ToString().Contains(searchString.ToUpper()));
                //pvErogatori = pvErogatori.Where(s => s.Value.ToString().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "date_desc":
                    pvDeficienze = pvDeficienze.OrderByDescending(s => s.FieldDate);
                    //pvErogatori = pvErogatori.OrderByDescending(s => s.Value);
                    break;
                default:
                    pvDeficienze = pvDeficienze.OrderBy(s => s.FieldDate);
                    //pvErogatori = pvErogatori.OrderBy(s => s.Value);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(pvDeficienze.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult PvDeficienzeDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvDeficienze pvDeficienze = db.PvDeficienze.Find(id);
            if (pvDeficienze == null)
            {
                return HttpNotFound();
            }
            return View(pvDeficienze);
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


        public ActionResult PvDeficienzeEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvDeficienze pvDeficienze = db.PvDeficienze.Find(id);
            if (pvDeficienze == null)
            {
                return HttpNotFound();
            }
            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Modello", pvDeficienze.PvTankId);
            return View(pvDeficienze);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvDeficienzeEdit([Bind(Include = "PvDefId,PvTankId,Value,FieldDate")] SystemWeb.Models.PvDeficienze pvDeficienze)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pvDeficienze).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Modello", pvDeficienze.PvTankId);
            return View(pvDeficienze);
        }
 
        public ActionResult PvDeficienzeDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvDeficienze pvDeficienze = db.PvDeficienze.Find(id);
            if (pvDeficienze == null)
            {
                return HttpNotFound();
            }
            return View(pvDeficienze);
        }

        [HttpPost, ActionName("PvDeficienzeDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PvDeficienzeDeleteConfirmed(Guid id)
        {
            SystemWeb.Models.PvDeficienze pvDeficienze = db.PvDeficienze.Find(id);
            db.PvDeficienze.Remove(pvDeficienze);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        #region PvCali
 
        public ActionResult PvCali(string sortOrder, string currentFilter, string searchString, int? page, DateTime? dateFrom, DateTime? dateTo)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            if (dateFrom == null | dateTo == null)
            {
                dateFrom = new DateTime(2016, 12, 31);
                dateTo = DateTime.Now;

                var getAll = from a in db.PvCali.ToList()
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


                ViewBag.PvCaliSumG = SumG;
                ViewBag.PvCaliSumB = SumB;
            }

            if (dateFrom != null | dateTo != null)
            {
                var getAll = from a in db.PvCali.ToList()
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

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Cali_Desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.PvTank = new SelectList(db.PvTank, "pvID", "pvName");

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            lastYear = DateTime.Today.Year;
            ly = lastYear.ToString();

            IQueryable<SystemWeb.Models.PvCali> pvCali = (from r in db.PvCali
                                                     select r)
                .Where(c => currentUser.pvID == c.PvTank.pvID && c.FieldDate.Year.ToString().Contains(ly))
                .Include(c => c.PvTank)
                .OrderBy(q => (q.FieldDate));
            if (!String.IsNullOrEmpty(searchString))
            {
                pvCali = pvCali.Where(s => s.FieldDate.ToString().Contains(searchString.ToUpper()));
                //pvErogatori = pvErogatori.Where(s => s.Value.ToString().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "date_desc":
                    pvCali = pvCali.OrderByDescending(s => s.FieldDate);
                    //pvErogatori = pvErogatori.OrderByDescending(s => s.Value);
                    break;
                default:
                    pvCali = pvCali.OrderBy(s => s.FieldDate);
                    //pvErogatori = pvErogatori.OrderBy(s => s.Value);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(pvCali.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult PvCaliDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvCali pvCali = db.PvCali.Find(id);
            if (pvCali == null)
            {
                return HttpNotFound();
            }
            return View(pvCali);
        }

        public ActionResult PvCaliCreate()
        {
            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Modello");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvCaliCreate([Bind(Include = "PvCaliId,PvTankId,Value,FieldDate")] SystemWeb.Models.PvCali pvCali)
        {
            if (ModelState.IsValid)
            {
                pvCali.PvCaliId = Guid.NewGuid();
                db.PvCali.Add(pvCali);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Modello", pvCali.PvTankId);
            return View(pvCali);
        }
    
        public ActionResult PvCaliEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvCali pvCali = db.PvCali.Find(id);
            if (pvCali == null)
            {
                return HttpNotFound();
            }
            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Modello", pvCali.PvTankId);
            return View(pvCali);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvCaliEdit([Bind(Include = "PvCaliId,PvTankId,Value,FieldDate")] SystemWeb.Models.PvCali pvCali)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pvCali).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PvTankId = new SelectList(db.PvTank, "PvTankId", "Modello", pvCali.PvTankId);
            return View(pvCali);
        }
     
        public ActionResult PvCaliDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.PvCali pvCali = db.PvCali.Find(id);
            if (pvCali == null)
            {
                return HttpNotFound();
            }
            return View(pvCali);
        }

        [HttpPost, ActionName("PvCaliDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PvCaliDeleteConfirmed(Guid id)
        {
            SystemWeb.Models.PvCali pvCali = db.PvCali.Find(id);
            db.PvCali.Remove(pvCali);
            db.SaveChanges();
            return RedirectToAction("Index");
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
 
        public ActionResult Dispenser(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "Dispenser_Desc" : "";
            ViewBag.PvTankId = new SelectList(db.PvTank, "pvTankId", "Modello");
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
            IQueryable<Dispenser> dispenser = (from r in db.Dispenser
                                               select r)
                .Where(c => currentUser.pvID == c.PvTank.pvID)
                .Include(c => c.PvTank)
                .OrderBy(q => (q.Modello));
            if (!string.IsNullOrEmpty(searchString))
            {
                dispenser = dispenser.Where(s => s.Modello.ToString().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Dispenser_Desc":
                    dispenser = dispenser.OrderByDescending(s => s.Modello);
                    break;
                default:
                    dispenser = dispenser.OrderBy(s => s.Modello);
                    break;
            }
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            return View(dispenser.ToPagedList(pageNumber, pageSize));
        }
      
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
        }
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