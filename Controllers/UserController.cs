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
using SystemWeb.Database.Repository.Interface;
using SystemWeb.Database.Repository;
using System.Data;
using System.Collections;
using System.Globalization;
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript.Models;
using Syncfusion.XlsIO;
using System.Web.Script.Serialization;
using System.Reflection;
using SystemWeb.ActionFilters;
using SystemWeb.Database.Entity;

namespace SystemWeb.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        #region Inizializzatori
        private readonly MyDbContext _db = new MyDbContext();
        private readonly iCaricoRepository _caricoRepository;
        private readonly iPvRepository _pvRepository;
        private readonly iPvErogatoriRepository _pvErogatoriRepository;
        private readonly iPvCaliRepository _pvCaliRepository;
        private readonly iPvDeficienzeRepository _pvDeficienzeRepository;
        public string Ly { get; set; }

        public UserController()
        {
            _caricoRepository = new CaricoRepository(new MyDbContext());
            _pvRepository = new PvRepository(new MyDbContext());
            _pvErogatoriRepository = new PvErogatoriRepository(new MyDbContext());
            _pvCaliRepository = new PvCaliRepository(new MyDbContext());
            _pvDeficienzeRepository = new PvDeficienzeRepository(new MyDbContext());
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
        [WhitespaceFilter]
        public ActionResult Index(DateTime? dateFrom, DateTime? dateTo, DateTime? dateFrom2, DateTime? dateTo2, Guid? id)
        {
            #region List

            var list = new UserIndexViewModel
            {
                carico = _db.Carico
                    .Include(i => i.Pv)
                    .Include(i => i.Year)
                    .ToList(),
                pverogatori = _db.PvErogatori
                    .Include(i => i.Pv)
                    .Include(i => i.Dispenser)
                    .Include(i => i.Product)
                    .ToList(),
                pv = _db.Pv
                    .Include(i => i.Flag)
                    .Include(i => i.Carico)
                    .Include(i => i.PvTank)
                    .Include(i => i.ApplicationUser)
                    .ToList(),
                pvprofile = _db.PvProfile
                    .Include(i => i.Pv)
                    .ToList(),
                pvtank = _db.PvTank
                    .Include(i => i.Product)
                    .Include(i => i.Pv)
                    .Include(i => i.PvCali)
                    .Include(i => i.PvDeficienze)
                    .ToList(),
                dispenser = _db.Dispenser
                    .Include(i => i.PvErogatori)
                    .Include(i => i.PvTank)
                    .ToList(),
                company = _db.Company
                    .Include(i => i.ApplicationUser)
                    .Include(i => i.RagioneSociale)
                    .ToList(),
                companytask = _db.CompanyTask
                    .Include(i => i.ApplicationUser)
                    .ToList(),
                userarea = _db.UserArea
                    .Include(i => i.ApplicationUser)
                    .ToList(),
                applicationuser = _db.Users
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
                    .ToList(),
                usersimage = _db.UsersImage
                    .Include(i => i.UserProfiles)
                    .ToList()
            };
            #endregion

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            var getPId = from a in _db.UserProfiles
                         where a.ProfileId == currentUser.ProfileId
                         select a;
            
            ViewBag.profileId = getPId;

            var somequalsD1 = (from pvProfile in _db.PvProfile where currentUser.pvID == pvProfile.pvID select pvProfile.Indirizzo).DefaultIfEmpty("Indirizzo non configurato").SingleOrDefault();
            var somequalsD2 = (from pvProfile in _db.PvProfile where currentUser.pvID == pvProfile.pvID select pvProfile.Città).DefaultIfEmpty("Città non configurata").SingleOrDefault();
            var somequalsD3 = (from applicationUser in _db.Users where currentUser.pvID == applicationUser.Pv.pvID select applicationUser.Pv.pvName).SingleOrDefault();
            var somequalsD4 = (from applicationUser in _db.Users where currentUser.pvID == applicationUser.Pv.pvID select applicationUser.Pv.Flag.Nome).SingleOrDefault();
            var somequalsD5 = _db.Users.Include(s => s.UserProfiles).Where(s => s.Id == currentUser.Id).Select(s => s.UserProfiles.ProfileCity + ", " + s.UserProfiles.ProfileAdress).SingleOrDefault(); 
            var somequalsD6 = _db.Users.Include(s => s.UserProfiles).Where(s => s.Id == currentUser.Id).Select(s => s.UserProfiles.ProfileName + " " + s.UserProfiles.ProfileSurname).SingleOrDefault();
            var somequalsD7 = _db.Users.Include(s => s.Company).Where(s => s.Id == currentUser.Id).Select(s => s.Company.Name).DefaultIfEmpty("Azienda non configurata").SingleOrDefault();
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

            UserProfiles profile = _db.UserProfiles.Include(s => s.UsersImage).Include(s => s.ApplicationUser).SingleOrDefault(s => s.ProfileId == id);
            list.userprofiles = profile;

            #region CaricoCreate

            var thisYear = DateTime.Now.Year;

            var getall = from order in _pvRepository.GetPvs()
                         where (currentUser.pvID == order.pvID)
                         select order;

            var year = _db.Year
                .Where(c => c.Anno.Year == thisYear);

            var dep = from d in _db.Deposito
                      select d;

            var doc = from d in _db.Documento
                      select d;

            ViewBag.pvID = new SelectList(getall, "pvID", "pvName");
            ViewBag.yearId = new SelectList(year, "yearId", "Anno");
            ViewBag.depId = new SelectList(dep, "depId", "Nome");
            ViewBag.docId = new SelectList(doc, "DocumentoID", "Tipo");

            #endregion

            #region TotaleContatori

            dateFrom = new DateTime(2016, 12, 31);
            dateTo = DateTime.Now;
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();

            var getAll = (from a in _pvErogatoriRepository.GetPvErogatori()

                          where (Convert.ToDateTime(a.FieldDate) >= dateFrom)
                                   && (Convert.ToDateTime(a.FieldDate) <= dateTo
                                   && currentUser.pvID == a.pvID)
                          //where (a.FieldDate.Year.ToString().Contains(ly))
                          select a);

            if (getAll.Count() == 0)
            {
                ViewBag.SSPBTotalAmount = "0";
                ViewBag.DieselTotalAmount = "0";
                //ViewBag.HiQbTotalAmount = "0";
                //ViewBag.HiQdTotalAmount = "0";
                ViewBag.TotalAmount = "0";
                ViewBag.SSPBTotalAmount2 = "0";
                ViewBag.DieselTotalAmount2 = "0";
                //ViewBag.HiQbTotalAmount2 = "0";
                //ViewBag.HiQdTotalAmount2 = "0";
                ViewBag.TotalAmount2 = "0";
                ViewBag.TotalAmountDifference = "0";
            }

            else
            {
                var enumerable = getAll as IList<PvErogatori> ?? getAll.ToList();

                var maxB = (enumerable
                    .Where(z => (z.Product.Nome.Contains("B")))
                    .Max(row => row.Value));
                var minB = enumerable
                    .Where(z => (z.Product.Nome.Contains("B")))
                    .Min(row => row.Value);

                var maxG = enumerable
                    .Where(z => (z.Product.Nome.Contains("G")))
                    .Max(row => row.Value);
                var minG = enumerable
                    .Where(z => (z.Product.Nome.Contains("G")))
                    .Min(row => row.Value);

                /* System.InvalidOperationException: La sequenza non contiene elementi
                var maxHiqB = enumerable
                    .Where(z => (z.Product.Nome.Contains("HiQb")))
                    .Max(row => row.Value);
                var minHiqB = enumerable
                    .Where(z => (z.Product.Nome.Contains("HiQb")))
                    .Min(row => row.Value);

                var maxHiqD = enumerable
                    .Where(z => (z.Product.Nome.Contains("HiQd")))
                    .Max(row => row.Value);
                var minHiqD = enumerable
                    .Where(z => (z.Product.Nome.Contains("HiQd")))
                    .Min(row => row.Value);*/

                #region Risultati

                var _resultB = maxB - minB;
                var _resultG = maxG - minG;
                //var _resultHiqB = maxHiqB - minHiqB;
                //var _resultHiqD = maxHiqD - minHiqD;
                var _totalResult = _resultB + _resultG /*+ _resultHiqB + _resultHiqD*/;

                #endregion

                ViewBag.SSPBTotalAmount = _resultB;
                ViewBag.DieselTotalAmount = _resultG;
                //ViewBag.HiQbTotalAmount = _resultHiqB;
                //ViewBag.HiQdTotalAmount = _resultHiqD;
                ViewBag.TotalAmount = _totalResult;

                #endregion

                #region TotaleContatoriPrecedente

                dateFrom2 = new DateTime(2015, 12, 31);
                dateTo2 = DateTime.Now.AddYears(-1);

                var getAll2 = from a in _pvErogatoriRepository.GetPvErogatori()
                              where (Convert.ToDateTime(a.FieldDate) >= dateFrom2)
                                      && (Convert.ToDateTime(a.FieldDate) <= dateTo2
                                      && currentUser.pvID == a.pvID)
                              //where (a.FieldDate.Year.ToString().Contains(ly))
                              select a;

                var pvErogatoris = getAll2 as IList<PvErogatori> ?? getAll2.ToList();

                var maxB2 = pvErogatoris
                    .DefaultIfEmpty()
                    .Where(z => (z.Product.Nome.Contains("B")))
                    .Max(row => row.Value);
                var minB2 = pvErogatoris
                    .DefaultIfEmpty()
                    .Where(z => (z.Product.Nome.Contains("B")))
                    .Min(row => row.Value);

                var maxG2 = pvErogatoris
                    .DefaultIfEmpty()
                    .Where(z => (z.Product.Nome.Contains("G")))
                    .Max(row => row.Value);
                var minG2 = pvErogatoris
                    .DefaultIfEmpty()
                    .Where(z => (z.Product.Nome.Contains("G")))
                    .Min(row => row.Value);

                /* System.InvalidOperationException: La sequenza non contiene elementi
                var maxHiqB2 = pvErogatoris
                    .DefaultIfEmpty()
                    .Where(z => (z.Product.Nome.Contains("HiQb")))
                    .Max(row => row.Value);
                var minHiqB2 = pvErogatoris
                    .DefaultIfEmpty()
                    .Where(z => (z.Product.Nome.Contains("HiQb")))
                    .Min(row => row.Value);

                var maxHiqD2 = pvErogatoris
                    .DefaultIfEmpty()
                    .Where(z => (z.Product.Nome.Contains("HiQd")))
                    .Max(row => row.Value);
                var minHiqD2 = pvErogatoris
                    .DefaultIfEmpty()
                    .Where(z => (z.Product.Nome.Contains("HiQd")))
                    .Min(row => row.Value);*/

                #region Risultati

                var _resultB2 = maxB2 - minB2;
                var _resultG2 = maxG2 - minG2;
                //var _resultHiqB2 = maxHiqB2 - minHiqB2;
                //var _resultHiqD2 = maxHiqD2 - minHiqD2;
                var _totalResult2 = _resultB2 + _resultG2 /*+ _resultHiqB2 + _resultHiqD2*/;

                #endregion

                ViewBag.SSPBTotalAmount2 = _resultB2;
                ViewBag.DieselTotalAmount2 = _resultG2;
                //ViewBag.HiQbTotalAmount2 = _resultHiqB2;
                //ViewBag.HiQdTotalAmount2 = _resultHiqD2;
                ViewBag.TotalAmount2 = _totalResult2;

                #endregion


                #region Difference

                ViewBag.TotalAmountDifference = _totalResult - _totalResult2;

                #endregion
            }
            return View(list);
        }

        #endregion

        #region FilePaths

        // GET: FilePaths
        public ActionResult File()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            var myBoard = from a in _db.FilePaths.Include(f => f.ApplicationUser)
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
            FilePath filePath = _db.FilePaths.Find(id);
            if (filePath == null)
            {
                return HttpNotFound();
            }
            return View(filePath);
        }

        public ActionResult FileCreate([Bind(Include = "FilePathID,FileName,FileType,UploadDate,UserID")] FilePath filePath, ApplicationUser user, IEnumerable<HttpPostedFileBase> upload)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                foreach (var file in upload)
                {
                    if (file == null || file.ContentLength <= 0) continue;
                    var filename = Path.GetFileName(Guid.NewGuid() + "." + file.FileName);
                    var filepath = Path.Combine(Server.MapPath("~/Uploads/Documents/"), filename);
                    file.SaveAs(filepath);

                    var path = "/Uploads/Documents/" + filename;
                    var fileDocumentPath = $"{path}";

                    var document = new FilePath
                    {
                        FileName = fileDocumentPath,
                        FileType = FileType.Document,
                        UploadDate = DateTime.Now.Date,
                        UserID = currentUser.Id
                    };
                    _db.FilePaths.Add(document);
                    _db.SaveChanges();
                }
            }

            ViewBag.UserID = new SelectList(_db.Users, "Id", "UserName", filePath.UserID);

            return Content("");
        }

        // GET: FilePaths/Edit/5
        public ActionResult FileEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilePath filePath = _db.FilePaths.Find(id);
            if (filePath == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(_db.Users, "Id", "UserName", filePath.UserID);
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
                _db.Entry(filePath).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(_db.Users, "Id", "UserName", filePath.UserID);
            return View(filePath);
        }

        // GET: FilePaths/Delete/5
        public ActionResult FileDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilePath filePath = _db.FilePaths.Find(id);
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
            var filePath = _db.FilePaths.Find(id);
            if (filePath != null) _db.FilePaths.Remove(filePath);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        #endregion

        #region Carico
        [Route("user/ordini/")]
        [WhitespaceFilter]
        public ActionResult Carico(DateTime? dateFrom, DateTime? dateTo)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion
            
            var dataSource = _db.Carico.Where(c => currentUser.pvID == c.pvID && c.Year.Anno.Year.ToString().Contains(Ly)).OrderBy(o => o.Ordine).ToList();
            ViewBag.datasource = dataSource;

            IEnumerable dataSource2 = _db.Year.Where(a => a.Anno.Year.ToString().Contains(Ly)).ToList();
            ViewBag.datasource2 = dataSource2;

            IEnumerable dataSource3 = _db.Pv.Where(c => currentUser.pvID == c.pvID).ToList();
            ViewBag.datasource3 = dataSource3;

            IEnumerable dataSource4 = _db.Documento.ToList();
            ViewBag.datasource4 = dataSource4;

            IEnumerable dataSource5 = _db.Deposito.ToList();
            ViewBag.datasource5 = dataSource5;

            #region AmmountByDateFrom
            // Totale Carico Benzina secondo il parametro di ricerca specificato
            ViewBag.SSPBTotalAmountFrom = dataSource.ToList()
                .Where(o => /*currentUser.pvID == o.pvID &&*/
                Convert.ToDateTime(o.cData) >= dateFrom && Convert.ToDateTime(o.cData) <= dateTo)
                .Sum(o => (decimal?)o.Benzina);

            // Totale Carico Gasolio secondo il parametro di ricerca specificato
            ViewBag.DieselTotalAmountFrom = dataSource.ToList()
                .Where(o => /*currentUser.pvID == o.pvID &&*/ Convert.ToDateTime(o.cData) >= dateFrom && Convert.ToDateTime(o.cData) <= dateTo)
                .Sum(o => (decimal?)o.Gasolio);
            #endregion

            #region Total Ammount ViewBag
            // Totale Carico annuo benzina.
            ViewBag.SSPBTotalAmount = dataSource.Sum(o => (decimal?)o.Benzina);

            // Totale Carico annuo gasolio. 
            ViewBag.DieselTotalAmount = dataSource.Sum(o => (decimal?)o.Gasolio);
            #endregion

            return View();
        }
        
        [Route("user/ordini/update")]
        public ActionResult UpdateCarico(Carico value)
        {
            OrderRepository.Update(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/ordini/insert")]
        public ActionResult InsertCarico(Carico value)
        {
            OrderRepository.Add(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/ordini/remove")]
        public ActionResult RemoveCarico(Guid key)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion
            
            _db.Carico.Remove(_db.Carico.Single(o => o.Id == key));
            _db.SaveChanges();

            var data = _db.Carico.Where(c => currentUser.pvID == c.pvID && c.Year.Anno.Year.ToString().Contains(Ly));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("user/ordini/caricoexcell")]
        public ActionResult CaricoExcell(string gridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var exp = new ExcelExport();
            var now = Guid.NewGuid();
            IEnumerable dataSource = _db.Carico.Where(c => currentUser.pvID == c.pvID && (c.Year.Anno.Year.ToString().Contains(Ly))).OrderBy(o => o.Ordine).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = _db.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[2].DataSource = _db.Year.Where(a => a.Anno.Year.ToString().Contains(Ly)).ToList();

            exp.Export(obj, dataSource, now + " - Ordini.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/ordini/caricoword")]
        public ActionResult CaricoWord(string gridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion
            
            var now = Guid.NewGuid();
            var exp = new WordExport();

            IEnumerable dataSource = _db.Carico.Where(c => currentUser.pvID == c.pvID && (c.Year.Anno.Year.ToString().Contains(Ly))).OrderBy(o => o.Ordine).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = _db.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[2].DataSource = _db.Year.Where(a => a.Anno.Year.ToString().Contains(Ly)).ToList();

            exp.Export(obj, dataSource, now + " - Ordini.docx", false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/ordini/caricopdf")]
        public ActionResult CaricoPdf(string gridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion
            
            var now = Guid.NewGuid();
            var exp = new PdfExport();

            IEnumerable dataSource = _db.Carico.Where(c => currentUser.pvID == c.pvID && (c.Year.Anno.Year.ToString().Contains(Ly))).OrderBy(o => o.Ordine).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = _db.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[2].DataSource = _db.Year.Where(a => a.Anno.Year.ToString().Contains(Ly)).ToList();

            exp.Export(obj, dataSource, now + " - Ordini.pdf", false, false, "flat-saffron");

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
            Ly = lastYear.ToString();

            var getall = from order in _caricoRepository.GetOrders()
                         where (currentUser.pvID == order.pvID && order.Year.Anno.Year.ToString().Contains(Ly))
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

        [WhitespaceFilter]
        public ActionResult CaricoCreate()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            var thisYear = DateTime.Now.Year;

            var getall = from order in _pvRepository.GetPvs()
                         where (currentUser.pvID == order.pvID)
                         select order;

            var year = _db.Year
                .Where(c => c.Anno.Year == thisYear);

            var dep = from d in _db.Deposito
                      select d;

            var doc = from d in _db.Documento
                      select d;

            ViewBag.pvID = new SelectList(getall, "pvID", "pvName");
            ViewBag.yearId = new SelectList(year, "yearId", "Anno");
            ViewBag.depId = new SelectList(dep, "depId", "Nome");
            ViewBag.docId = new SelectList(doc, "DocumentoID", "Tipo");
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CaricoCreate(Carico insertOrder)
        {
            if (ModelState.IsValid)
            {
                _caricoRepository.InsertOrder(insertOrder);
                _caricoRepository.Save();
                return RedirectToAction("Carico");
            }

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            var thisYear = DateTime.Now.Year;

            var getall = from order in _pvRepository.GetPvs()
                         where (currentUser.pvID == order.pvID)
                         select order;

            var year = _db.Year
                .Where(c => c.Anno.Year == thisYear);

            var dep = from d in _db.Deposito
                      select d;

            var doc = from d in _db.Documento
                      select d;

            ViewBag.pvID = new SelectList(getall, "pvID", "pvName");
            ViewBag.yearId = new SelectList(year, "yearId", "Anno");
            ViewBag.depId = new SelectList(dep, "depId", "Nome");
            ViewBag.docId = new SelectList(doc, "DocumentoID", "Tipo");
            return View(insertOrder);
        }
        
        #endregion

        #region PvErogatori
        [Route("user/contatori/")]
        [WhitespaceFilter]
        public ActionResult PvErogatori(DateTime? dateFrom, DateTime? dateTo)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var dataSource = new MyDbContext().PvErogatori.Where(c => currentUser.pvID == c.pvID && (c.FieldDate.Year.ToString().Contains(Ly)) && c.Dispenser.isActive == true).OrderBy(o => o.FieldDate).ToList();
            ViewBag.datasource = dataSource;

            IEnumerable dataSource2 = new MyDbContext().Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            ViewBag.datasource2 = dataSource2;

            IEnumerable dataSource3 = new MyDbContext().Product.ToList();
            ViewBag.datasource3 = dataSource3;

            IEnumerable dataSource4 = new MyDbContext().Dispenser/*.Include(c => c.PvTank)*/.Where(a => currentUser.pvID == a.PvTank.pvID && a.isActive == true).ToList();
            ViewBag.datasource4 = dataSource4;

            #region IF dateFrom == null | dateTo == null
            if (dateFrom == null | dateTo == null)
            {
                //da = new DateTime((newDate.Year), 12, 31);
                var da = new DateTime(2016,12,31);
                var al = DateTime.Now;

                ViewBag.ShowLastYear = da.ToString(CultureInfo.CurrentCulture);
                lastYear = DateTime.Today.Year;
                Ly = lastYear.ToString();

                var getall = _pvErogatoriRepository.GetPvErogatori()
                    .Where(a => currentUser != null && (currentUser.pvID == a.pvID &&
                                                        (Convert.ToDateTime(a.FieldDate) >= da)
                                                        && (Convert.ToDateTime(a.FieldDate) <= al)));
                if (getall.Count() == 0)
                {
                    ViewBag.SSPBTotalAmount = "0";
                    ViewBag.DieselTotalAmount = "0";
                }
                else
                {
                    var pvErogatoris = getall as IList<PvErogatori> ?? getall.ToList();
                    var maxB = pvErogatoris
                        .Where(z => (z.Product.Nome.Contains("B")))
                        .Max(row => row.Value);
                    var minB = pvErogatoris
                        .Where(z => (z.Product.Nome.Contains("B")))
                        .Min(row => row.Value);

                    var maxG = pvErogatoris
                        .Where(z => (z.Product.Nome.Contains("G")))
                        .Max(row => row.Value);
                    var minG = pvErogatoris
                        .Where(z => (z.Product.Nome.Contains("G")))
                        .Min(row => row.Value);

                    ViewBag.SSPBTotalAmount = maxB - minB;
                    ViewBag.DieselTotalAmount = maxG - minG;
                }
            }

            else
            {
                ViewBag.SSPBTotalAmount = "E' stato impostato un parametro";
                ViewBag.DieselTotalAmount = "E' stato impostato un parametro";
            }
            #endregion

            #region IF dateFrom != null | dateTo |= null

            if (!(dateFrom != null | dateTo != null)) return View();
            {
                var getAllfromParam = _pvErogatoriRepository.GetPvErogatori()
                    .Where(a => currentUser != null && (currentUser.pvID == a.pvID
                                                        && (Convert.ToDateTime(a.FieldDate) >= dateFrom)
                                                        && (Convert.ToDateTime(a.FieldDate) <= dateTo)));
                if (getAllfromParam.Count() == 0)
                {
                    ViewBag.SSPBTotalAmountFrom = "0";
                    ViewBag.DieselTotalAmountFrom = "0";
                }
                else
                {
                    var enumerable = getAllfromParam as IList<PvErogatori> ?? getAllfromParam.ToList();
                    var maxB = enumerable
                        .Where(z => (z.Product.Nome.Contains("B")))
                        .Max(row => row.Value);
                    var minB = enumerable
                        .Where(z => (z.Product.Nome.Contains("B")))
                        .Min(row => row.Value);

                    var maxG = enumerable
                        .Where(z => (z.Product.Nome.Contains("G")))
                        .Max(row => row.Value);
                    var minG = enumerable
                        .Where(z => (z.Product.Nome.Contains("G")))
                        .Min(row => row.Value);

                    ViewBag.SSPBTotalAmountFrom = maxB - minB;
                    ViewBag.DieselTotalAmountFrom = maxG - minG;
                }
            }

            #endregion

            return View();
        }

        [Route("user/contatori/update")]
        public ActionResult UpdateErogatori(PvErogatori value)
        {
            ContatoriRepository.Update(value);
            
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        [Route("user/contatori/insert")]
        public ActionResult InsertErogatori(PvErogatori value)
        {
            ContatoriRepository.Add(value);
            
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/contatori/remove")]
        public ActionResult RemoveErogatori(Guid key)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var context = new MyDbContext();
            context.PvErogatori.Remove(context.PvErogatori.Single(o => o.PvErogatoriId == key));
            context.SaveChanges();

            var data = context.PvErogatori.Where(c => currentUser.pvID == c.pvID && c.FieldDate.Year.ToString().Contains(Ly));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("user/contatori/contatoriexcell")]
        public ActionResult PvErogatoriExcell(string gridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var exp = new ExcelExport();
            var context = new MyDbContext();
            var now = Guid.NewGuid();
            IEnumerable dataSource = new MyDbContext().PvErogatori.Where(c => currentUser.pvID == c.pvID && (c.FieldDate.Year.ToString().Contains(Ly))).OrderBy(o => o.FieldDate).ToList();

            GridProperties obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[2].DataSource = context.Product.ToList();
            obj.Columns[3].DataSource = context.Dispenser.Where(a => currentUser.pvID == a.PvTank.pvID).ToList();

            exp.Export(obj, dataSource, now + " - Contatori.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/contatori/contatoriword")]
        public ActionResult PvErogatoriWord(string gridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var context = new MyDbContext();
            var now = Guid.NewGuid();
            var exp = new WordExport();

            IEnumerable dataSource = new MyDbContext().PvErogatori.Where(c => currentUser.pvID == c.pvID && (c.FieldDate.Year.ToString().Contains(Ly))).OrderBy(o => o.FieldDate).ToList();

            GridProperties obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[2].DataSource = context.Product.ToList();
            obj.Columns[3].DataSource = context.Dispenser.Where(a => currentUser.pvID == a.PvTank.pvID).ToList();

            exp.Export(obj, dataSource, now + " - Contatori.docx", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/contatori/contatoripdf")]
        public ActionResult PvErogatoriPdf(string gridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var context = new MyDbContext();

            var now = Guid.NewGuid();

            var exp = new PdfExport();

            IEnumerable dataSource = new MyDbContext().PvErogatori.Where(c => currentUser.pvID == c.pvID && (c.FieldDate.Year.ToString().Contains(Ly))).OrderBy(o => o.FieldDate).ToList();

            GridProperties obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[2].DataSource = context.Product.ToList();
            obj.Columns[3].DataSource = context.Dispenser.Where(a => currentUser.pvID == a.PvTank.pvID).ToList();

            exp.Export(obj, dataSource, now + " - Contatori.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }
        
        [Route("user/contatori/nuovo")]
        public ActionResult PvErogatoriCreate()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());

            var getall = from a in _pvRepository.GetPvs()
                         where (currentUser.pvID == a.pvID)
                         select a;

            var getdisp = from a in _db.Dispenser
                         where (currentUser.pvID == a.PvTank.pvID && a.isActive == true)
                         select a;

            ViewBag.pvID = new SelectList(getall, "pvID", "pvName");
            ViewBag.DispenserId = new SelectList(getdisp, "DispenserId", "Modello");
            ViewBag.ProductId = new SelectList(_db.Product, "ProductId", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvErogatoriCreate(PvErogatori pvErogatori)
        {
            if (ModelState.IsValid)
            {
                pvErogatori.PvErogatoriId = Guid.NewGuid();
                _pvErogatoriRepository.InsertPvErogatori(pvErogatori);
                _pvErogatoriRepository.Save();
                return RedirectToAction("PvErogatori");
            }
            
            ViewBag.DispenserId = new SelectList(_db.Dispenser, "DispenserId", "Modello", pvErogatori.DispenserId);
            ViewBag.ProductId = new SelectList(_db.Product, "ProductId", "Nome", pvErogatori.ProductId);
            ViewBag.pvID = new SelectList(_db.Pv, "pvID", "pvName", pvErogatori.pvID);
            return View(pvErogatori);
        }
        
        #endregion

        #region Pv

        public ActionResult Pv()
        {
            var pv = _pvRepository.GetPvs();
            return View(pv);
        }
  
        public ActionResult PvDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pv pv = _pvRepository.GetPvsById(id);
            if (pv == null)
            {
                return HttpNotFound();
            }
            return View(pv);
        }
  
        public ActionResult PvCreate()
        {
            ViewBag.pvFlagId = new SelectList(_db.Flag, "pvFlagId", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvCreate(Pv pv)
        {
            if (ModelState.IsValid)
            {
                pv.pvID = Guid.NewGuid();
                _pvRepository.InsertPv(pv);
                _pvRepository.Save();
                return RedirectToAction("Pv");
            }

            ViewBag.pvFlagId = new SelectList(_db.Flag, "pvFlagId", "Nome", pv.pvFlagId);
            return View(pv);
        }

        public ActionResult PvEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pv pv = _pvRepository.GetPvsById(id);
            if (pv == null)
            {
                return HttpNotFound();
            }
            ViewBag.pvFlagId = new SelectList(_db.Flag, "pvFlagId", "Nome", pv.pvFlagId);
            return View(pv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvEdit(Pv pv)
        {
            if (ModelState.IsValid)
            {
                _pvRepository.UpdatePv(pv);
                _pvRepository.Save();
                return RedirectToAction("Pv");
            }
            ViewBag.pvFlagId = new SelectList(_db.Flag, "pvFlagId", "Nome", pv.pvFlagId);
            return View(pv);
        }
      
        public ActionResult PvDelete(Guid? id, bool? saveChangesError)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Unable to save changes. Try again, and if the problem persists contact your system administrator.";
            }
            Pv pv = _pvRepository.GetPvsById(id);
            return View(pv);
        }

        [HttpPost, ActionName("PvDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PvDeleteConfirmed(Guid id)
        {
            try
            {
                Pv pv = _pvRepository.GetPvsById(id);
                _pvRepository.DeletePv(id);
                _pvRepository.Save();
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
            ViewBag.pvID = new SelectList(_db.Pv, "pvID", "pvName");
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            IQueryable<PvProfile> pvProfile = _db.PvProfile
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
            PvProfile pvProfile = _db.PvProfile.Find(id);
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
            IQueryable<Pv> pv = _db.Pv
                .Where(c => currentUser.pvID == c.pvID);
            var sql = pv.ToList();
            ViewBag.pvID = new SelectList(pv, "pvID", "pvName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvProfilesCreate([Bind(Include = "PvProfileId,pvID,Indirizzo,Città,Nazione,Cap")] PvProfile pvProfile)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            var pvp = (from PvProfile in _db.PvProfile where currentUser.pvID == PvProfile.pvID select PvProfile.PvProfileId).ToString();

            if (ModelState.IsValid)
            {
                pvProfile.PvProfileId = Guid.NewGuid();
                _db.PvProfile.Add(pvProfile);
                _db.SaveChanges();
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
            PvProfile pvProfile = _db.PvProfile.Find(id);
            if (pvProfile == null)
            {
                return HttpNotFound();
            }
            ViewBag.pvID = new SelectList(_db.Pv, "pvID", "pvName", pvProfile.pvID);
            return View(pvProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvProfilesEdit([Bind(Include = "PvProfileId,pvID,Indirizzo,Città,Nazione,Cap")] PvProfile pvProfile)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(pvProfile).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("PvProfiles");
            }
            ViewBag.pvID = new SelectList(_db.Pv, "pvID", "pvName", pvProfile.pvID);
            return View(pvProfile);
        }
      
        public ActionResult PvProfilesDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PvProfile pvProfile = _db.PvProfile.Find(id);
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
            PvProfile pvProfile = _db.PvProfile.Find(id);
            _db.PvProfile.Remove(pvProfile);
            _db.SaveChanges();
            return RedirectToAction("PvProfiles");
        }
        #endregion

        #region PvTanks
        [Route("user/cisterne/")]
        [WhitespaceFilter]
        public ActionResult PvTanks()
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var dataSource = new MyDbContext().PvTank.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.Modello).ToList();
            ViewBag.datasource = dataSource;

            IEnumerable dataSource2 = new MyDbContext().Pv.Where(c => currentUser.pvID == c.pvID).ToList();
            ViewBag.datasource2 = dataSource2;

            IEnumerable dataSource3 = new MyDbContext().Product.ToList();
            ViewBag.datasource3 = dataSource3;

            return View();
        }

        [Route("user/cisterne/update")]
        public ActionResult UpdatePvTanks(PvTank value)
        {
            TankRepository.Update(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        [Route("user/cisterne/insert")]
        public ActionResult InsertPvTanks(PvTank value)
        {
            TankRepository.Add(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/cisterne/remove")]
        public ActionResult RemovePvTanks(Guid key)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            context.PvTank.Remove(context.PvTank.Single(o => o.PvTankId == key));
            context.SaveChanges();

            var data = context.PvTank.Where(c => currentUser.pvID == c.pvID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("user/cisterne/cisterneexcell")]
        public ActionResult PvTanksExcell(string gridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var exp = new ExcelExport();
            var context = new MyDbContext();
            var now = Guid.NewGuid();
            IEnumerable dataSource = context.PvTank.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.Modello).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[2].DataSource = context.Product.ToList();

            exp.Export(obj, dataSource, now + " - PvTank.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/cisterne/cisterneword")]
        public ActionResult PvTanksWord(string gridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var context = new MyDbContext();
            var now = Guid.NewGuid();
            var exp = new WordExport();

            IEnumerable dataSource = context.PvTank.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.Modello).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[2].DataSource = context.Product.ToList();

            exp.Export(obj, dataSource, now + " - PvTank.docx", false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/cisterne/cisternepdf")]
        public ActionResult PvTanksPdf(string gridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var context = new MyDbContext();

            var now = Guid.NewGuid();

            var exp = new PdfExport();

            IEnumerable dataSource = context.PvTank.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.Modello).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();
            obj.Columns[2].DataSource = context.Product.ToList();

            exp.Export(obj, dataSource, now + " - PvTank.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PvTanksDesc
    
        public ActionResult PvTanksDesc()
        {
            var pvTankDescs = _db.PvTankDesc.Include(p => p.PvTank);
            return View(pvTankDescs.ToList());
        }
 
        public ActionResult PvTanksDescDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PvTankDesc pvTankDesc = _db.PvTankDesc.Find(id);
            if (pvTankDesc == null)
            {
                return HttpNotFound();
            }
            return View(pvTankDesc);
        }
    
        public ActionResult PvTanksDescCreate()
        {
            ViewBag.PvTankId = new SelectList(_db.PvTank, "PvTankId", "Modello");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        internal ActionResult PvTanksDescCreate([Bind(Include = "PvTankDescId,PvTankId,PvTankCM,PvTankLT")] PvTankDesc pvTankDesc)
        {
            if (ModelState.IsValid)
            {
                pvTankDesc.PvTankDescId = Guid.NewGuid();
                _db.PvTankDesc.Add(pvTankDesc);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PvTankId = new SelectList(_db.PvTank, "PvTankId", "Modello", pvTankDesc.PvTankId);
            return View(pvTankDesc);
        }
     
        public ActionResult PvTanksDescEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PvTankDesc pvTankDesc = _db.PvTankDesc.Find(id);
            if (pvTankDesc == null)
            {
                return HttpNotFound();
            }
            ViewBag.PvTankId = new SelectList(_db.PvTank, "PvTankId", "Modello", pvTankDesc.PvTankId);
            return View(pvTankDesc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PvTanksDescEdit([Bind(Include = "PvTankDescId,PvTankId,PvTankCM,PvTankLT")] PvTankDesc pvTankDesc)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(pvTankDesc).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PvTankId = new SelectList(_db.PvTank, "PvTankId", "Modello", pvTankDesc.PvTankId);
            return View(pvTankDesc);
        }
     
        public ActionResult PvTanksDescDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PvTankDesc pvTankDesc = _db.PvTankDesc.Find(id);
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
            PvTankDesc pvTankDesc = _db.PvTankDesc.Find(id);
            _db.PvTankDesc.Remove(pvTankDesc);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion

        #region PvDeficienze
        [Route("user/deficienze/")]
        [WhitespaceFilter]
        public ActionResult PvDeficienze(DateTime? dateFrom, DateTime? dateTo)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            IEnumerable dataSource = new MyDbContext().PvDeficienze.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(Ly))).OrderBy(o => o.FieldDate).ToList();
            ViewBag.datasource = dataSource;

            IEnumerable dataSource2 = new MyDbContext().PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            ViewBag.dataSource2 = dataSource2;

            if (dateFrom == null | dateTo == null)
            {
                dateFrom = new DateTime(2016, 12, 31);
                dateTo = DateTime.Now;

                var getAll = _pvDeficienzeRepository.GetRecords()
                    .ToList()
                    .Where(a => Convert.ToDateTime(a.FieldDate) >= dateFrom
                                && (Convert.ToDateTime(a.FieldDate) <= dateTo));

                var enumerable = getAll as IList<PvDeficienze> ?? getAll.ToList();
                var sumG = enumerable
                    .Where(z => currentUser.pvID == z.PvTank.pvID
                    && z.PvTank.Modello.Contains("MC-10993"))
                    .Sum(row => row.Value);
                
                var sumB = enumerable
                    .Where(z => currentUser.pvID == z.PvTank.pvID
                    && z.PvTank.Modello.Contains("MC-10688"))
                    .Sum(row => row.Value);


                ViewBag.PvDeficienzeSumG = sumG;
                ViewBag.PvDeficienzeSumB = sumB;
            }

            {
                var getAll = _pvDeficienzeRepository.GetRecords()
                    .ToList()
                    .Where(a => currentUser != null && (currentUser.pvID == a.PvTank.pvID
                                                        && (Convert.ToDateTime(a.FieldDate) >= dateFrom)
                                                        && (Convert.ToDateTime(a.FieldDate) <= dateTo)));

                var enumerable = getAll as IList<PvDeficienze> ?? getAll.ToList();
                var sumG = enumerable
                    .Where(z => z.PvTank.Modello.Contains("MC-10993"))
                    .Sum(row => row.Value);

                var sumB = enumerable
                    .Where(z => z.PvTank.Modello.Contains("MC-10688"))
                    .Sum(row => row.Value);

                ViewBag.PvDeficienzeSumGByDate = sumG;
                ViewBag.PvDeficienzeSumBByDate = sumB;
            }

            return View();
        }


        [Route("user/deficienze/update")]
        public ActionResult UpdateDeficienze(PvDeficienze value)
        {
            DeficienzeRepository.Update(value);
            
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/deficienze/insert")]
        public ActionResult InsertDeficienze(PvDeficienze value)
        {
            DeficienzeRepository.Add(value);
            
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/deficienze/remove")]
        public ActionResult RemoveDeficienze(Guid key)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var context = new MyDbContext();
            context.PvDeficienze.Remove(context.PvDeficienze.Single(o => o.PvDefId == key));
            context.SaveChanges();

            var data = context.PvDeficienze.Where(c => currentUser.pvID == c.PvTank.pvID && c.FieldDate.ToString(CultureInfo.CurrentCulture).Contains(Ly));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("user/deficienze/deficienzeexcell")]
        public ActionResult DeficienzeExcell(string gridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var exp = new ExcelExport();
            var context = new MyDbContext();
            var now = Guid.NewGuid();
            IEnumerable dataSource = context.PvDeficienze.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString(CultureInfo.CurrentCulture).Contains(Ly))).OrderBy(o => o.FieldDate).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, dataSource, now + " - Deficienze.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/deficienze/deficienzeword")]
        public ActionResult DeficienzeWord(string gridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var context = new MyDbContext();
            var now = Guid.NewGuid();
            var exp = new WordExport();

            IEnumerable dataSource = context.PvDeficienze.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString(CultureInfo.CurrentCulture).Contains(Ly))).OrderBy(o => o.FieldDate).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, dataSource, now + " - Deficienze.docx", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/deficienze/deficienzepdf")]
        public ActionResult DeficienzePdf(string gridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var context = new MyDbContext();

            var now = Guid.NewGuid();

            var exp = new PdfExport();

            IEnumerable dataSource = context.PvDeficienze.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(Ly))).OrderBy(o => o.FieldDate).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, dataSource, now + " - Deficienze.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        [WhitespaceFilter]
        [Route("user/deficienze/aggiungi")]
        public ActionResult PvDeficienzeCreate()
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            #endregion

            var _getPvTank = from a in _db.PvTank
                             where currentUser.pvID == a.pvID
                             select a;

            ViewBag.PvTankId = new SelectList(_getPvTank, "PvTankId", "Modello");
            return View();
        }

        [HttpPost]
        [Route("user/deficienze/aggiungi")]
        [ValidateAntiForgeryToken]
        public ActionResult PvDeficienzeCreate([Bind(Include = "PvDefId,PvTankId,Value,FieldDate")] PvDeficienze pvDeficienze)
        {
            if (ModelState.IsValid)
            {
                pvDeficienze.PvDefId = Guid.NewGuid();
                _db.PvDeficienze.Add(pvDeficienze);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PvTankId = new SelectList(_db.PvTank, "PvTankId", "Modello", pvDeficienze.PvTankId);
            return View(pvDeficienze);
        }
        
        #endregion

        #region PvCali
        [Route("user/cali/")]
        [WhitespaceFilter]
        public ActionResult PvCali(DateTime? dateFrom, DateTime? dateTo)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            
            IEnumerable DataSource = new MyDbContext().PvCali.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(Ly))).OrderBy(o => o.FieldDate).ToList();
            ViewBag.datasource = DataSource;

            IEnumerable DataSource2 = new MyDbContext().PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            ViewBag.dataSource2 = DataSource2;

            #region DateFrom == Null
            if (dateFrom == null | dateTo == null)
            {
                dateFrom = new DateTime(2016, 12, 31);
                dateTo = DateTime.Now;

                var getAll = from a in _pvCaliRepository.GetRecords()
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
                var getAll = from a in _pvCaliRepository.GetRecords()
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
            Ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            CaliRepository.Update(value);
            var data = context.PvCali.Include(i=>i.PvTank).Where(c => currentUser.pvID == c.PvTank.pvID && c.FieldDate.ToString().Contains(Ly));
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/cali/insert")]
        public ActionResult InsertCalo(PvCali value)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            CaliRepository.Add(value);
            var data = context.PvCali.Include(i => i.PvTank).Where(c => currentUser.pvID == c.PvTank.pvID && c.FieldDate.ToString().Contains(Ly));
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/cali/remove")]
        public ActionResult RemoveCalo(Guid key)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            context.PvCali.Remove(context.PvCali.Single(o => o.PvCaliId == key));
            context.SaveChanges();

            var data = context.PvCali.Where(c => currentUser.pvID == c.PvTank.pvID && c.FieldDate.ToString().Contains(Ly));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("user/cali/caliexcell")]
        public ActionResult CaliExcell(string GridModel)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            ExcelExport exp = new ExcelExport();
            MyDbContext context = new MyDbContext();
            Guid now = Guid.NewGuid();
            IEnumerable DataSource = context.PvCali.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(Ly))).OrderBy(o => o.FieldDate).ToList();

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
            Ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            Guid now = Guid.NewGuid();
            WordExport exp = new WordExport();

            IEnumerable DataSource = context.PvCali.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(Ly))).OrderBy(o => o.FieldDate).ToList();

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
            Ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();

            Guid now = Guid.NewGuid();

            PdfExport exp = new PdfExport();

            IEnumerable DataSource = context.PvCali.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(Ly))).OrderBy(o => o.FieldDate).ToList();

            GridProperties obj = ConvertGridObject(GridModel);

            obj.Columns[1].DataSource = context.PvTank.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, DataSource, now.ToString() + " - Cali.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        [WhitespaceFilter]
        [Route("user/cali/nuovo")]
        public ActionResult PvCaliCreate()
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            #endregion

            var _getPvTank = from a in _db.PvTank
                             where currentUser.pvID == a.pvID
                             select a;

            ViewBag.PvTankId = new SelectList(_getPvTank, "PvTankId", "Modello");
            return View();
        }

        [HttpPost]
        [Route("user/cali/nuovo")]
        [ValidateAntiForgeryToken]
        public ActionResult PvCaliCreate(PvCali pvCali)
        {
            if (ModelState.IsValid)
            {
                _pvCaliRepository.InsertRecords(pvCali);
                _pvCaliRepository.Save();

                return RedirectToAction("Index", "User");
            }

            ViewBag.PvTankId = new SelectList(_db.PvTank, "PvTankId", "Modello", pvCali.PvTankId);
            return View(pvCali);
        }

        #endregion 

        #region Manage
  
        public async Task<ActionResult> Manage(ManageMessageId? message, Pv pv)
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var somequals = (from Pv in _db.Pv where currentUser.pvID == Pv.pvID select Pv.pvName).SingleOrDefault();
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
        [Route("user/dispenser/")]
        [WhitespaceFilter]
        public ActionResult Dispenser()
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
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
            Ly = lastYear.ToString();
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
            Ly = lastYear.ToString();
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
            Ly = lastYear.ToString();
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
            Ly = lastYear.ToString();
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
            Ly = lastYear.ToString();
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
            Ly = lastYear.ToString();
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
        #endregion

        #region Companies
     
        public ActionResult Companies()
        {
            var company = _db.Company.Include(c => c.RagioneSociale);
            return View(company.ToList());
        }
       
        public ActionResult CompaniesDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = _db.Company.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }
     
        public ActionResult CompaniesCreate()
        {
            ViewBag.RagioneSocialeId = new SelectList(_db.RagioneSociale, "RagioneSocialeId", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompaniesCreate([Bind(Include = "CompanyId,Name,PartitaIva,RagioneSocialeId")] Company company)
        {
            if (ModelState.IsValid)
            {
                company.CompanyId = Guid.NewGuid();
                _db.Company.Add(company);
                _db.SaveChanges();
                return RedirectToAction("Companies");
            }

            ViewBag.RagioneSocialeId = new SelectList(_db.RagioneSociale, "RagioneSocialeId", "Nome", company.RagioneSocialeId);
            return View(company);
        }

        public ActionResult CompaniesEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = _db.Company.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            ViewBag.RagioneSocialeId = new SelectList(_db.RagioneSociale, "RagioneSocialeId", "Nome", company.RagioneSocialeId);
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompaniesEdit([Bind(Include = "CompanyId,Name,PartitaIva,RagioneSocialeId")] Company company)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(company).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Companies");
            }
            ViewBag.RagioneSocialeId = new SelectList(_db.RagioneSociale, "RagioneSocialeId", "Nome", company.RagioneSocialeId);
            return View(company);
        }
 
        public ActionResult CompaniesDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = _db.Company.Find(id);
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
            Company company = _db.Company.Find(id);
            _db.Company.Remove(company);
            _db.SaveChanges();
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
            IQueryable<CompanyTask> companyTask = (from r in _db.CompanyTask
                                                   select r)
                .Include(c => c.ApplicationUser)
                .OrderBy(q => (q.FieldDate));
            if (!string.IsNullOrEmpty(searchString))
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
            CompanyTask companyTask = _db.CompanyTask.Find(id);
            if (companyTask == null)
            {
                return HttpNotFound();
            }
            return View(companyTask);
        }
 
        public ActionResult TaskCreate()
        {
            ViewBag.UsersId = new SelectList(_db.Users, "Id", "UserName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaskCreate([Bind(Include = "CompanyTaskId,UsersId,FieldChiusura,FieldDate,FieldResult")] CompanyTask companyTask)
        {
            if (ModelState.IsValid)
            {
                companyTask.CompanyTaskId = Guid.NewGuid();
                _db.CompanyTask.Add(companyTask);
                _db.SaveChanges();
                return RedirectToAction("Task");
            }

            ViewBag.UsersId = new SelectList(_db.Users, "Id", "UserName", companyTask.UsersId);
            return View(companyTask);
        }
     
        public ActionResult TaskEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyTask companyTask = _db.CompanyTask.Find(id);
            if (companyTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.UsersId = new SelectList(_db.Users, "Id", "UserName", companyTask.UsersId);
            return View(companyTask);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaskEdit([Bind(Include = "CompanyTaskId,UsersId,FieldChiusura,FieldDate,FieldResult")] CompanyTask companyTask)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(companyTask).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Task");
            }
            ViewBag.UsersId = new SelectList(_db.Users, "Id", "UserName", companyTask.UsersId);
            return View(companyTask);
        }

        public ActionResult TaskDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyTask companyTask = _db.CompanyTask.Find(id);
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
            CompanyTask companyTask = _db.CompanyTask.Find(id);
            _db.CompanyTask.Remove(companyTask);
            _db.SaveChanges();
            return RedirectToAction("Task");
        }
        #endregion

        #region Area

        [Authorize(Roles = "Administrator,User")]
        public ActionResult Area()
        {
            var userArea = _db.UserArea.Include(u => u.ApplicationUser);
            return View(userArea.ToList());
        }

        [Authorize(Roles = "Administrator,User")]
        public ActionResult AreaDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserArea userArea = _db.UserArea.Find(id);
            if (userArea == null)
            {
                return HttpNotFound();
            }
            return View(userArea);
        }

        [Authorize(Roles = "Administrator,User")]
        public ActionResult AreaCreate()
        {
            ViewBag.UsersId = new SelectList(_db.Users, "Id", "UserName");
            return View();
        }

        [Authorize(Roles = "Administrator,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AreaCreate([Bind(Include = "UserAreaId,UsersId,UserFieldAccount,UserFieldUsername,UserFieldPassword,CreateDate")] UserArea userArea)
        {
            if (ModelState.IsValid)
            {
                userArea.UserAreaId = Guid.NewGuid();
                userArea.CreateDate = DateTime.Now;
                userArea.UsersId = User.Identity.GetUserId();
                _db.UserArea.Add(userArea);
                _db.SaveChanges();
                return RedirectToAction("Area");
            }

            ViewBag.UsersId = new SelectList(_db.Users, "Id", "UserName", userArea.UsersId);
            return View(userArea);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult AreaEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserArea userArea = _db.UserArea.Find(id);
            if (userArea == null)
            {
                return HttpNotFound();
            }
            ViewBag.UsersId = new SelectList(_db.Users, "Id", "UserName", userArea.UsersId);
            return View(userArea);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AreaEdit([Bind(Include = "UserAreaId,UsersId,UserFieldAccount,UserFieldUsername,UserFieldPassword,CreateDate")] UserArea userArea)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(userArea).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Area");
            }
            ViewBag.UsersId = new SelectList(_db.Users, "Id", "UserName", userArea.UsersId);
            return View(userArea);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult AreaDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserArea userArea = _db.UserArea.Find(id);
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
            UserArea userArea = _db.UserArea.Find(id);
            _db.UserArea.Remove(userArea);
            _db.SaveChanges();
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

            UserProfiles profile = _db.UserProfiles.Include(s => s.UsersImage).SingleOrDefault(s => s.ProfileId == id);

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
                UserProfiles profile = _db.UserProfiles.Include(s => s.UsersImage).SingleOrDefault(s => s.ProfileId == id);
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
                var profileToUpdate = _db.UserProfiles.Find(id);
                if (TryUpdateModel(profileToUpdate, "",
                    new string[] { "ProfileName", "ProfileSurname", "ProfileAdress", "ProfileCity", "ProfileZipCode", "ProfileNation", "ProfileInfo" }))
                {
                    try
                    {
                        if (upload != null && upload.ContentLength > 0)
                        {
                            if (profileToUpdate.UsersImage.Any(f => f.FileType == FileType.Avatar))
                            {
                                _db.UsersImage.Remove(profileToUpdate.UsersImage.First(f => f.FileType == FileType.Avatar));
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
                        _db.Entry(profileToUpdate).State = EntityState.Modified;
                        _db.SaveChanges();

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
                UserProfiles profile = _db.UserProfiles.Find(id);
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
                UserProfiles profile = _db.UserProfiles.Find(id);
                _db.UserProfiles.Remove(profile);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

        #endregion

        #region Chiusura

        public ActionResult Chiusura()
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            var da = new DateTime(2016, 12, 31);
            var al = DateTime.Now;
            #endregion

            var _carico = from a in _db.Carico
                          where currentUser.pvID == a.pvID && a.Year.Anno.ToString().Contains(Ly)
                          select a;

            var _contatori = _pvErogatoriRepository.GetPvErogatori()
               .Where(a => currentUser != null && (currentUser.pvID == a.pvID &&
                                                   (Convert.ToDateTime(a.FieldDate) >= da)
                                                   && (Convert.ToDateTime(a.FieldDate) <= al)));

            var _deficienze = _pvDeficienzeRepository.GetRecords()
                    .ToList()
                    .Where(a => Convert.ToDateTime(a.FieldDate) >= da
                                && (Convert.ToDateTime(a.FieldDate) <= al));

            var _cali = _pvCaliRepository.GetRecords()
                         .ToList()
                         .Where(a => Convert.ToDateTime(a.FieldDate) >= da
                                && (Convert.ToDateTime(a.FieldDate) <= al));

            var _cisternaRimB = from a in _db.PvTank
                                where currentUser.pvID == a.pvID && a.Product.Nome.Contains("B")
                                select a.Rimanenza;

            var _cisternaRimG = from a in _db.PvTank
                                where currentUser.pvID == a.pvID && a.Product.Nome.Contains("G")
                                select a.Rimanenza;

            var _ordiniEccedenze = (from a in _db.Carico
                                    where currentUser.pvID == a.pvID && a.Year.Anno.ToString().Contains(Ly) && a.Tipo.Tipo.Contains("T")
                                    select a);

            var _ordiniScatti = (from a in _db.Carico
                                 where currentUser.pvID == a.pvID && a.Year.Anno.ToString().Contains(Ly) && a.Tipo.Tipo.Contains("R")
                                 select a);

            var _rimIB = _cisternaRimB.Sum();
            var _rimIG = _cisternaRimG.Sum();

            if (_ordiniEccedenze == null)
            {
                ViewBag.eccB = 0;
                ViewBag.eccG = 0;
            }

            else
            {
                var _eccB = _ordiniEccedenze.Sum(s => s.Benzina);
                var _eccG = _ordiniEccedenze.Sum(s => s.Gasolio);

                ViewBag.eccB = _eccB;
                ViewBag.eccG = _eccG;
            }

            if (_ordiniScatti == null)
            {
                ViewBag.scatB = 0;
                ViewBag.scatG = 0;
            }

            else
            {
                var _scatB = /*_ordiniScatti.Sum(s => s.Benzina);*/ 0;
                var _scatG = /*_ordiniScatti.Sum(s => s.Gasolio);*/ 0;
                
                ViewBag.scatB = _scatB;
                ViewBag.scatG = _scatG;
            }

            #region Carico
            var _totCaricoB = _carico.Sum(s => s.Benzina);
            var _totCaricoG = _carico.Sum(s => s.Gasolio);
            var TotCaricoB = _rimIB + _totCaricoB;
            var TotCaricoG = _rimIG + _totCaricoG;
            var rCaricoB = _totCaricoB - _rimIB;
            var rCaricoG = _totCaricoG - _rimIG;
            ViewBag.RimIB = _rimIB;
            ViewBag.RimIG = _rimIG;
            ViewBag.rCaricoB = rCaricoB;
            ViewBag.rCaricoG = rCaricoG;
            ViewBag.TotCaricoB = TotCaricoB;
            ViewBag.TotCaricoG = TotCaricoG;
            #endregion

            #region Scarico

            #region contatori
            var pvErogatoris = _contatori as IList<PvErogatori> ?? _contatori.ToList();
            var maxB = pvErogatoris
                .Where(z => (z.Product.Nome.Contains("B")))
                .Max(row => row.Value);
            var minB = pvErogatoris
                .Where(z => (z.Product.Nome.Contains("B")))
                .Min(row => row.Value);

            var maxG = pvErogatoris
                .Where(z => (z.Product.Nome.Contains("G")))
                .Max(row => row.Value);
            var minG = pvErogatoris
                .Where(z => (z.Product.Nome.Contains("G")))
                .Min(row => row.Value);

            var totalB = maxB - minB;
            var totalG = maxG - minG;
            ViewBag.totalB = totalB;
            ViewBag.totalG = totalG;

            #endregion
            
            #region deficienze
            var enumerable = _deficienze as IList<PvDeficienze> ?? _deficienze.ToList();
            var sumdG = enumerable
                .Where(z => currentUser.pvID == z.PvTank.pvID
                && z.PvTank.Modello.Contains("MC-10993"))
                .Sum(row => row.Value);

            var sumdB = enumerable
                .Where(z => currentUser.pvID == z.PvTank.pvID
                && z.PvTank.Modello.Contains("MC-10688"))
                .Sum(row => row.Value);

            ViewBag.sumdB = sumdB;
            ViewBag.sumdG = sumdG;

            #endregion

            #region cali
            var pvcali = _cali as IList<PvCali> ?? _cali.ToList();
            var SumcG = pvcali
                .Where(z => currentUser.pvID == z.PvTank.pvID
                && z.PvTank.Modello.Contains("MC-10993"))
                .Sum(row => row.Value);

            var SumcB = pvcali
                .Where(z => currentUser.pvID == z.PvTank.pvID
                && z.PvTank.Modello.Contains("MC-10688"))
                .Sum(row => row.Value);

            ViewBag.SumcB = SumcB;
            ViewBag.SumcG = SumcG;

            #endregion

            #endregion

            var totScaricoB = totalB + sumdB + SumcB;
            var totScaricoG = totalG + sumdG + SumcG;

            #region Cisterne 

            var _cisternaB = from a in _db.PvTank
                             where currentUser.pvID == a.pvID && a.Product.Nome.Contains("B")
                             select a.Giacenza;

            var rimEfB = _cisternaB.Sum();

            var _cisternaG = from a in _db.PvTank
                             where currentUser.pvID == a.pvID && a.Product.Nome.Contains("G")
                             select a.Giacenza;

            var rimEfG = _cisternaG.Sum();
            #endregion

            ViewBag.TotScaricoB = totScaricoB;
            ViewBag.TotScaricoG = totScaricoG;

            var rimContB = TotCaricoB - totScaricoB;
            var rimContG = TotCaricoG - totScaricoG;

            ViewBag.RiportoB = rimContB;
            ViewBag.RiportoG = rimContG;
            ViewBag.RiportoEB = rimEfB;
            ViewBag.RiportoEG = rimEfG;
            ViewBag.DifEB = rimEfB - rimContB;
            ViewBag.DifEG = rimEfG - rimContG;
            ViewBag.pvId = new SelectList(_db.Pv, "pvID", "pvName");
            return View();
        }

        #endregion

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

        protected override void Dispose(bool disposing)
        {
            var db = new MyDbContext();
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);

        }
    }
}