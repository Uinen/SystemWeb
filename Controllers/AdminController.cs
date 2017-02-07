﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SystemWeb.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using PagedList;

namespace SystemWeb.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {

        #region Inizializzatori
        private MyDbContext db = new MyDbContext();
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
            list.prodotto = db.Product.ToList();
            list.flag = db.Flag.ToList();
            list.utente = db.Users.ToList();
            list.notizia = db.Notice.ToList();
            #endregion

            return View(list);
        }
        #endregion

        #region YearsController
        public ActionResult Years()
        {
            return View(db.Year.ToList());
        }
        
        public ActionResult YearsDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Year year = db.Year.Find(id);
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
        public ActionResult YearsCreate([Bind(Include = "yearId,Anno")] SystemWeb.Models.Year year)
        {
            if (ModelState.IsValid)
            {
                year.yearId = Guid.NewGuid();
                db.Year.Add(year);
                db.SaveChanges();
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
            SystemWeb.Models.Year year = db.Year.Find(id);
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
                db.Entry(year).State = EntityState.Modified;
                db.SaveChanges();
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
            SystemWeb.Models.Year year = db.Year.Find(id);
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
            SystemWeb.Models.Year year = db.Year.Find(id);
            db.Year.Remove(year);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        #region NoticesController
        public ActionResult Notices()
        {
            var notice = db.Notice.Include(n => n.ApplicationUser);
            return View(notice.ToList());
        }

        public ActionResult NoticesDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Notice notice = db.Notice.Find(id);
            if (notice == null)
            {
                return HttpNotFound();
            }
            return View(notice);
        }

        public ActionResult NoticesCreate()
        {
            ViewBag.UsersId = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NoticesCreate([Bind(Include = "NoticeId,NoticeName,CreateDate,TextBox,Url,UsersId,Description")] SystemWeb.Models.Notice notice)
        {
            if (ModelState.IsValid)
            {
                notice.NoticeId = Guid.NewGuid();
                notice.CreateDate = DateTime.Now;
                notice.UsersId = User.Identity.GetUserId();
                db.Notice.Add(notice);
                db.SaveChanges();
            }

            ViewBag.UsersId = new SelectList(db.Users, "Id", "UserName", notice.UsersId);
            return View(notice);
        }
        
        
        public ActionResult NoticesEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Notice notice = db.Notice.Find(id);
            if (notice == null)
            {
                return HttpNotFound();
            }
            ViewBag.UsersId = new SelectList(db.Users, "Id", "UserName", notice.UsersId);
            return View(notice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NoticesEdit([Bind(Include = "NoticeId,NoticeName,CreateDate,TextBox,UsersId")] SystemWeb.Models.Notice notice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Notices");
            }
            ViewBag.UsersId = new SelectList(db.Users, "Id", "UserName", notice.UsersId);
            return View(notice);
        }

        public ActionResult NoticesDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Notice notice = db.Notice.Find(id);
            if (notice == null)
            {
                return HttpNotFound();
            }
            return View(notice);
        }

        [HttpPost, ActionName("NoticesDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult NoticesDeleteConfirmed(Guid id)
        {
            SystemWeb.Models.Notice notice = db.Notice.Find(id);
            db.Notice.Remove(notice);
            db.SaveChanges();
            return RedirectToAction("Notices");
        }
        #endregion

        #region ProductsController
        public ActionResult Products()
        {
            var DataSource = new MyDbContext().Product.ToList();
            ViewBag.dataSource = DataSource;
            return View();
        }
        public ActionResult ProductsDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        public ActionResult ProductsCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductsCreate([Bind(Include = "ProductId,Nome,Peso,Prezzo")] SystemWeb.Models.Product product)
        {
            if (ModelState.IsValid)
            {
                product.ProductId = Guid.NewGuid();
                db.Product.Add(product);
                db.SaveChanges();
                return RedirectToAction("Products");
            }

            return View(product);
        }
        public ActionResult ProductsEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductsEdit([Bind(Include = "ProductId,Nome,Peso,Prezzo")] SystemWeb.Models.Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Products");
            }
            return View(product);
        }
        public ActionResult ProductsDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("ProductsDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult ProductsDeleteConfirmed(Guid id)
        {
            SystemWeb.Models.Product product = db.Product.Find(id);
            db.Product.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Products");
        }
        #endregion

        #region RagioneSocialeController
        public ActionResult RagioneSociale()
        {
            return View(db.RagioneSociale.ToList());
        }
        public ActionResult RagioneSocialeDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.RagioneSociale ragioneSociale = db.RagioneSociale.Find(id);
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
        public ActionResult RagioneSocialeCreate([Bind(Include = "RagioneSocialeId,Nome")] SystemWeb.Models.RagioneSociale ragioneSociale)
        {
            if (ModelState.IsValid)
            {
                ragioneSociale.RagioneSocialeId = Guid.NewGuid();
                db.RagioneSociale.Add(ragioneSociale);
                db.SaveChanges();
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
            SystemWeb.Models.RagioneSociale ragioneSociale = db.RagioneSociale.Find(id);
            if (ragioneSociale == null)
            {
                return HttpNotFound();
            }
            return View(ragioneSociale);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RagioneSocialeEdit([Bind(Include = "RagioneSocialeId,Nome")] SystemWeb.Models.RagioneSociale ragioneSociale)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ragioneSociale).State = EntityState.Modified;
                db.SaveChanges();
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
            SystemWeb.Models.RagioneSociale ragioneSociale = db.RagioneSociale.Find(id);
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
            SystemWeb.Models.RagioneSociale ragioneSociale = db.RagioneSociale.Find(id);
            db.RagioneSociale.Remove(ragioneSociale);
            db.SaveChanges();
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
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Ordine_Desc" : "";
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
            if (!String.IsNullOrEmpty(searchString))
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
            return View(db.Flag.ToList());
        }

        public ActionResult FlagsDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemWeb.Models.Flag flag = db.Flag.Find(id);
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
        public ActionResult FlagsCreate([Bind(Include = "pvFlagId,Nome,Descrizione")] SystemWeb.Models.Flag flag)
        {
            if (ModelState.IsValid)
            {
                flag.pvFlagId = Guid.NewGuid();
                db.Flag.Add(flag);
                db.SaveChanges();
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
            SystemWeb.Models.Flag flag = db.Flag.Find(id);
            if (flag == null)
            {
                return HttpNotFound();
            }
            return View(flag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FlagsEdit([Bind(Include = "pvFlagId,Nome,Descrizione")] SystemWeb.Models.Flag flag)
        {
            if (ModelState.IsValid)
            {
                db.Entry(flag).State = EntityState.Modified;
                db.SaveChanges();
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
            SystemWeb.Models.Flag flag = db.Flag.Find(id);
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
            SystemWeb.Models.Flag flag = db.Flag.Find(id);
            db.Flag.Remove(flag);
            db.SaveChanges();
            return RedirectToAction("Flags");
        }
        #endregion
   
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}