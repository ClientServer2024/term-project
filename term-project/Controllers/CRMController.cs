using dotenv.net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Supabase;
using term_project.Models.CRMModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using Guid = System.Guid;
using System.Diagnostics;
using term_project.Models;


namespace term_project.Controllers
{
    public class CRMController : Controller
    {
        private readonly Supabase.Client _supabase;

        public CRMController()
        {
            DotEnv.Load();

            var supabaseUrl = Environment.GetEnvironmentVariable("Supabase__Url");
            var supabaseKey = Environment.GetEnvironmentVariable("Supabase__Key");

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };
            _supabase = new Supabase.Client(supabaseUrl, supabaseKey, options);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CRMLanding()
        {
            return View("~/Views/CRMView/CRMLanding.cshtml");
        }

        public IActionResult MaintenanceLanding()
        {
            return View("~/Views/CRMView/MaintenanceLanding.cshtml");
        }


        [HttpGet]
        public async Task<IActionResult> GetAssetsForRenter(Guid renterId)
        {
            Guid specificRenterId = new Guid("6b161358-25c1-4bb5-b29f-5bf7d1117d47");
            Guid specificAssetId = new Guid("c8a5ca4d-8bce-4d0c-b169-d15724120f09");

            if (renterId == specificRenterId)
            {
                var assetsResponse = await _supabase
                    .From<Asset>()
                    .Select("*")
                    .Get();

                var allAssets = assetsResponse.Models;

                var specificAsset = allAssets.FirstOrDefault(a => a.AssetId == specificAssetId);

                if (specificAsset != null)
                {
                    return Json(new[] { specificAsset });
                }
            }

            return Json(new object[] { });
        }


        // GET: CRM/CreateRequest
        public async Task<IActionResult> CreateRequest()
        {
            var rentersResponse = await _supabase.From<Renter>().Select("renter_id").Get();
            ViewBag.Renters = rentersResponse.Models;

            return View("~/Views/CRMView/CreateRequest.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(MaintenanceRequest model)
        {
            try
            {
                var insertResponse = await _supabase.From<MaintenanceRequest>().Insert(model);

                return RedirectToAction("MaintenanceLanding");
            }
            catch (Exception ex)
            {
                var rentersResponse = await _supabase.From<Renter>().Select("renter_id").Get();
                ViewBag.Renters = rentersResponse.Models;

                var errorViewModel = new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    ErrorMessage = ex.Message
                };
                return View("~/Views/CRMView/CreateRequest.cshtml", model);
            }
        }

        public async Task<IActionResult> ViewRequests()
        {
            try
            {
                var response = await _supabase.From<MaintenanceRequest>()
                                              .Select("*")
                                              .Get();
                var requests = response.Models;

                return View("~/Views/CRMView/ViewRequests.cshtml", requests);
            }
            catch (Exception ex)
            {
                return View("Error", new { ErrorMessage = ex.Message });
            }
        }


    }
}