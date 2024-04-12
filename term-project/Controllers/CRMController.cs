using dotenv.net;
using Microsoft.AspNetCore.Mvc;
using Supabase;
using System;
using System.Threading.Tasks;
using term_project.Models.CRMModels;

namespace term_project.Controllers
{
    public class CRMController : Controller
    {
        private readonly Supabase.Client _supabase;

        public CRMController()
        {
            // Load environment variables
            DotEnv.Load();

            // Get Supabase configuration from environment variables
            var supabaseUrl = Environment.GetEnvironmentVariable("Supabase__Url");
            var supabaseKey = Environment.GetEnvironmentVariable("Supabase__Key");

            // Initialize Supabase client with auto-connect to realtime functionality
            var options = new SupabaseOptions { AutoConnectRealtime = true };
            _supabase = new Supabase.Client(supabaseUrl, supabaseKey, options);
        }

        [Route("CRM/Invoices")]
        public IActionResult Invoices()
        {
            return View("~/Views/CRMView/Invoices.cshtml");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CRMLanding()
        {
            return View("~/Views/CRMView/CRMLanding.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAssets()
        {
            try
            {
                var response = await _supabase.From<Asset>()
                                              .Select("asset_id, type, status, rate")
                                              .Get();

                if (!response.Models.Any())
                {
                    Console.WriteLine("No Assets found.");
                    return NotFound("No assets found.");
                }

                // Prepare the data for the frontend
                var assetDTOs = response.Models.Select(asset => new
                {
                    AssetId = asset.AssetId,
                    Type = asset.Type,
                    Status = asset.Status,
                    Rate = asset.Rate
                }).ToList();

                // Serialize the result to JSON
                return Json(new { success = true, data = assetDTOs });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching assets: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet("GenerateInvoices")]
        public async Task<IActionResult> GenerateInvoices()
        {
            try
            {
                // Fetch all active assets (those linked to an active occupancy without an end date)
                var activeOccupancies = await _supabase.From<OccupancyHistory>()
                    .Select("asset_id, renter_id")
                    .Where(oh => oh.EndDate == null)
                    .Get();

                if (!activeOccupancies.Models.Any())
                {
                    return Json(new { success = false, message = "No active occupancies found." });
                }

                var invoices = new List<object>();

                // Iterate through each active occupancy to compile invoice data
                foreach (var occupancy in activeOccupancies.Models)
                {
                    // Fetch asset details
                    var asset = await _supabase.From<Asset>()
                        .Select("*")
                        .Where(a => a.AssetId == occupancy.AssetId)
                        .Single();

                    // Fetch renter details
                    var renter = await _supabase.From<Renter>()
                        .Select("*, applicant_id (*)")
                        .Where(r => r.RenterId == occupancy.RenterId)
                        .Single();

                    // Fetch applicant details
                    var applicant = await _supabase.From<Applicant>()
                        .Select("*")
                        .Where(a => a.ApplicantId == renter.ApplicantId)
                        .Single();

                    // Create the invoice message
                    string invoiceMessage = $"Hi {applicant.FirstName} {applicant.LastName}, your charge for this month is ${asset.Rate}. Please pay by your bank to the added payee. If you need any assistance please call our office at (dummy info). Thank you.";

                    invoices.Add(new
                    {
                        AssetId = asset.AssetId,
                        RenterName = $"{applicant.FirstName} {applicant.LastName}",
                        AssetType = asset.Type,
                        MonthlyRate = asset.Rate,
                        InvoiceMessage = invoiceMessage
                    });
                }

                // Return the serialized result to JSON
                return Json(new { success = true, data = invoices });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating invoices: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }



        [HttpGet("CRM/AssetDetails/{assetId}")]
        public async Task<IActionResult> GetAppliancesForAsset(Guid assetId)
        {
            try
            {
                var response = await _supabase.From<Appliance>()
                                              .Select("*")
                                              .Where(a => a.AssetId == assetId)
                                              .Get();

                if (response == null)
                {
                    // No appliances found, pass an empty list to the view
                    ViewBag.Appliances = new List<Appliance>();
                }
                else
                {
                    ViewBag.Appliances = response.Models;
                }

                return View("~/Views/CRMView/AssetDetails.cshtml");
            }
            catch (Exception ex)
            {
                // Log the exception here if necessary
                ViewBag.Appliances = new List<Appliance>();  // Ensure ViewBag.Appliances is set even in case of error
                ViewBag.Error = "Failed to load appliances due to an error: " + ex.Message;
                return View("~/Views/CRMView/AssetDetails.cshtml");
            }
        }


        public IActionResult ManageAssets()
{
    return View("~/Views/CRMView/ManageAssets.cshtml");
}




        public async Task<IActionResult> AvailableAssets()
        {
            try
            {
                var availableAssetsResponse = await _supabase
                    .From<Asset>()
                    .Select("*")
                    .Where(a => a.Status == "Available")
                    .Get();

                return View("~/Views/CRMView/AvailableAssets.cshtml", availableAssetsResponse.Models);
            }
            catch (Exception ex)
            {
                return View("Error", new { ErrorMessage = ex.Message });
            }
        }
    }
}
