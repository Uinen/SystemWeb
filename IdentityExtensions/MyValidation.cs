using SystemWeb.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace SystemWeb.IdentityExtensions
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
            if (item.ToLower().Contains("111111") | item.ToLower().Contains("222222") 
                | item.ToLower().Contains("333333") | item.ToLower().Contains("444444")
                | item.ToLower().Contains("555555") | item.ToLower().Contains("666666")
                | item.ToLower().Contains("777777") | item.ToLower().Contains("888888")
                | item.ToLower().Contains("999999") | item.ToLower().Contains("101010")
                | item.ToLower().Contains("123456") | item.ToLower().Contains("654321")
                | item.ToLower().Contains("1234554321") | item.ToLower().Contains("123454321")
                | item.ToLower().Contains("1234512345") | item.ToLower().Contains("5432154321")
                | item.ToLower().Contains("000000") | item.ToLower().Contains("abcdef")
                | item.ToLower().Contains("fedcba"))
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