using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SystemWeb.Models;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System;
using System.Web.Security;
using SystemWeb.ViewModels;
using System.IO;
using System.Collections.Generic;
using System.Net;

namespace SystemWeb.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, string userName, string password, bool rememberMe)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:

                default:
                    ModelState.AddModelError("", "Tentativo di login errato.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());

            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Codice non valido.");
                    return View(model);
            }
        }

        private MyDbContext db = new MyDbContext();

        [HttpGet]
        [AllowAnonymous]
        [Route("Account/New")]
        public ActionResult New()
        {
            RegistrationViewModel _model = new RegistrationViewModel()
            {
                WizardID = null,
                WizardType = "UserInfo"
            };

            return View("RegisterStep1", _model);
        }
        
        /*
        [HttpGet]
        [Route("Account/edit/{wizardID:int}")]
        public ActionResult Edit(int wizardID)
        {
            RegistrationViewModel _model = db.GetData(wizardID);

            return View("Step1", _model);
        }
        */

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("Account/RegisterStep1")]
        public ActionResult RegisterStep1(RegistrationViewModel model)
        {
            if (ModelState.IsValidField("RegisterStep1"))
            {
                return View("RegisterStep2", model);
            }
            return View("RegisterStep1", model);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Account/RegisterStep2")]
        public ActionResult RegisterStep2(RegistrationViewModel model)
        {
            if (ModelState.IsValidField("RegisterStep2"))
            {
                return View("RegisterStep3", model);
            }
            return View("RegisterStep2", model);
        }

        [AllowAnonymous]
        public ActionResult RegisterStep3()
        {
            var company = db.Company.Include(c => c.RagioneSociale);
            var pv = db.Pv.Include(p => p.Flag);
            ViewBag.pvFlagId = new SelectList(db.Flag, "pvFlagId", "Nome");
            ViewBag.CompanyId = new SelectList(db.Company, "CompanyId", "Name");
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName");
            ViewBag.RagioneSocialeId = new SelectList(db.RagioneSociale, "RagioneSocialeId", "Nome");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Account/RegisterStep3")]
        public ActionResult RegisterStep3(RegistrationViewModel model)
        {
            if (ModelState.IsValidField("RegisterStep3"))
            {
                return View("RegisterStep4", model);
            }
            return View("RegisterStep3", model);
        }

        [AllowAnonymous]
        public ActionResult RegisterStep4()
        {
            var company = db.Company.Include(c => c.RagioneSociale);
            var pv = db.Pv.Include(p => p.Flag);
            ViewBag.pvFlagId = new SelectList(db.Flag, "pvFlagId", "Nome");
            ViewBag.CompanyId = new SelectList(db.Company, "CompanyId", "Name");
            ViewBag.pvID = new SelectList(db.Pv, "pvID", "pvName");
            ViewBag.RagioneSocialeId = new SelectList(db.RagioneSociale, "RagioneSocialeId", "Nome");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterStep4(RegistrationViewModel model, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {

                //Save the data
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();

                var currentUser = userManager.FindById(User.Identity.GetUserId());

                const string roleName = "Administrator";

                var role = roleManager.FindByName(roleName);

                if (role == null)
                {
                    role = new ApplicationRole(roleName);
                    var roleresult = roleManager.Create(role);
                }

                var user = new ApplicationUser()
                {
                    UserName = model.step1.Username,
                    Email = model.step1.Email
                };
                
                user.TwoFactorEnabled = false;

                user.CreateDate = DateTime.Now;

                user.Company = new Company()
                {
                    Name = model.step3.name,
                    PartitaIva = model.step3.iva,
                    RagioneSocialeId = model.step3.RagioneSocialeId
                };

                user.Pv = new Pv()
                {
                    pvName = model.step4.PvName,
                    pvFlagId = model.step4.PvFlagId
                };

                if (upload != null && upload.ContentLength > 0)
                {
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

                    user.UserProfiles = new UserProfiles()
                    {
                        ProfileName = model.step2.mProfileName,
                        ProfileSurname = model.step2.mProfileSurname,
                        ProfileAdress = model.step2.mProfileAdress,
                        ProfileCity = model.step2.mProfileCity,
                        ProfileZipCode = model.step2.mProfilezipCode,
                        ProfileNation = model.step2.mProfileNation,
                        ProfileInfo = model.step2.mProfileInfo
                    };

                    user.UserProfiles.UsersImage = new List<UsersImage> { avatar };
                }

                var result = await UserManager.CreateAsync(user, model.step1.Password);

                if (result.Succeeded)
                {
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Conferma il tuo account");
                    var rolesForUser = UserManager.GetRoles(user.Id);
                    if (!rolesForUser.Contains(role.Name))
                    {
                        var result2 = UserManager.AddToRole(user.Id, role.Name);
                    }
                    await SignInAsync(user, isPersistent: false);
                    return View("Confirm");
                }
            }
            return RedirectToAction("Index", "User");
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }
        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Modifica Password", "Modifica la tua password confermando il link seguente: <a href=\"" + callbackUrl + "\">Conferma</a>");
                return View("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }
        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
        }
        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userID, subject,
               "Conferma il tuo Account cliccando <a href=\"" + callbackUrl + "\">Qui</a>");

            return callbackUrl;
        }
        //
        // GET: /Account/Manage
        [Authorize]
        public ActionResult Manage(ManageMessageId? message, IOwinContext context)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "La tua password è cambiata."
                : message == ManageMessageId.SetPasswordSuccess ? "La tua password è stata aggiunta."
                : message == ManageMessageId.RemoveLoginSuccess ? "Il login esterno è stato rimosso."
                : message == ManageMessageId.Error ? "Errore sconosciuto"
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>(context.Get<MyDbContext>()));

            // Get the current logged in User and look up the user in ASP.NET Identity
            var currentUser = manager.FindById(User.Identity.GetUserId());

            // Recover the profile information about the logged in user
            ViewBag.ProfileName = currentUser.UserProfiles.ProfileName;
            ViewBag.ProfileSurname = currentUser.UserProfiles.ProfileSurname;
            ViewBag.ProfileAdress = currentUser.UserProfiles.ProfileAdress;
            ViewBag.ProfileCity = currentUser.UserProfiles.ProfileCity;
            ViewBag.ProfileZipCode = currentUser.UserProfiles.ProfileZipCode;
            ViewBag.ProfileNation = currentUser.UserProfiles.ProfileNation;
            ViewBag.ProfileInfo = currentUser.UserProfiles.ProfileInfo;


            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous] 
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };

                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            // Add more custom claims here if you want. Eg HomeTown can be a claim for the User
            //var homeclaim = new Claim(ClaimTypes.Country, user.HomeTown);
            //identity.AddClaim(homeclaim);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
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

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public class ChallengeResult : HttpUnauthorizedResult
        {
            private string provider;
            private string p1;
            private string p2;

            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
        // The FormsAuthentication type is sealed and contains static members, so it is difficult to
        // unit test code that calls its members. The interface and helper class below demonstrate
        // how to create an abstract wrapper around such a type in order to make the AccountController
        // code unit testable.

        public interface IFormsAuthentication
        {
            void SignIn(string userName, bool createPersistentCookie);
            void SignOut();
        }

        public class FormsAuthenticationService : IFormsAuthentication
        {
            public void SignIn(string userName, bool createPersistentCookie)
            {
                FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
            }
            public void SignOut()
            {
                FormsAuthentication.SignOut();
            }
        }

        public interface IMembershipService
        {
            int MinPasswordLength { get; }

            bool ValidateUser(string userName, string password);
            string GetCanonicalUsername(string userName);
            MembershipCreateStatus CreateUser(string userName, string password, string email);
            bool ChangePassword(string userName, string oldPassword, string newPassword);
        }

        public class AccountMembershipService : IMembershipService
        {
            private MembershipProvider _provider;

            public AccountMembershipService()
                : this(null)
            {
            }

            public AccountMembershipService(MembershipProvider provider)
            {
                _provider = provider ?? Membership.Provider;
            }

            public int MinPasswordLength
            {
                get
                {
                    return _provider.MinRequiredPasswordLength;
                }
            }

            public bool ValidateUser(string userName, string password)
            {
                return _provider.ValidateUser(userName, password);
            }

            public string GetCanonicalUsername(string userName)
            {
                var user = _provider.GetUser(userName, true);
                if (user != null)
                {
                    return user.UserName;
                }

                return null;
            }

            public MembershipCreateStatus CreateUser(string userName, string password, string email)
            {
                MembershipCreateStatus status;
                _provider.CreateUser(userName, password, email, null, null, true, null, out status);
                return status;
            }

            public bool ChangePassword(string userName, string oldPassword, string newPassword)
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
        }
    }
}