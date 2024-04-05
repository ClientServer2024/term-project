// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using System;
// using System.Linq;
// using System.Threading.Tasks;
// using term_project.Models.CareModels;
// using Supabase;
// using term_project.Models.CRMModels;

// namespace term_project.Controllers
// {
//   public class CRMController1 : Controller
//   {
//     public IActionResult Index()
//     {
//       return View();
//     }

//     public IActionResult CRMLanding()
//     {
//       return View("~/Views/CRMView/CRMLanding.cshtml");
//     }

//     public IActionResult CRM_renters(){
//       return View("~/Views/CRMView/Renters/CRM_renters.cshtml");
//     }

//      // Action method for the renters landing page
//    public async Task<IActionResult> CRM_listRenters()
// {
//     try
//     {
//         var supabaseUrl = Environment.GetEnvironmentVariable("Supabase__Url");
//         var supabaseKey = Environment.GetEnvironmentVariable("Supabase__Key");

//         if (supabaseUrl != null && supabaseKey != null)
//         {
//             var options = new SupabaseOptions
//             {
//                 AutoConnectRealtime = true
//             };

//             var supabase = new Supabase.Client(supabaseUrl, supabaseKey, options);
//             await supabase.InitializeAsync();

//             var result = await supabase.From<Renter>().Get(); // Changed Employee to Renter
//             var renters = result.Models; // Changed employees to renters

//             ViewBag.Renters = renters;
//         }
//         else
//         {
//             ViewBag.ErrorMessage = "Supabase URL or key is not provided.";
//             return View("Error");
//         }

//         return View("~/Views/CRMView/Renters/CRM_listRenters.cshtml");
//     }
//     catch (Exception ex)
//     {
//         ViewBag.ErrorMessage = $"Error: {ex.Message}";
//         return View("Error");
//     }
// }

//   }
// }
