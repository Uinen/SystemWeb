using GestioniDirette.Database.Utility;
using GestioniDirette.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Controllers
{
    public class ValidationController : Controller
    {
        // GET: Validation
        public ActionResult Index()
        {
            // List of things you can do with Validation
            // Tweak the default settings
            return View();
        }

        // POST: Validation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Default(RegisterBindingModel model, IOwinContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>(context.Get<MyDbContext>()));
            
            // The default Validators that the UserManager uses are UserValidator and MinimumLengthValidator
            // You can tweak some of the settings as follows
            // This example sets the Password length to be 3 characters
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false
            };
             userManager.PasswordValidator = new MinimumLengthValidator(3);


            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.Username };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var authManager = HttpContext.GetOwinContext().Authentication;
                    var claimsIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authManager.SignIn(claimsIdentity);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Customize(RegisterBindingModel model, IOwinContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>(context.Get<MyDbContext>()));

            // The default Validators that the UserManager uses are UserValidator and MinimumLengthValidator
            // If you want to have complete control over validation then you can write your own validators.
            userManager.UserValidator = new MyUserValidation();
            userManager.PasswordValidator = new MyPasswordValidation();
            userManager.PasswordHasher = new PasswordHasher();


            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.Username };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var authManager = HttpContext.GetOwinContext().Authentication;
                    var claimsIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authManager.SignIn(claimsIdentity);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View("Index",model);
        }
    }
}
