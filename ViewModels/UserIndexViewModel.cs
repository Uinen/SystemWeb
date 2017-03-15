using PagedList;
using System.Collections.Generic;
using System.Linq;

namespace SystemWeb.Models
{
    public class UserIndexViewModel : IPagedList
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
        public List<UsersImage> createuserimage { get; set; }
        public List<UsersImage> showuserimage { get; set; }

        public int FirstItemOnPage
        {
            get;
            set;
        }

        public bool HasNextPage
        {
            get;
            set;
        }

        public bool HasPreviousPage
        {
            get;
            set;
        }

        public bool IsFirstPage
        {
            get;
            set;
        }

        public bool IsLastPage
        {
            get;
            set;
        }

        public int LastItemOnPage
        {
            get;
            set;
        }

        public int PageCount
        {
            get;
            set;
        }

        public int PageNumber
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public int TotalItemCount
        {
            get;
            set;
        }
    }
}