using System.Web;
using System.Web.Mvc;

namespace GestioniDirette.ActionFilters
{
    public class SearchBotFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Request.Browser.Crawler)
            {
                filterContext.Result = new ViewResult() { ViewName = "NotFound" };
            }
        }
    }
}