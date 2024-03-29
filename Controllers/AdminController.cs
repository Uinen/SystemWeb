﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestioniDirette.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using PagedList;
using Syncfusion.EJ.Export;
using System.Collections;
using Syncfusion.XlsIO;
using Syncfusion.JavaScript.Models;
using System.Web.Script.Serialization;
using System.Reflection;
using GestioniDirette.Database.Entity;
using GestioniDirette.Database.Repository;

namespace GestioniDirette.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {

        #region Inizializzatori
        private readonly MyDbContext _db = new MyDbContext();
        private readonly UserInfo _userInfo = new UserInfo();
        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        #endregion

        #region Index 
        public ActionResult Index()
        {
            #region Inizializzatori
            AdminIndexViewModel list = new AdminIndexViewModel();
            list.prodotto = _db.Product.ToList();
            list.flag = _db.Flag.ToList();
            list.utente = _db.Users.ToList();
            list.notizia = _db.Notice.ToList();
            #endregion

            ViewBag.GetTotalUser = _userInfo.GetTotalUsers();
            ViewBag.GetMypvID = _userInfo.GetPVID();
            return View(list);
        }
        #endregion

        #region YearsController
        public ActionResult Years()
        {
            return View(_db.Year.ToList());
        }
        
        public ActionResult YearsDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Year year = _db.Year.Find(id);
            if (year == null)
            {
                return HttpNotFound();
            }
            return View(year);
        }

        // GET: Years/Create
        public ActionResult YearsCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YearsCreate([Bind(Include = "yearId,Anno")] Year year)
        {
            if (ModelState.IsValid)
            {
                year.yearId = Guid.NewGuid();
                _db.Year.Add(year);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(year);
        }

        public ActionResult YearsEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Year year = _db.Year.Find(id);
            if (year == null)
            {
                return HttpNotFound();
            }
            return View(year);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YearsEdit([Bind(Include = "yearId,Anno")] Year year)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(year).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(year);
        }

        public ActionResult YearsDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Year year = _db.Year.Find(id);
            if (year == null)
            {
                return HttpNotFound();
            }
            return View(year);
        }

        [HttpPost, ActionName("YearsDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult YearsDeleteConfirmed(Guid id)
        {
            Year year = _db.Year.Find(id);
            _db.Year.Remove(year);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        #region NoticesController
        [Route("admin/notizie")]
        public ActionResult Notices()
        {
            var dataSource = _db.Notice.ToList();
            //var _getUser = (from user in _db.Users select user).FirstOrDefault();
            ViewBag.datasource = dataSource;
            //ViewBag.datasource2 = _getUser;

            return View();
        }

        [Route("admin/notizie/update")]
        public ActionResult UpdateNotices(Notice value)
        {
            value.CreateDate = DateTime.Today.Date;
            value.UsersId = User.Identity.GetUserId();
            NoticeRepository.Update(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        [Route("admin/notizie/insert")]
        public ActionResult InsertNotices(Notice value)
        {
            value.CreateDate = DateTime.Today.Date;
            value.UsersId = User.Identity.GetUserId();
            NoticeRepository.Add(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("admin/notizie/remove")]
        public ActionResult RemoveNotices(Guid key)
        {
            _db.Notice.Remove(_db.Notice.Single(o => o.NoticeId == key));
            _db.SaveChanges();
            var data = _db.Product.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("admin/notizie/excell")]
        public ActionResult NoticesExcell(string gridModel)
        {
            var exp = new ExcelExport();
            var context = new MyDbContext();
            var now = Guid.NewGuid();
            IEnumerable dataSource = context.Notice.OrderBy(o => o.CreateDate).ToList();

            var obj = ConvertGridObject(gridModel);

            exp.Export(obj, dataSource, now + " - Notizie.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("admin/notizie/word")]
        public ActionResult NoticesWord(string gridModel)
        {
            var context = new MyDbContext();
            var now = Guid.NewGuid();
            var exp = new WordExport();

            IEnumerable dataSource = context.Notice.OrderBy(o => o.CreateDate).ToList();

            var obj = ConvertGridObject(gridModel);

            exp.Export(obj, dataSource, now + " - Notizie.docx", false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("admin/notizie/pdf")]
        public ActionResult NoticesPdf(string gridModel)
        {
            var context = new MyDbContext();

            var now = Guid.NewGuid();

            var exp = new PdfExport();

            IEnumerable dataSource = context.Notice.OrderBy(o => o.CreateDate).ToList();

            var obj = ConvertGridObject(gridModel);

            exp.Export(obj, dataSource, now + " - Notizie.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ProductsController

        [Route("admin/prodotti")]
        public ActionResult Products()
        {
            var dataSource = _db.Product.ToList();
            ViewBag.datasource = dataSource;

            return View();
        }

        [Route("admin/prodotti/update")]
        public ActionResult UpdateProducts(Product value)
        {
            ProductsRepository.Update(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        [Route("admin/prodotti/insert")]
        public ActionResult InsertProducts(Product value)
        {
            ProductsRepository.Add(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("admin/prodotti/remove")]
        public ActionResult RemoveProducts(Guid key)
        {
            _db.Product.Remove(_db.Product.Single(o => o.ProductId == key));
            _db.SaveChanges();
            var data = _db.Product.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("admin/prodotti/excell")]
        public ActionResult ProductsExcell(string gridModel)
        {
            var exp = new ExcelExport();
            var context = new MyDbContext();
            var now = Guid.NewGuid();
            IEnumerable dataSource = context.Product.OrderBy(o => o.Nome).ToList();

            var obj = ConvertGridObject(gridModel);

            exp.Export(obj, dataSource, now + " - Prodotti.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("admin/prodotti/word")]
        public ActionResult ProductsWord(string gridModel)
        {
            var context = new MyDbContext();
            var now = Guid.NewGuid();
            var exp = new WordExport();

            IEnumerable dataSource = context.Product.OrderBy(o => o.Nome).ToList();

            var obj = ConvertGridObject(gridModel);
            
            exp.Export(obj, dataSource, now + " - Prodotti.docx", false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("admin/prodotti/pdf")]
        public ActionResult ProductsPdf(string gridModel)
        {
            var context = new MyDbContext();

            var now = Guid.NewGuid();

            var exp = new PdfExport();

            IEnumerable dataSource = context.Product.OrderBy(o => o.Nome).ToList();

            var obj = ConvertGridObject(gridModel);
            
            exp.Export(obj, dataSource, now + " - Prodotti.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region RagioneSocialeController
        public ActionResult RagioneSociale()
        {
            return View(_db.RagioneSociale.ToList());
        }
        public ActionResult RagioneSocialeDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RagioneSociale ragioneSociale = _db.RagioneSociale.Find(id);
            if (ragioneSociale == null)
            {
                return HttpNotFound();
            }
            return View(ragioneSociale);
        }

        public ActionResult RagioneSocialeCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RagioneSocialeCreate([Bind(Include = "RagioneSocialeId,Nome")] RagioneSociale ragioneSociale)
        {
            if (ModelState.IsValid)
            {
                ragioneSociale.RagioneSocialeId = Guid.NewGuid();
                _db.RagioneSociale.Add(ragioneSociale);
                _db.SaveChanges();
                return RedirectToAction("RagioneSociale");
            }

            return View(ragioneSociale);
        }
 
        public ActionResult RagioneSocialeEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RagioneSociale ragioneSociale = _db.RagioneSociale.Find(id);
            if (ragioneSociale == null)
            {
                return HttpNotFound();
            }
            return View(ragioneSociale);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RagioneSocialeEdit([Bind(Include = "RagioneSocialeId,Nome")] RagioneSociale ragioneSociale)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(ragioneSociale).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("RagioneSociale");
            }
            return View(ragioneSociale);
        }

        public ActionResult RagioneSocialeDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RagioneSociale ragioneSociale = _db.RagioneSociale.Find(id);
            if (ragioneSociale == null)
            {
                return HttpNotFound();
            }
            return View(ragioneSociale);
        }

        [HttpPost, ActionName("RagioneSocialeDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult RagioneSocialeDeleteConfirmed(Guid id)
        {
            RagioneSociale ragioneSociale = _db.RagioneSociale.Find(id);
            _db.RagioneSociale.Remove(ragioneSociale);
            _db.SaveChanges();
            return RedirectToAction("RagioneSociale");
        }
        #endregion

        #region RolesAdminController
        public ActionResult Roles()
        {
            return View(RoleManager.Roles);
        }
        public async Task<ActionResult> RolesDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            // Get the list of Users in this Role
            var users = new List<ApplicationUser>();

            // Get the list of Users in this Role
            foreach (var user in UserManager.Users.ToList())
            {
                if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();
            return View(role);
        }
 
        public ActionResult RolesCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RolesCreate(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole(roleViewModel.Name);
                var roleresult = await RoleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First());
                    return View();
                }
                return RedirectToAction("Roles");
            }
            return View();
        }

        public async Task<ActionResult> RolesEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            RoleViewModel roleModel = new RoleViewModel { Id = role.Id, Name = role.Name };
            return View(roleModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RolesEdit([Bind(Include = "Name,Id")] RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name;
                await RoleManager.UpdateAsync(role);
                return RedirectToAction("Roles");
            }
            return View();
        }
 
        public async Task<ActionResult> RolesDelete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        [HttpPost, ActionName("RolesDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RolesDeleteConfirmed(string id, string deleteUser)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                IdentityResult result;
                if (deleteUser != null)
                {
                    result = await RoleManager.DeleteAsync(role);
                }
                else
                {
                    result = await RoleManager.DeleteAsync(role);
                }
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Roles");
            }
            return View();
        }
        #endregion

        #region UserAdminController

        public ActionResult UserAdmin(string sortOrder, string currentFilter, string searchString, int? page)
        {
            MyDbContext db = new MyDbContext();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "Ordine_Desc" : "";
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
            var utenti = from s in db.Users
                         select s;
            if (!string.IsNullOrEmpty(searchString))
            {
                utenti = utenti.Where(s => s.UserName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "Ordine_Desc":
                    utenti = utenti.OrderByDescending(s => s.UserName);
                    break;
                default:
                    utenti = utenti.OrderBy(s => s.UserName);
                    break;
            }
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            return View(utenti.ToPagedList(pageNumber, pageSize));
        }

        public async Task<ActionResult> UserAdminDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        public async Task<ActionResult> UserAdminCreate()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UserAdminCreate(RegisterBindingModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();

                }
                return RedirectToAction("UserAdmin");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        public async Task<ActionResult> UserAdminEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Username = user.UserName,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserAdminEdit([Bind(Include = "Username,Id")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Username;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("UserAdmin");
            }
            ModelState.AddModelError("", "Si è verificato un errore sconosciuto.");
            return View();
        }
 
        public async Task<ActionResult> UserAdminDelete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("UserAdminDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("UserAdmin");
            }
            return View();
        }
        #endregion

        #region Flags

        public ActionResult Flags()
        {
            return View(_db.Flag.ToList());
        }

        public ActionResult FlagsDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Flag flag = _db.Flag.Find(id);
            if (flag == null)
            {
                return HttpNotFound();
            }
            return View(flag);
        }

        public ActionResult FlagsCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FlagsCreate([Bind(Include = "pvFlagId,Nome,Descrizione")] Flag flag)
        {
            if (ModelState.IsValid)
            {
                flag.pvFlagId = Guid.NewGuid();
                _db.Flag.Add(flag);
                _db.SaveChanges();
                return RedirectToAction("Flags");
            }

            return View(flag);
        }

        public ActionResult FlagsEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Flag flag = _db.Flag.Find(id);
            if (flag == null)
            {
                return HttpNotFound();
            }
            return View(flag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FlagsEdit([Bind(Include = "pvFlagId,Nome,Descrizione")] Flag flag)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(flag).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Flags");
            }
            return View(flag);
        }

        public ActionResult FlagsDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Flag flag = _db.Flag.Find(id);
            if (flag == null)
            {
                return HttpNotFound();
            }
            return View(flag);
        }

        [HttpPost, ActionName("FlagsDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult FlagsDeleteConfirmed(Guid id)
        {
            Flag flag = _db.Flag.Find(id);
            _db.Flag.Remove(flag);
            _db.SaveChanges();
            return RedirectToAction("Flags");
        }
        #endregion

        #region Depositi

        [Route("admin/depositi")]
        public ActionResult Depositi()
        {
            var dataSource = _db.Deposito.ToList();
            ViewBag.datasource = dataSource;

            return View();
        }

        [Route("admin/depositi/update")]
        public ActionResult UpdateDepositi(Deposito value)
        {
            DepotRepository.Update(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        [Route("admin/depositi/insert")]
        public ActionResult InsertDepositi(Deposito value)
        {
            DepotRepository.Add(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("admin/depositi/remove")]
        public ActionResult RemoveDepositi(Guid key)
        {
            _db.Deposito.Remove(_db.Deposito.Single(o => o.depId == key));
            _db.SaveChanges();
            var data = _db.Deposito.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("admin/depositi/excell")]
        public ActionResult DepositiExcell(string gridModel)
        {
            var exp = new ExcelExport();
            var context = new MyDbContext();
            var now = Guid.NewGuid();
            IEnumerable dataSource = context.Deposito.OrderBy(o => o.Nome).ToList();

            var obj = ConvertGridObject(gridModel);

            exp.Export(obj, dataSource, now + " - Depositi.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("admin/depositi/word")]
        public ActionResult DepositiWord(string gridModel)
        {
            var context = new MyDbContext();
            var now = Guid.NewGuid();
            var exp = new WordExport();

            IEnumerable dataSource = context.Deposito.OrderBy(o => o.Nome).ToList();

            var obj = ConvertGridObject(gridModel);

            exp.Export(obj, dataSource, now + " - Depositi.docx", false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("admin/depositi/pdf")]
        public ActionResult DepositiPdf(string gridModel)
        {
            var context = new MyDbContext();

            var now = Guid.NewGuid();

            var exp = new PdfExport();

            IEnumerable dataSource = context.Deposito.OrderBy(o => o.Nome).ToList();

            var obj = ConvertGridObject(gridModel);

            exp.Export(obj, dataSource, now + " - Depositi.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Documenti

        [Route("admin/documenti")]
        public ActionResult Documenti()
        {
            var dataSource = _db.Documento.ToList();
            ViewBag.datasource = dataSource;

            return View();
        }

        [Route("admin/documenti/update")]
        public ActionResult UpdateDocumenti(Documento value)
        {
            DocumentiRepository.Update(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        [Route("admin/documenti/insert")]
        public ActionResult InsertDocumenti(Documento value)
        {
            DocumentiRepository.Add(value);
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [Route("admin/documenti/remove")]
        public ActionResult RemoveDocumenti(Guid key)
        {
            _db.Documento.Remove(_db.Documento.Single(o => o.DocumentoID == key));
            _db.SaveChanges();
            var data = _db.Deposito.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Route("admin/documenti/excell")]
        public ActionResult DocumentiExcell(string gridModel)
        {
            var exp = new ExcelExport();
            var context = new MyDbContext();
            var now = Guid.NewGuid();
            IEnumerable dataSource = context.Documento.OrderBy(o => o.Tipo).ToList();

            var obj = ConvertGridObject(gridModel);

            exp.Export(obj, dataSource, now + " - Documenti.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("admin/documenti/word")]
        public ActionResult DocumentiWord(string gridModel)
        {
            var context = new MyDbContext();
            var now = Guid.NewGuid();
            var exp = new WordExport();

            IEnumerable dataSource = context.Documento.OrderBy(o => o.Tipo).ToList();

            var obj = ConvertGridObject(gridModel);

            exp.Export(obj, dataSource, now + " - Documenti.docx", false, false, "flat-saffron");
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Route("admin/documenti/pdf")]
        public ActionResult DocumentiPdf(string gridModel)
        {
            var context = new MyDbContext();

            var now = Guid.NewGuid();

            var exp = new PdfExport();

            IEnumerable dataSource = context.Documento.OrderBy(o => o.Tipo).ToList();

            var obj = ConvertGridObject(gridModel);

            exp.Export(obj, dataSource, now + " - Documenti.pdf", false, false, "flat-saffron");

            return Json(JsonRequestBehavior.AllowGet);
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
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}