using System.Web.Mvc;

namespace EnterprisePics.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string message, string errorMessage)
        {
            ViewBag.Message = message;
            ViewBag.ErrorMessage = errorMessage;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}