using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;

namespace SystemWeb.Database.Entity
{
    public class ApplicationUserManager : UserManager<ApplicationUser, string>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, string> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>(context.Get<MyDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            manager.PasswordHasher = new PasswordHasher();
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("Codice via smartphone", new PhoneNumberTokenProvider<ApplicationUser>()
            {
                MessageFormat = "Il tuo codice da inserire è: {0}"
            });

            manager.RegisterTwoFactorProvider("Codice via mail", new EmailTokenProvider<ApplicationUser>()
            {
                Subject = "Codice di sicurezza",
                BodyFormat = "Salve, " + "il codice da inserire è: {0}"
            });

            //manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                   new DataProtectorTokenProvider<ApplicationUser>
                      (dataProtectionProvider.Create("Gestioni Dirette -  powered by SystemWeb"))
                   {
                       TokenLifespan = TimeSpan.FromHours(3)
                   };
            }
            return manager;
        }
    }
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager)
        { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }/*
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var config = WebConfigurationManager.OpenWebConfiguration("Web.config");
            var settings = config.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;
            var port = settings.Smtp.Network.Port;
            var host = settings.Smtp.Network.Host;
            var username = settings.Smtp.Network.UserName;
            var password = settings.Smtp.Network.Password;

            SmtpClient client = new SmtpClient(host, port);
            client.Credentials = new System.Net.NetworkCredential(username, password);
            var msg = new MailMessage();
            msg.From = new MailAddress("administrator@gestionidirette.com");
            msg.To.Add(message.Destination);
            msg.Subject = message.Subject;
            msg.Body = message.Body;

            client.Timeout = 1000;

            var t = Task.Run(() => client.SendAsync(msg, null));

            return t;
        }
    }*/

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {

            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }

    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new ApplicationRoleStore(context.Get<MyDbContext>()));
        }

    }
}
