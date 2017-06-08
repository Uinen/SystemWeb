using System.Collections.Generic;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Models
{
    public class UserIndexViewModel
    {
        public IEnumerable<Carico> carico { get; set; }
        public IEnumerable<PvErogatori> pverogatori { get; set; }
        public IEnumerable<Pv> pv { get; set; }
        public IEnumerable<PvProfile> pvprofile { get; set; }
        public IEnumerable<PvTank> pvtank { get; set; }
        public IEnumerable<PvTankDesc> pvtankdesc { get; set; }
        public IEnumerable<Dispenser> dispenser { get; set; }
        public IEnumerable<Company> company { get; set; }
        public IEnumerable<CompanyTask> companytask { get; set; }
        public IEnumerable<UserArea> userarea { get; set; }
        public IEnumerable<ApplicationUser> applicationuser { get; set; }
        public IEnumerable<UserProfiles> userprofiles { get; set; }
        public IEnumerable<UsersImage> usersimage { get; set; }
        public IEnumerable<Notice> notice { get; set; }
    }
}