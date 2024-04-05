using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using term_project.Models.CareModels;
using term_project.Models.HRModels;
using dotenv.net;
using Newtonsoft.Json;
using Supabase;
using Guid = System.Guid;

namespace term_project.Controllers
{
  public class CRMController : Controller{
    
    private readonly Client _supabase;

    public CRMController(ILogger<CRMController> logger)
{
    DotEnv.Load();
    var supabaseUrl = Environment.GetEnvironmentVariable("Supabase__Url");
    var supabaseKey = Environment.GetEnvironmentVariable("Supabase__Key");

    logger.LogInformation($"Supabase URL: {supabaseUrl}");
    logger.LogInformation($"Supabase Key: {supabaseKey}");

    var options = new SupabaseOptions
    {
        AutoConnectRealtime = true
    };

    if (supabaseUrl != null)
    {
        _supabase = new Client(supabaseUrl, supabaseKey, options);
    }
    else
    {
        // Handle the case when supabaseUrl is null
        // For example, you can log an error or throw an exception
    }
}


    public IActionResult Index()
    {
      return View();
    }

    public IActionResult CRMLanding()
    {
      return View("~/Views/CRMView/CRMLanding.cshtml");
    }

    // public IActionResult CRM_renters(){
    //   return View("~/Views/CRMView/Renters/CRM_renters.cshtml");
    // }

     public async Task<IActionResult> CRM_renters()
        {
            Console.WriteLine("TESTIONG");
            try
            {
                // Fetch services from the database
                var servicesResponse = await _supabase
                    .From<Service>()
                    .Select("*")
                    .Get();

                // Extract the services data
                var services = servicesResponse.Models;
                
                if (services != null && services.Any())
                {
                    var firstEmployee = services.First();
                    Console.WriteLine(firstEmployee);
                }
                else
                {
                    Console.WriteLine("No Services");
                }

                // Store the services data in TempData
                TempData["Services"] = JsonConvert.SerializeObject(services);

                    return View("~/Views/CRMView/Renters/CRM_renters.cshtml");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    public IActionResult CRM_applicants(){
        return View("~/Views/CRMView/Renters/CRM_listRenters.cshtml");
    }

//   [HttpGet]
//         public async Task<IActionResult> GetAllApplicants()
//         {
//             Console.Write("REACHING INSIDE THE APPLICANT METHOD");

//             var applicantResponse = await _supabase
//                 .From<Applicant>()
//                 .Select("*")
//                 .Get();

//             var applicantResponseList = applicantResponse.Models;

//             var applicantNames = new List<String>();
            
//             foreach (var applicant in applicantResponseList)
//             {
//                 applicantNames.Add(applicant.FirstName);
//             }

//             var renterId = new List<Guid>();

//             var renterResponseList = new List<Renter>();

//             foreach (var renter in applicantResponseList)
//             {
//                 var renterResponse = await _supabase
//                     .From<Renter>()
//                     .Select("*")
//                     .Where(r => r.ApplicantId == renter.ApplicantId)
//                     .Get();
                
//                 renterResponseList.AddRange(renterResponse.Models);
//             }

//             foreach (var renter in renterResponseList)
//             {
//                 renterId.Add(renter.RenterId);
//             }

//             var JsonData = new
//             {
//                 applicantNames = applicantNames,
//                 renterIds = renterId
//             };

//             return Json(JsonData);
//         }

  }
}