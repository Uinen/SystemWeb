using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SystemWeb.ViewModels
{
    public class ContactViewModel
    {
        [Required]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Oggetto")]
        public string Subject { get; set; }
        [Required]
        [Display(Name = "Messaggio")]
        public string Message { get; set; }
    }
}