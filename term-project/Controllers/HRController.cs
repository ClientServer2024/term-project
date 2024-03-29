using Microsoft.AspNetCore.Mvc;

namespace term_project.Controllers
{
  public class HRController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult HRLanding()
    {
      return View("~/Views/HRView/HRLanding.cshtml");
    }
  }
}
