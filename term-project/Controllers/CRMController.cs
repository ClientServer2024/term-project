using Microsoft.AspNetCore.Mvc;

namespace term_project.Controllers
{
  public class CRMController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult CRMLanding()
    {
      return View("~/Views/CRMView/CRMLanding.cshtml");
    }
  }
}
