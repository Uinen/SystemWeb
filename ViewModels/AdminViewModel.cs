using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SystemWeb.Database.Entity;

namespace SystemWeb.Models
{
    public class AdminIndexViewModel
    {
        public IEnumerable<Product> prodotto { get; set; }
        public IEnumerable<Notice> notizia { get; set; }
        public IEnumerable<Year> year { get; set; }
        public IEnumerable<RagioneSociale> rs { get; set; }
        public IEnumerable<ApplicationRoleStore> ruolo { get; set; }
        public IEnumerable<ApplicationUser> utente { get; set; }
        public IEnumerable<Flag> flag { get; set; }
    }
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Username")]
        [EmailAddress]
        public string Username { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}