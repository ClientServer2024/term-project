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

        public CRMController() // This was corrected from 'CareController' to 'CRMController'
        {
            // Load environment variables
            DotEnv.Load();

            // Get Supabase configuration from environment variables
            var supabaseUrl = Environment.GetEnvironmentVariable("Supabase__Url");
            var supabaseKey = Environment.GetEnvironmentVariable("Supabase__Key");

            // Initialize Supabase client
            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true // Optional: enables real-time functionality
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

        public async Task<IActionResult> ManageAssets()
        {
            try
            {
                // Fetch assets with specific fields: id, type, status, and rate
                var response = await _supabase.From<Asset>()
                                              .Select("asset_id, type, status, rate")
                                              .Get();
                var assets = response.Models;

                // Specify the correct path to the ManageAssets view
                return View("~/Views/CRMView/ManageAssets.cshtml", assets);
            }
            catch (Exception ex)
            {
                return View("Error", new { ErrorMessage = ex.Message });
            }
        }

        public async Task<IActionResult> AvailableAssets()
        {
            try
            {
                // Fetch only assets that are marked as available
                var availableAssetsResponse = await _supabase
                    .From<Asset>()
                    .Select("*")
                    .Where(asset => asset.Status == "Available")
                    .Get();

                var availableAssets = availableAssetsResponse.Models;

                // Check if the view for available assets exists and return the model to that view
                return View("~/Views/CRMView/AvailableAssets.cshtml", availableAssets);
            }
            catch (Exception ex)
            {
                // Log the error message and return the Error view
                return View("Error", new { ErrorMessage = ex.Message });
            }
        }




    }
}
