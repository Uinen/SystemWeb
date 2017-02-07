using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SystemWeb.Models;

namespace SystemWeb.ViewModels
{
    [Serializable]
    public class RegistrationViewModel
    {
        public int? WizardID { get; set; }

        public string WizardType { get; set; }

        public Step1ViewModel step1 { get; set; }
        public Step2ViewModel step2 { get; set; }
        public Step3ViewModel step3 { get; set; }
        public Step3_1ViewModel step3_1 { get; set; }
        public Step4ViewModel step4 { get; set; }

        public bool IsNew
        {
            get
            {
                return WizardID.HasValue;
            }
        }
    }

    [Serializable]
    public class Step1ViewModel 
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
    }

    [Serializable]
    public class Step2ViewModel 
    {
        [Display(Name = "Nome")]
        public string mProfileName { get; set; }

        [Display(Name = "Cognome")]
        public string mProfileSurname { get; set; }

        [Display(Name = "Indirizzo")]
        public string mProfileAdress { get; set; }

        [Display(Name = "Città")]
        public string mProfileCity { get; set; }

        [Display(Name = "Codice Avviamento Postale")]
        public int mProfilezipCode { get; set; }

        [Display(Name = "Nazione")]
        public string mProfileNation { get; set; }

        [Display(Name = "Stato Utente")]
        public string mProfileInfo { get; set; }
    }

    [Serializable]
    public class Step3ViewModel 
    {
        [Display(Name = "Azienda")]
        public string name { get; set; }

        [Display(Name = "Partita Iva")]
        public int iva { get; set; }

        [Key]
        [Display(Name = "Ragione Sociale")]
        public Guid RagioneSocialeId { get; set; }
        public virtual RagioneSociale RagioneSociale { get; set; }
    }

    [Serializable]
    public class Step3_1ViewModel 
    {
        [Key]
        [Display(Name = "Seleziona la tua azienda")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }

    [Serializable]
    public class Step4ViewModel 
    {

        [Display(Name = "Nome Punto Vendita")]
        public string PvName { get; set; }

        [Key]
        [Display(Name = "Bandiera")]
        public Guid PvFlagId { get; set; }

        public virtual Pv Pv { get; set; }
    }
}