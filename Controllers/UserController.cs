﻿using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestioniDirette.Models;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using GestioniDirette.Database.Repository.Interface;
using GestioniDirette.Database.Repository;
using System.Data;
using System.Collections;
using System.Globalization;
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript.Models;
using Syncfusion.XlsIO;
using System.Web.Script.Serialization;
using System.Reflection;
using GestioniDirette.ActionFilters;
using GestioniDirette.Database.Entity;
using System.Data.Entity;
using GestioniDirette.Database.Operation;
using GestioniDirette.Database.Operation.Interface;

namespace GestioniDirette.Controllers
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
        private readonly iOperation _operation;
        public string Ly { get; set; }

        public UserController()
        {
            _caricoRepository = new CaricoRepository(new MyDbContext());
            _pvRepository = new PvRepository(new MyDbContext());
            _pvErogatoriRepository = new PvErogatoriRepository(new MyDbContext());
            _pvCaliRepository = new PvCaliRepository(new MyDbContext());
            _pvDeficienzeRepository = new PvDeficienzeRepository(new MyDbContext());
            _operation = new Operation(new MyDbContext());
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

            var list = new UserIndexViewModel();

            #endregion

            #region GetOperation
            var getCarico = _operation.GetCarico();
            var getErogatori = _operation.GetErogatori();
            var getErogatoriLastYear = _operation.GetErogatoriLastYear();
            #endregion

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            

            list.usersimage = _db.UsersImage
                .Include(i => i.UserProfiles)
                .Where(s => currentUser.ProfileId == s.ProfileID)
                .ToList();

            var somequalsD1 = (from pvProfile in _db.PvProfile where currentUser.pvID == pvProfile.pvID select pvProfile.Indirizzo).DefaultIfEmpty("Indirizzo non configurato").SingleOrDefault();
            var somequalsD2 = (from pvProfile in _db.PvProfile where currentUser.pvID == pvProfile.pvID select pvProfile.Città).DefaultIfEmpty("Città non configurata").SingleOrDefault();
            var somequalsD3 = (from applicationUser in _db.Users where currentUser.pvID == applicationUser.Pv.pvID select applicationUser.Pv.pvName).SingleOrDefault();
            var somequalsD4 = (from applicationUser in _db.Users where currentUser.pvID == applicationUser.Pv.pvID select applicationUser.Pv.Flag.Nome).SingleOrDefault();
            var somequalsD5 = _db.Users.Include(s => s.UserProfiles).Where(s => s.Id == currentUser.Id).Select(s => s.UserProfiles.ProfileCity + ", " + s.UserProfiles.ProfileAdress).SingleOrDefault(); 
            var somequalsD6 = _db.Users.Include(s => s.UserProfiles).Where(s => s.Id == currentUser.Id).Select(s => s.UserProfiles.ProfileName + " " + s.UserProfiles.ProfileSurname).SingleOrDefault();
            var somequalsD7 = _db.Users.Include(s => s.Company).Where(s => s.Id == currentUser.Id).Select(s => s.Company.Name).DefaultIfEmpty("Azienda non configurata").SingleOrDefault();
            var somequalsD8 = _db.Licenza.Where(c => currentUser.pvID == c.pvID);
            var somequalsD9 = _db.Dispenser.Where(c => currentUser.pvID == c.PvTank.pvID);
            
            // SSPB 
            var somequalsD10 = _operation.GetTank().Where(w => w.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")).Sum(s => s.Capienza);
            var somequalsD11 = _operation.GetTank().Where(w => w.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")).Sum(s => s.Giacenza);
            
            // DSL
            var somequalsD13 = _operation.GetTank().Where(w => w.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")).Sum(s => s.Capienza);
            var somequalsD14 = _operation.GetTank().Where(w => w.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")).Sum(s => s.Giacenza);

            ViewBag.FullAdress = somequalsD5;
            ViewBag.ProfileFullName = somequalsD6;
            ViewBag.Flag = somequalsD4;
            ViewBag.PvNamee = somequalsD3;
            ViewBag.PvInd = somequalsD1;
            ViewBag.PvCity = somequalsD2;
            ViewBag.CompanyId = somequalsD7;
            ViewBag.Licenza = somequalsD8.Select(c => c.Codice).DefaultIfEmpty("Nessuna licenza inserita").Single();
            ViewBag.Scadenza = somequalsD8.Select(c => c.Scadenza).SingleOrDefault();
            ViewBag.CapienzaSSPB = somequalsD10;
            ViewBag.GiacenzaSSPB = somequalsD11;
            ViewBag.VuotoSSPB = somequalsD10 - somequalsD11;
            ViewBag.CapienzaDSL = somequalsD13;
            ViewBag.GiacenzaDSL = somequalsD14;
            ViewBag.VuotoDSL = somequalsD13 - somequalsD14;
            //ViewBag.Bollino = somequalsD9.Select(c => c.Scadenza).SingleOrDefault();
            list.sspbGauge = _operation.DoSSPBTankPercentageById();
            list.dslGauge = _operation.DoDSLTankPercentageById();

            IEnumerable<UserProfiles> userprofiles = _db.UserProfiles
                .Where(w => w.ProfileId == currentUser.ProfileId);

            list.userprofiles = userprofiles;

            var notice = _db.Notice.AsEnumerable().Take(5);
            list.notice = notice;

            #region TotaleContatori
            
            if (getErogatori.Count() == 0)
            {
                #region ViewBag
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
                #endregion
            }

            else
            {
                var nPvDispenser = _db.Dispenser
                    .Where(a => a.PvTank.pvID == currentUser.pvID)
                    .Select(s => s.DispenserId);

                if (nPvDispenser.Count() == 6)
                {
                    #region Variabili
                    var totalB1 = _operation.DoSSPBOperationShort();
                    var totalG1 = _operation.DoDSLOperationShort();
                    var _totalResult1 = totalB1 + totalG1;
                    #endregion

                    #region ViewBag
                    ViewBag.SSPBTotalAmount = totalB1;
                    ViewBag.DieselTotalAmount = totalG1;
                    ViewBag.TotalAmount = _totalResult1;
                    #endregion
                }

                else
                {
                    #region Variabili
                    var totalB = _operation.DoSSPBOperation();
                    var totalG = _operation.DoDSLOperation();
                    var _totalResult = totalB + totalG;
                    #endregion

                    #region ViewBag
                    ViewBag.SSPBTotalAmount = totalB;
                    ViewBag.DieselTotalAmount = totalG;
                    ViewBag.TotalAmount = _totalResult;
                    #endregion

                    #endregion

                    #region TotaleContatoriPrecedente
                    var createdDate = from a in _db.Users where currentUser.Id == a.Id select a.CreateDate.Year;
                    var thisID = "7c949a00-01bd-4057-a156-b6b33a4142ef";
                    var exID = Guid.Parse(thisID);

                    if (currentUser.CreateDate.Year == DateTime.Today.Year)
                    {
                        #region ViewBag
                        ViewBag.SSPBTotalAmount2 = 0;
                        ViewBag.DieselTotalAmount2 = 0;
                        //ViewBag.HiQbTotalAmount2 = 0;
                        //ViewBag.HiQdTotalAmount2 = 0;
                        ViewBag.TotalAmount2 = 0;
                        #endregion
                    }
                    else
                    {
                        // E' necessario aggiornare lastYearShort in operation
                        if (currentUser.Id.Contains("9199e89c-59a0-4d88-83b6-6f73f548dc87"))
                        {
                            #region Variabili
                            var totalB3 = _operation.DoSSPBOperationForLastYearShort();
                            var totalG3 = _operation.DoDSLOperationForLastYearShort();
                            var _totalResult3 = totalB3 + totalG3;
                            #endregion

                            #region ViewBag
                            ViewBag.SSPBTotalAmount2 = totalB3;
                            ViewBag.DieselTotalAmount2 = totalG3;
                            ViewBag.TotalAmount2 = _totalResult3;
                            #endregion

                            #region Difference

                            ViewBag.TotalAmountDifference = _totalResult - _totalResult3;

                            #endregion
                        }

                        #region Variabili

                        var totalB2 = _operation.DoSSPBOperationForLastYear();
                        var totalG2 = _operation.DoDSLOperationForLastYear();
                        var _resultB2 = totalB2;
                        var _resultG2 = totalG2;
                        var _totalResult2 = _resultB2 + _resultG2;

                        #endregion

                        #region ViewBag
                        ViewBag.SSPBTotalAmount2 = _resultB2;
                        ViewBag.DieselTotalAmount2 = _resultG2;
                        ViewBag.TotalAmount2 = _totalResult2;
                        #endregion

                        #endregion

                        #region Difference

                        ViewBag.TotalAmountDifference = _totalResult - _totalResult2;

                        

                    }
                    #endregion
                }
            }

            return View(list);
        }

        #endregion

        #region Reclami

        [Route("user/reclami/")]
        [WhitespaceFilter]
        public ActionResult Reclami()
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var dataSource = new MyDbContext().Reclami.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.DataCreazione).ToList();
            ViewBag.datasource = dataSource;

            IEnumerable dataSource2 = new MyDbContext().Pv.Where(c => currentUser.pvID == c.pvID).ToList();
            ViewBag.datasource2 = dataSource2;

            IEnumerable dataSource3 = new Liste().Reclamo;
            ViewBag.datasource3 = dataSource3;

            IEnumerable dataSource4 = new Liste().Documento;
            ViewBag.datasource4 = dataSource4;

            return View();
        }

        [Route("user/reclami/update")]
        public ActionResult UpdateReclami(Reclami value)
        {
            ReclamiRepository.Update(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        [Route("user/reclami/insert")]
        public ActionResult InsertReclami(Reclami value)
        {
            ReclamiRepository.Add(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/reclami/remove")]
        public ActionResult RemoveReclami(Guid key)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            context.Reclami.Remove(context.Reclami.Single(o => o.ReclamiID == key));
            context.SaveChanges();

            var data = context.Reclami.Where(c => currentUser.pvID == c.pvID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("user/reclami/reclamiexcell")]
        public ActionResult ReclamiExcell(string gridModel)
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

            IEnumerable dataSource = context.Reclami.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.DataCreazione).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();

            obj.Columns[2].DataSource = new Liste().Reclamo;

            obj.Columns[4].DataSource = new Liste().Documento;

            exp.Export(obj, dataSource, now + " - Reclami.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/reclami/reclamiword")]
        public ActionResult ReclamiWord(string gridModel)
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

            IEnumerable dataSource = context.Reclami.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.DataCreazione).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();

            obj.Columns[2].DataSource = new Liste().Reclamo;

            obj.Columns[4].DataSource = new Liste().Documento;

            exp.Export(obj, dataSource, now + " - Reclami.docx", false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/reclami/reclamipdf")]
        public ActionResult ReclamiPdf(string gridModel)
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

            IEnumerable dataSource = context.Reclami.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.DataCreazione).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();

            obj.Columns[2].DataSource = new Liste().Reclamo;

            obj.Columns[4].DataSource = new Liste().Documento;

            exp.Export(obj, dataSource, now + " - Reclami.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
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
            var thisYear = DateTime.Today.Year;
            #endregion

            var dataSource = _db.Carico.Where(c => currentUser.pvID == c.pvID && c.Year.Anno.Year == thisYear).OrderByDescending(o => o.Ordine).ToList();
            ViewBag.datasource = dataSource;
            
            IEnumerable dataSource2 = _db.Year.Where(a => a.Anno.Year == thisYear).ToList();
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

            // Totale Additivo Gasolio secondo il parametro di ricerca specificato
            ViewBag.DieselTotalAmountFrom = dataSource.ToList()
                .Where(o => /*currentUser.pvID == o.pvID &&*/ Convert.ToDateTime(o.cData) >= dateFrom && Convert.ToDateTime(o.cData) <= dateTo)
                .Sum(o => (decimal?)o.Lube);
            #endregion

            #region Total Ammount ViewBag
            // Totale Carico annuo benzina.
            ViewBag.SSPBTotalAmount = dataSource.Sum(o => (decimal?)o.Benzina + (decimal?)o.HiQb);

            // Totale Carico annuo gasolio. 
            ViewBag.DieselTotalAmount = dataSource.Sum(o => (decimal?)o.Gasolio + (decimal?)o.HiQd);

            // Totale additivo
            ViewBag.LubeTotalAmount = dataSource.Sum(o => (decimal?)o.Lube);
            #endregion

            return View();
        }
        
        [Route("user/ordini/update")]
        public ActionResult UpdateCarico(Carico value)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            var thisYear = DateTime.Today.Year;
            var _selectThisYear = (from a in _db.Year
                                   where a.Anno.Year == thisYear
                                   select a.yearId).Single();
            #endregion

            value.yearId = _selectThisYear;
            OrderRepository.Update(value);
            var data = _db.Carico.Where(c => currentUser.pvID == c.pvID && c.Year.Anno.Year.ToString().Contains(Ly));
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/ordini/insert")]
        public ActionResult InsertCarico(Carico value)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            var thisYear = DateTime.Today.Year;
            var _selectThisYear = (from a in _db.Year
                                   where a.Anno.Year == thisYear
                                   select a.yearId).Single();
            #endregion

            value.yearId = _selectThisYear;
            value.cData = DateTime.Today.Date;
            value.rData = DateTime.Today.Date;

            OrderRepository.Add(value);
            var data = _db.Carico.Where(c => currentUser.pvID == c.pvID && c.Year.Anno.Year.ToString().Contains(Ly));
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
            obj.Columns[5].DataSource = _db.Documento.ToList();

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
            obj.Columns[5].DataSource = _db.Documento.ToList();

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
            obj.Columns[5].DataSource = _db.Documento.ToList();

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

            var dep = _db.Deposito;

            var doc = _db.Documento;

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
                #region var
                /*
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var currentUser = userManager.FindById(User.Identity.GetUserId());

                var thisYear = DateTime.Now.Year;

                var getall = from order in _pvRepository.GetPvs()
                             where (currentUser.pvID == order.pvID)
                             select order;

                var year = _db.Year
                    .Where(c => c.Anno.Year == thisYear);

                var dep = _db.Deposito;

                var doc = _db.Documento;

                ViewBag.pvID = new SelectList(getall, "pvID", "pvName", insertOrder.pvID);
                ViewBag.yearId = new SelectList(year, "yearId", "Anno", insertOrder.yearId);
                ViewBag.depId = new SelectList(dep, "depId", "Nome", insertOrder.depId);
                ViewBag.docId = new SelectList(doc, "DocumentoID", "Tipo", insertOrder.DocumentoID);*/
                #endregion

                OrderRepository.Add(insertOrder);
                return RedirectToAction("Index");
            }

            return View(insertOrder);
        }
        
        #endregion

        #region Contatori
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

            var dataSource = new MyDbContext().PvErogatori.Where(c => currentUser.pvID == c.pvID && (c.FieldDate.Year.ToString().Contains(Ly)) && c.Dispenser.isActive == true).OrderByDescending(o => o.FieldDate).ToList();
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
                if (_operation.GetErogatori().Count() == 0)
                {
                    ViewBag.SSPBTotalAmount = "0";
                    ViewBag.DieselTotalAmount = "0";
                }
                else
                {
                    #region Variabili
                    var totalB = _operation.DoSSPBOperation();
                    var totalG = _operation.DoDSLOperation();
                    #endregion

                    ViewBag.SSPBTotalAmount = totalB;
                    ViewBag.DieselTotalAmount = totalG;
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
                if (_operation.GetErogatoriByParam(dal: dateFrom, a: dateTo).Count() == 0)
                {
                    ViewBag.SSPBTotalAmountFrom = "0";
                    ViewBag.DieselTotalAmountFrom = "0";
                }
                else
                {
                    #region Variabili
                    var totalB = _operation.DoSSPBOperationByParam(dal: dateFrom, a: dateTo);
                    var totalG = _operation.DoDSLOperationByParam(dal: dateFrom, a: dateTo);
                    #endregion

                    ViewBag.SSPBTotalAmountFrom = totalB;
                    ViewBag.DieselTotalAmountFrom = totalG;
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

        #region Licenza

        [Route("user/licenza/")]
        [WhitespaceFilter]
        public ActionResult Licenza()
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            var dataSource = new MyDbContext().Licenza.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.nSuccessivo).ToList();
            ViewBag.datasource = dataSource;

            IEnumerable dataSource2 = new MyDbContext().Pv.Where(c => currentUser.pvID == c.pvID).ToList();
            ViewBag.datasource2 = dataSource2;
            

            return View();
        }

        [Route("user/licenza/update")]
        public ActionResult UpdateLicenza(Licenza value)
        {
            LicenzaRepository.Update(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        [Route("user/licenza/insert")]
        public ActionResult InsertLicenza(Licenza value)
        {
            LicenzaRepository.Add(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("user/licenza/remove")]
        public ActionResult RemoveLicenza(Guid key)
        {
            #region Initial var
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            #endregion

            MyDbContext context = new MyDbContext();
            context.Licenza.Remove(context.Licenza.Single(o => o.LicenzaID == key));
            context.SaveChanges();

            var data = context.Licenza.Where(c => currentUser.pvID == c.pvID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("user/licenza/licenzaexcell")]
        public ActionResult LicenzaExcell(string gridModel)
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
            IEnumerable dataSource = context.Licenza.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.nSuccessivo).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, dataSource, now + " - Licenza.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/licenza/licenzaword")]
        public ActionResult LicenzaWord(string gridModel)
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

            IEnumerable dataSource = context.Licenza.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.nSuccessivo).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, dataSource, now + " - Licenza.docx", false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("user/licenza/licenzapdf")]
        public ActionResult LicenzaPdf(string gridModel)
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

            IEnumerable dataSource = context.Licenza.Where(c => currentUser.pvID == c.pvID).OrderBy(o => o.nSuccessivo).ToList();

            var obj = ConvertGridObject(gridModel);

            obj.Columns[1].DataSource = context.Pv.Where(a => currentUser.pvID == a.pvID).ToList();

            exp.Export(obj, dataSource, now + " - Licenza.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
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
            value.LastDate = DateTime.Today.Date;
            TankRepository.Update(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        [Route("user/cisterne/insert")]
        public ActionResult InsertPvTanks(PvTank value)
        {
            value.LastDate = DateTime.Today.Date;
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

            IEnumerable dataSource = new MyDbContext().PvDeficienze.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(Ly))).OrderByDescending(o => o.FieldDate).ToList();
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
            
            IEnumerable DataSource = new MyDbContext().PvCali.Where(c => currentUser.pvID == c.PvTank.pvID && (c.FieldDate.ToString().Contains(Ly))).OrderByDescending(o => o.FieldDate).ToList();
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

            var DataSource = new MyDbContext().Dispenser.Where(c => currentUser.pvID == c.PvTank.pvID).OrderByDescending(o => o.Modello).ToList();
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
            value.isActive = true;
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
        [HttpPost, ActionName("EditProfiles")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(Guid id, HttpPostedFileBase upload)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var profileToUpdate = _db.UserProfiles.Find(id);
            var myOldImage = _db.UsersImage.Where(s => s.ProfileID == id).Select(s => s.UsersImageID).Single();
            var _userImageId = _db.UsersImage.Find(myOldImage);
            if (TryUpdateModel(profileToUpdate, "",
                new string[] { "ProfileName", "ProfileSurname", "ProfileAdress", "ProfileCity", "ProfileZipCode", "ProfileNation", "ProfileInfo" }))
            {
                try
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        if (profileToUpdate.UsersImage.Any(f => f.FileType == FileType.Avatar))
                        {
                            profileToUpdate.UsersImage.Remove(profileToUpdate.UsersImage.Single(f => f.FileType == FileType.Avatar && _userImageId.UsersImageID == f.UsersImageID));
                        }

                        var avatar = new UsersImage
                        {
                                UsersImageID = _userImageId.UsersImageID,
                                UsersImageName = string.Format(Guid.NewGuid() + "-" + Path.GetFileName(upload.FileName)),
                                FileType = FileType.Avatar,
                                ContentType = upload.ContentType,
                                UploadDate = DateTime.Now.Date
                        };

                        using (var reader = new BinaryReader(upload.InputStream))
                        {
                            avatar.Content = reader.ReadBytes(upload.ContentLength);
                        }
                            profileToUpdate.UsersImage = new List<UsersImage> { avatar };
                        }

                        _db.Entry(profileToUpdate).State = EntityState.Modified;
                        _db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                catch (RetryLimitExceededException dex)
                {
                    var error = string.Format("{0}, {1}, {2}", dex.Message, dex.Source, dex.HResult);
                    ModelState.AddModelError(error, "Impossibile salvare le modifiche. Prova di nuovo, e se il problema persiste contattaci copiando l'errore sopraciato nel messaggio.");
                }
            }
            return View(profileToUpdate);
        }

        [Authorize(Roles = "Administrators")]
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
            #region Current User
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentUser = userManager.FindById(User.Identity.GetUserId());
            #endregion

            #region Get Operations
            var getCarico = _operation.GetCarico();
            var getErogatori = _operation.GetErogatori();
            var getDeficienze = _operation.GetDeficienze();
            var getCali = _operation.GetCali();
            var getLicenza = _operation.GetLicenza();
            var getTank = _operation.GetTank();
            var getPvProfile = _operation.GetPvProfile();
            #endregion

            #region Variabili
            var emptyValue = "0";

            var _cisternaRimB = getTank
                                .Where(w => w.Product.Nome.Contains("B"))
                                .Select(s => s.Rimanenza);

            var _cisternaRimG = getTank
                                .Where(w => w.Product.Nome.Contains("G"))
                                .Select(s => s.Rimanenza);

            var _ordiniEccedenze = getCarico
                .Where(w => w.DocumentoID.ToString() == "ce657bdc-ef45-4014-b638-ebac50214031");

            var _ordiniScatti = getCarico
                .Where(w => w.DocumentoID.ToString() == "cb8deef4-a81d-45f6-bc6b-6ee445a5f6c7");

            var _rimIB = _cisternaRimB.Sum();
            var _rimIG = _cisternaRimG.Sum();
            #endregion

            #region ViewBag

            ViewBag.Città = getPvProfile.Select(a => a.Città).DefaultIfEmpty(emptyValue).SingleOrDefault();

            ViewBag.nPrecedente = getLicenza.Select(a => a.nPrecedente).DefaultIfEmpty(emptyValue).SingleOrDefault();

            ViewBag.nSuccessivo = getLicenza.Select(a => a.nSuccessivo).DefaultIfEmpty(emptyValue).SingleOrDefault();

            ViewBag.Code = getLicenza.Select(a => a.Codice).DefaultIfEmpty(emptyValue).SingleOrDefault();

            #endregion

            // caso in cui non ho contatori ne ordini, dunque la chiusura non può essere compilata, e per questo assegno a tutti i viewbag lo zero
            if (getErogatori == null && getCarico == null && _ordiniEccedenze == null && _ordiniScatti == null && getCali == null && getDeficienze == null)
            {
                #region ViewBag
                ViewBag.eccB = 0;
                ViewBag.eccG = 0;
                ViewBag.scatB = 0;
                ViewBag.scatG = 0;
                ViewBag.sumdB = 0;
                ViewBag.sumdG = 0;
                ViewBag.totalB = 0;
                ViewBag.totalG = 0;
                ViewBag.SumcB = 0;
                ViewBag.SumcG = 0;
                ViewBag.TotScaricoB = 0;
                ViewBag.TotScaricoG = 0;
                ViewBag.RiportoB = 0;
                ViewBag.RiportoG = 0;
                ViewBag.RiportoEB = 0;
                ViewBag.RiportoEG = 0;
                ViewBag.DifEB = 0;
                ViewBag.DifEG = 0;
                #endregion
            }

            // caso in cui ho tutto
            if (getCarico.Count() > 0 && _ordiniEccedenze.Count() > 0 && _ordiniScatti.Count() > 0 && getErogatori.Count() > 0 && getDeficienze.Count() > 0 && getCali.Count() > 0)
            {
                #region Carico

                #region Variabili
                var _eccB = _ordiniEccedenze.Sum(s => s.Benzina + s.HiQb);
                var _eccG = _ordiniEccedenze.Sum(s => s.Gasolio + s.HiQd);
                var _scatB = _ordiniScatti.Sum(s => s.Benzina + s.HiQb);
                var _scatG = _ordiniScatti.Sum(s => s.Gasolio + s.HiQd);
                var _totCaricoB = _operation.GetCarico().Sum(s => s.Benzina + s.HiQb);
                var _totCaricoG = _operation.GetCarico().Sum(s => s.Gasolio + s.HiQd);
                var TotCaricoB = _rimIB + _totCaricoB;
                var TotCaricoG = _rimIG + _totCaricoG;
                var rCaricoB = _totCaricoB - _eccB - _scatB;
                var rCaricoG = _totCaricoG - _eccG - _scatG;
                #endregion

                #region ViewBag
                ViewBag.eccB = _eccB;
                ViewBag.eccG = _eccG;
                ViewBag.scatB = _scatB;
                ViewBag.scatG = _scatG;
                ViewBag.RimIB = _rimIB;
                ViewBag.RimIG = _rimIG;
                ViewBag.rCaricoB = rCaricoB;
                ViewBag.rCaricoG = rCaricoG;
                ViewBag.TotCaricoB = TotCaricoB;
                ViewBag.TotCaricoG = TotCaricoG;
                #endregion

                #endregion

                #region Scarico

                #region contatori <= 2
               
                var nPvDispenser = _db.Dispenser
                    .Where(a => a.PvTank.pvID == currentUser.pvID)
                    .Select(s => s.DispenserId);

                if (nPvDispenser.Count() == 6)
                {
                    #region Contatori
                    var totalB = _operation.DoSSPBOperationShort();
                    var totalG = _operation.DoDSLOperationShort();
                    #endregion

                    #region deficienze

                    var sumdB = _operation.DoSSPBDeficienze();
                    var sumdG = _operation.DoDSLDeficienze();
                    ViewBag.sumdB = sumdB;
                    ViewBag.sumdG = sumdG;

                    #endregion

                    #region cali

                    var sumcB = _operation.DoSSPBCali();
                    var sumcD = _operation.DoDSLCali();
                    ViewBag.SumcB = sumcB;
                    ViewBag.SumcG = sumcD;

                    #endregion

                    #region Totale Venduto Benzina e Gasolio
                    var totScaricoB = totalB + sumdB + sumcB;
                    var totScaricoG = totalG + sumdG + sumcD;
                    #endregion

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

                    #region Differenze
                    var rimContB = TotCaricoB - totScaricoB;
                    var rimContG = TotCaricoG - totScaricoG;
                    #endregion

                    #region ViewBag
                    ViewBag.totalB = totalB;
                    ViewBag.totalG = totalG;
                    ViewBag.nettoB = 0;
                    ViewBag.nettoG = 0;
                    ViewBag.TotScaricoB = totScaricoB;
                    ViewBag.TotScaricoG = totScaricoG;
                    ViewBag.RiportoB = rimContB;
                    ViewBag.RiportoG = rimContG;
                    ViewBag.RiportoEB = rimEfB;
                    ViewBag.RiportoEG = rimEfG;
                    ViewBag.DifEB = rimEfB - rimContB;
                    ViewBag.DifEG = rimEfG - rimContG;
                    #endregion
                }
                #endregion

                #region contatori > 2
                else
                {
                    #region Contatori
                    var totalB = _operation.DoSSPBOperation();
                    var totalG = _operation.DoDSLOperation();
                    ViewBag.totalB = totalB;
                    ViewBag.totalG = totalG;
                    #endregion

                    #region deficienze

                    var sumdB = _operation.DoSSPBDeficienze();
                    var sumdG = _operation.DoDSLDeficienze();
                    ViewBag.sumdB = sumdB;
                    ViewBag.sumdG = sumdG;

                    #endregion

                    #region cali

                    var sumcB = _operation.DoSSPBCali();
                    var sumcG = _operation.DoDSLCali();
                    ViewBag.SumcB = sumcB;
                    ViewBag.SumcG = sumcG;

                    #endregion

                    #region Totale Venduto Benzina e Gasolio

                    var totScaricoB = totalB + sumdB + sumcB;
                    var totScaricoG = totalG + sumdG + sumcG;

                    #endregion

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

                    #region Differenze
                    var rimContB = TotCaricoB - totScaricoB;
                    var rimContG = TotCaricoG - totScaricoG;
                    #endregion

                    #region ViewBag
                    ViewBag.TotScaricoB = totScaricoB;
                    ViewBag.TotScaricoG = totScaricoG;
                    ViewBag.nettoB = 0;
                    ViewBag.nettoG = 0;
                    ViewBag.RiportoB = rimContB;
                    ViewBag.RiportoG = rimContG;
                    ViewBag.RiportoEB = rimEfB;
                    ViewBag.RiportoEG = rimEfG;
                    ViewBag.DifEB = rimEfB - rimContB;
                    ViewBag.DifEG = rimEfG - rimContG;
                    #endregion
                }
                #endregion

                #endregion
            }

            ViewBag.pvId = new SelectList(_db.Pv, "pvID", "pvName");
            return View();
        }

        #endregion

        #region Premium



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