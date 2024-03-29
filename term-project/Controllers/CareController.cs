using Microsoft.AspNetCore.Mvc;

namespace term_project.Controllers
{
    public class CareController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
