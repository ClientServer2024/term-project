using dotenv.net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Supabase;
using term_project.Models.CRMModels;
using Guid = System.Guid;

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

    public IActionResult CreateRequestView()
    {
      return View("~/Views/CRMView/CreateRequest.cshtml");
    }

    [HttpGet]
    public async Task<IActionResult> CreateRequest()
    {
      try
      {
        var rentersResponse = await _supabase.From<Renter>().Select("renter_id, applicant_id").Get();
        var renterApplicantIds = rentersResponse.Models.Select(r => r.ApplicantId).Distinct().ToList();

        var allApplicantsResponse = await _supabase.From<Applicant>().Select("applicant_id, email").Get();
        var applicableApplicants = allApplicantsResponse.Models
                                                        .Where(a => renterApplicantIds.Contains(a.ApplicantId))
                                                        .ToList();

        var renterEmails = applicableApplicants.ToDictionary(
            app => app.ApplicantId,
            app => app.Email
        );

        ViewBag.RenterEmails = renterEmails.Values.ToList();

        return View("~/Views/CRMView/CreateRequest.cshtml");
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }


    [HttpPost]
    public async Task<IActionResult> CreateRequest([FromBody] MaintenanceRequest model)
    {
      try
      {
        var applicants = await _supabase.From<Applicant>()
                                         .Select("*")
                                         .Where(a => a.Email == model.CustomerEmail)
                                         .Get();
        var applicant = applicants.Models.FirstOrDefault();

        if (applicant == null)
        {
          return BadRequest("Applicant not found.");
        }

        var renters = await _supabase.From<Renter>()
                                     .Select("*")
                                     .Where(r => r.ApplicantId == applicant.ApplicantId)
                                     .Get();
        var renter = renters.Models.FirstOrDefault();

        if (renter == null)
        {
          return BadRequest("Renter not found.");
        }

        var assets = await _supabase.From<Asset>()
                                    .Select("*")
                                    .Where(a => a.Type == model.AssetType)
                                    .Get();
        var asset = assets.Models.FirstOrDefault();

        if (asset == null)
        {
          return BadRequest("Asset not found.");
        }

        var newMaintenanceRequest = new MaintenanceRequest
        {
          AssetId = asset.AssetId,
          RenterId = renter.RenterId,
          Description = model.Description,
          Status = model.Status,
          DueDate = model.DueDate,
          ApplianceMake = model.ApplianceMake,
          ApplianceModel = model.ApplianceModel,
          CustomerEmail = model.CustomerEmail,
          AssetType = model.AssetType
        };

        var insertResponse = await _supabase.From<MaintenanceRequest>()
                                             .Insert(newMaintenanceRequest);

        return Ok(new { message = "Maintenance request created successfully" });
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    public async Task<IActionResult> ViewRequests()
    {
      try
      {
        // Fetch all maintenance requests
        var response = await _supabase.From<MaintenanceRequest>()
                                      .Select("*")
                                      .Get();
        var requests = response.Models;

        if (requests == null || !requests.Any())
        {
          return View("~/Views/CRMView/ViewRequests.cshtml", new List<MaintenanceRequest>());
        }

        return View("~/Views/CRMView/ViewRequests.cshtml", requests);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllApplicants()
    {
      Console.Write("REACHING INSIDE THE APPLICANT METHOD");

      var applicantResponse = await _supabase
          .From<Applicant>()
          .Select("*")
          .Get();

      var applicantResponseList = applicantResponse.Models;

      var applicantIDs = new List<Guid>();
      var applicantEmails = new List<String>();
      foreach (var applicant in applicantResponseList)
      {
        applicantIDs.Add(applicant.ApplicantId);
        applicantEmails.Add(applicant.Email);
      }

      var renters = new List<Renter>();
      var renterIDs = new List<Guid>();

      foreach (var id in applicantIDs)
      {
        var renterResponse = await _supabase
            .From<Renter>()
            .Select("*")
            .Where(r => r.ApplicantId == id)
            .Get();

        renters.AddRange(renterResponse.Models);
      }

      foreach (var renter in renters)
      {
        renterIDs.Add(renter.RenterId);
      }

      var OPs = new List<OccupancyHistory>();
      var assetIDs = new List<Guid>();

      foreach (var id in renterIDs)
      {
        var OPresponse = await _supabase
            .From<OccupancyHistory>()
            .Select("*")
            .Where(o => o.RenterId == id)
            .Get();
        OPs.AddRange(OPresponse.Models);
      }

      foreach (var OP in OPs)
      {
        assetIDs.Add(OP.AssetId);
      }

      var assets = new List<Asset>();
      var assetTypes = new List<String>();

      foreach (var id in assetIDs)
      {
        var assetResponse = await _supabase
            .From<Asset>()
            .Select("*")
            .Where(a => a.AssetId == id)
            .Get();
        assets.AddRange(assetResponse.Models);
      }

      foreach (var asset in assets)
      {
        assetTypes.Add(asset.Type);
      }

      var JsonData = new
      {
        applicantEmails = applicantEmails
      };

      return Json(JsonData);
    }
    [HttpGet]
    public async Task<IActionResult> GetAvailableAssetTypes(String customerEmail)
    {
      Console.Write("REACHING INSIDE THE ASSET TYPES METHOD");

      try
      {
        var applicantResponse = await _supabase
            .From<Applicant>()
            .Select("*")
            .Where(a => a.Email == customerEmail)
            .Get();

        var applicantResponseList = applicantResponse.Models;

        var applicantIDs = new List<Guid>();
        foreach (var applicant in applicantResponseList)
        {
          applicantIDs.Add(applicant.ApplicantId);
        }

        var renters = new List<Renter>();
        var renterIDs = new List<Guid>();

        foreach (var id in applicantIDs)
        {
          var renterResponse = await _supabase
              .From<Renter>()
              .Select("*")
              .Where(r => r.ApplicantId == id)
              .Get();

          renters.AddRange(renterResponse.Models);
        }

        foreach (var renter in renters)
        {
          renterIDs.Add(renter.RenterId);
        }

        var OPs = new List<OccupancyHistory>();
        var assetIDs = new List<Guid>();

        foreach (var id in renterIDs)
        {
          var OPresponse = await _supabase
              .From<OccupancyHistory>()
              .Select("*")
              .Where(o => o.RenterId == id)
              .Get();
          OPs.AddRange(OPresponse.Models);
        }

        foreach (var OP in OPs)
        {
          assetIDs.Add(OP.AssetId);
        }

        var assets = new List<Asset>();
        var assetTypes = new List<String>();

        foreach (var id in assetIDs)
        {
          var assetResponse = await _supabase
              .From<Asset>()
              .Select("*")
              .Where(a => a.AssetId == id)
              .Get();
          assets.AddRange(assetResponse.Models);
        }

        foreach (var asset in assets)
        {
          assetTypes.Add(asset.Type);
        }

        var combinedData = new List<object>();

        for (int i = 0; i < Math.Min(assetTypes.Count, assetIDs.Count); i++)
        {
          combinedData.Add(new
          {
            AssetType = assetTypes[i],
            AssetID = assetIDs[i]
          });
        }

        var jsonOutput = JsonConvert.SerializeObject(combinedData);
        Console.WriteLine(jsonOutput);
        return Content(jsonOutput, "application/json");

      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }


    [HttpPost]
    public async Task<IActionResult> UpdateStatus(Guid maintenanceRequestId, string status)
    {
      try
      {
        var response = await _supabase.From<MaintenanceRequest>()
                                      .Select("*")
                                      .Where(m => m.MaintenanceRequestId == maintenanceRequestId)
                                      .Get();

        var maintenanceRequest = response.Models.FirstOrDefault();

        if (maintenanceRequest == null)
        {
          return NotFound($"Maintenance request with ID {maintenanceRequestId} not found.");
        }

        var updateResponse = await _supabase
                        .From<MaintenanceRequest>()
                        .Where(m => m.MaintenanceRequestId == maintenanceRequestId)
                        .Set(m => m.Status, status)
                        .Update();


        if (updateResponse.ResponseMessage.IsSuccessStatusCode)
        {
          return Ok(new { message = "Status updated successfully." });
        }
        else
        {
          return StatusCode((int)updateResponse.ResponseMessage.StatusCode, updateResponse.ResponseMessage.ReasonPhrase);
        }
      }
      catch (Exception ex)
      {
        // Log the exception and return an error response
        return BadRequest($"An error occurred while updating status: {ex.Message}");
      }
    }



    [HttpGet]
    public async Task<IActionResult> GetAvailableApplianceInfo(Guid assetId, String assetType)
    {
      Console.Write("REACHING INSIDE THE APPLIANCES INFO METHOD");

      try
      {
        var applianceMakes = new List<String>();
        var applianceModels = new List<String>();

        var applianceResponse = await _supabase
            .From<Appliance>()
            .Select("*")
            .Where(a => a.AssetId == assetId)
            .Get();

        var appliances = applianceResponse.Models;

        foreach (var appliance in appliances)
        {
          applianceMakes.Add(appliance.Make);
          applianceModels.Add(appliance.Model);
        }

        var combinedData = new List<object>();

        for (int i = 0; i < Math.Min(applianceMakes.Count, applianceModels.Count); i++)
        {
          combinedData.Add(new
          {
            ApplianceMakes = applianceMakes[i],
            ApplianceModels = applianceModels[i]
          });
        }

        var jsonOutput = JsonConvert.SerializeObject(combinedData);

        return Content(jsonOutput, "application/json");

      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
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

