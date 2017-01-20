using SystemWeb.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace SystemWeb.IdentityExtensions
{
    public class MyUserValidation : IIdentityValidator<ApplicationUser>
    {
        public System.Threading.Tasks.Task<IdentityResult> ValidateAsync(ApplicationUser item)
        {
            if (item.UserName.ToLower().Contains("@#*$%£&^§[]{}<>"))
                return Task.FromResult(IdentityResult.Failed("Il nome utente non può contenere i seguenti simboli: @#*$%£&^§[]{}<> "));
            //else if (item.HomeTown.ToLower().Contains("unknown"))
            //    return Task.FromResult(IdentityResult.Failed("HomeTown cannot contain unknown city"));
            else
                return Task.FromResult(IdentityResult.Success);
        }
    }

    public class MyPasswordValidation : IIdentityValidator<string>
    {
        public System.Threading.Tasks.Task<IdentityResult> ValidateAsync(string item)
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
            using (SHA256 mySHA256 = SHA256.Create())
            {
                byte[] hash = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(password.ToString()));

                StringBuilder hashSB = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    hashSB.Append(hash[i].ToString("x2"));
                }
                return hashSB.ToString();
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