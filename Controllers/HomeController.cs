using System.Web.Mvc;
using SystemWeb.Models;

namespace SystemWeb.Controllers
{
    public class HomeController : Controller
    {
        private MyDbContext db = new MyDbContext();

        public ActionResult Confused()
        {
            return View();
        }

        public ActionResult Index()
        {
            ViewData["Message"] = "Benvenuto";
            return View();
        }

    }
}
