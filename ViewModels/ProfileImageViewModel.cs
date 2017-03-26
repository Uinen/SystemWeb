using System;
using System.Linq;

namespace SystemWeb.ViewModels
{
    public class ProfileImageViewModel
    {
        public string IMAGEPATH { get; set; }
    }

    public class ApplicationUserImageViewModel
    {
        public string ID { get; set; }
        public IQueryable <Guid?> USERSIMAGEID { get; set; }
    }
}