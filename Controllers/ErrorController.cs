using System.Web.Mvc;

namespace SystemWeb.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error

        public ActionResult Index()
        {
            return View("Error");
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }

        public ActionResult InternalServer()
        {
            Response.StatusCode = 200;
            return View("InternalServer");
        }
    }
}