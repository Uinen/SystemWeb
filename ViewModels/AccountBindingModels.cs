using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SystemWeb.Models
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
        [Compare("NewPassword", ErrorMessage = "La nuova password e la password di conferma non corrispondono.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
    {
        [Required]
        [Display(Name = "Nome Utente")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Posta elettronica")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La lunghezza di {0} deve essere di almeno {2} caratteri.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Conferma password")]
        [Compare("Password", ErrorMessage = "La password e la password di conferma non corrispondono.")]
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
        [Display(Name = "Stato Utente")]
        public string mProfileInfo { get; set; }

        [Display(Name = "Azienda")]
        public string name { get; set; }

        [Display(Name = "Partita Iva")]
        public int iva { get; set; }
 
        [Key]
        [Display(Name = "Ragione Sociale")]
        public Guid RagioneSocialeId { get; set; }
        [Required]
        [Key]
        [Display(Name = "Seleziona la tua azienda")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public bool NuovaAzienda { get; set; }
        public bool SelezionaAzienda { get; set; }
        [Required]
        [Display(Name = "Nome Punto Vendita")]
        public string PvName { get; set; }
        [Required]
        [Key]
        [Display(Name = "Bandiera")]
        public Guid PvFlagId { get; set; }
        [Required]
        [Key]
        [Display(Name = "Punto Vendita")]
        public Guid pvID { get; set; }
        public virtual Pv Pv { get; set; }
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
        [Compare("NewPassword", ErrorMessage = "La nuova password e la password di conferma non corrispondono.")]
        public string ConfirmPassword { get; set; }
    }
}
