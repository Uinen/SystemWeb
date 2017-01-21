using System.Web.Mvc;

namespace SystemWeb.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error

        public ActionResult Unknown()
        {
            return View("Unknown");
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }

        public ActionResult InternalServer()
        {
            Response.StatusCode = 500;
            return View("InternalServer");
        }
    }
}