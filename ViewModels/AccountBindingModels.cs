using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using GestioniDirette.Database.Entity;
using System.Collections;

namespace GestioniDirette.Models
{
    // Modelli utilizzati come parametri per le azioni AccountController.

    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = "Token di accesso esterno")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password corrente")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La lunghezza di {0} deve essere di almeno {2} caratteri.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nuova password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Conferma nuova password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "La nuova password e la password di conferma non corrispondono.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
    {
        [Required]
        [Display(Name = "Nome Utente")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Posta elettronica")]
        public string Email { get; set; }

        public string Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La lunghezza di {0} deve essere di almeno {2} caratteri.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Conferma password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "La password e la password di conferma non corrispondono.")]
        public string ConfirmPassword { get; set; }
        [Required]
        [Display(Name = "Nome")]
        public string mProfileName { get; set; }

        [Required]
        [Display(Name = "Cognome")]
        public string mProfileSurname { get; set; }
        [Required]
        [Display(Name = "Indirizzo")]
        public string mProfileAdress { get; set; }

        [Required]
        [Display(Name = "Città")]
        public string mProfileCity { get; set; }
        [Required]
        [Display(Name = "Codice Avviamento Postale")]
        public int mProfilezipCode { get; set; }

        [Required]
        [Display(Name = "Nazione")]
        public string mProfileNation { get; set; }

        [Required]
        [Display(Name = "Numero Punto Vendita")]
        public string mPvName { get; set; }

        [Required]
        [Display(Name = "Compagnia")]
        public Guid mPvFlagId { get; set; }

        public IEnumerable flag { get; set; }

        public ApplicationUser GetUser()
        {
            var user = new ApplicationUser()
            {
                UserName = Username,
                Email = Email,
                TwoFactorEnabled = false,
                isPremium = false,
                CreateDate = DateTime.Now
            };
            user.UserProfiles = new UserProfiles()
            {
                ProfileName = mProfileName,
                ProfileSurname = mProfileSurname,
                ProfileAdress = mProfileAdress,
                ProfileCity = mProfileCity,
                ProfileZipCode = mProfilezipCode,
                ProfileNation = mProfileNation
            };
            user.Pv = new Pv()
            {
                pvName = mPvName,
                pvFlagId = mPvFlagId
            };
            return user;
        }
    }

    public class RegisterStep2
    {
        [Display(Name = "Numero Punto Vendita")]
        public string mPvName { get; set; }

        [Display(Name = "Indirizzo")]
        public string mIndirizzo { get; set; }

        [Display(Name = "Città")]
        public string mCittà { get; set; }

        [Display(Name = "Cap")]
        public int mCap { get; set; }

        [Display(Name = "Nazione")]
        public string mNazione { get; set; }
        /*
        public PvProfile GetPvProfile()
        {
            var profile = new PvProfile()
            {
                Indirizzo = mIndirizzo,
                Città = mCittà,
                Cap = mCap,
                Nazione = mNazione
            };
            return profile;
        }*/
    }

    public class RegisterExternalBindingModel
    {
        [Required]
        [Display(Name = "Nome utente")]
        public string Username { get; set; }
        [Required]
        [Display(Name = "Posta elettronica")]
        public string Email { get; set; }
    }

    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Provider di accesso")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Chiave provider")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "La lunghezza di {0} deve essere di almeno {2} caratteri.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nuova password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Conferma nuova password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "La nuova password e la password di conferma non corrispondono.")]
        public string ConfirmPassword { get; set; }
    }
}
