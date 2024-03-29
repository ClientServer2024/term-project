using Microsoft.AspNetCore.Mvc;

namespace term_project.Controllers
{
  public class CareController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult CareLanding()
    {
      return View("~/Views/CareView/CareLanding.cshtml");
    }
  }
}
