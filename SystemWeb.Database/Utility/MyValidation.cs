using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using SystemWeb.Database.Entity;

namespace SystemWeb.Database.Utility
{
    public class MyUserValidation : IIdentityValidator<ApplicationUser>
    {
        public Task<IdentityResult> ValidateAsync(ApplicationUser item)
        {
            return Task.FromResult(item.UserName.ToLower().Contains("@#*$%£&^§[]{}<>") ? IdentityResult.Failed("Il nome utente non può contenere i seguenti simboli: @#*$%£&^§[]{}<> ") : IdentityResult.Success);
        }
    }

    public class MyPasswordValidation : IIdentityValidator<string>
    {
        public Task<IdentityResult> ValidateAsync(string item)
        {
            if (item.ToLower().Contains("111111"))
                return Task.FromResult(IdentityResult.Failed("La password non può contenere numeri consecutivi, oppure forme EZ"));
            else
                return Task.FromResult(IdentityResult.Success);
        }
    }

    #region Deprecated - version 3.0
    /*
    public class MyPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return password + "testpranav";
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == providedPassword + "testpranav")
                return PasswordVerificationResult.Success;
            else
                return PasswordVerificationResult.Failed;
        }
    }
    */
    #endregion

    public class MyPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            using (var mySha256 = SHA256.Create())
            {
                var hash = mySha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                var hashSb = new StringBuilder();
                foreach (var t in hash)
                {
                    hashSb.Append(t.ToString("x2"));
                }
                return hashSb.ToString();
            }
        }


        public PasswordVerificationResult VerifyHashedPassword(
          string hashedPassword, string providedPassword)
        {
            if (hashedPassword == HashPassword(providedPassword))
                return PasswordVerificationResult.Success;
            else
                return PasswordVerificationResult.Failed;
        }
    }


}