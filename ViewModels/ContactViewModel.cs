﻿using System.ComponentModel.DataAnnotations;

namespace GestioniDirette.ViewModels
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