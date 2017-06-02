using System;
using System.Web.Mvc;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Controllers
{
    public class UsersImageController : Controller
    {
        // GET: UsersImage
        private MyDbContext db = new MyDbContext();
        public ActionResult Index(Guid id)
        {
            var fileToRetrieve = db.UsersImage.Find(id);
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }
    }
}