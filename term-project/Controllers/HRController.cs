using Microsoft.AspNetCore.Mvc;

namespace term_project.Controllers
{
    public class HRController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
