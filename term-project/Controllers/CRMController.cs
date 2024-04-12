using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic; // Added for List<object>
using System.Linq;
using term_project.Models.CareModels;
using term_project.Models.HRModels;
using dotenv.net;
// Removed Newtonsoft.Json reference
using Supabase;
using Guid = System.Guid;
using term_project.Models.CRMModels;
using System.Text.Json.Serialization;
using System.Text.Json; // Added for System.Text.Json.JsonSerializer
 using System.Text.Json.Serialization;
using System.Threading.Tasks;




namespace term_project.Controllers
{
  public class CRMController : Controller{
    
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


        _supabase = new Client(supabaseUrl, supabaseKey, options);
  
}

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult CRMLanding()
    {
      return View("~/Views/CRMView/CRMLanding.cshtml");
    }

    public IActionResult CRM_renters(){
      return View("~/Views/CRMView/Renters/CRM_renters.cshtml");
    }

    public IActionResult CRM_listRenters(){
        return View("~/Views/CRMView/Renters/CRM_listRenters.cshtml");
    }
    
    public IActionResult CRM_listapplicants(){
        return View("~/Views/CRMView/Renters/CRM_listapplicants.cshtml");
    }

 

  [HttpPost]
public async Task<IActionResult> AddApplicant(Applicant applicant)
{
    try
    {
        // Generate a unique ID for the applicant
        applicant.ApplicantId = Guid.NewGuid();

        // Ensure status is always set to "Pending"
        applicant.Status = "Pending";

        // Pass the 'applicant' object directly to the Insert method
        await _supabase
            .From<Applicant>()
            .Insert(applicant); // Use the 'applicant' object

        return RedirectToAction("CRM_listapplicants");
    }
    catch (Exception ex)
    {
        ModelState.AddModelError("", "Error adding applicant: " + ex.Message);
        return View(applicant);
    }
}




public async Task<IActionResult> GetAllApplicantsWithIds()
{
    try
    {
        // Fetch applicants from the database
        var applicantResponse = await _supabase
            .From<Applicant>()
            .Select("*")
            .Get();

        var applicantResponseList = applicantResponse.Models;

        var applicantData = new List<object>();
        var applicantIds = new List<Guid>(); // List to store applicant IDs
        var firstNames = new List<string>();
        var lastNames = new List<string>();
        var currentEmployers = new List<string>();
        var incomes = new List<double>();
        var referenceInfos = new List<string>();
        var sharingPeopleInfos = new List<string>();
        var statuses = new List<string>();
        var emails = new List<string>();

        foreach (var applicant in applicantResponseList)
        {
            var applicantStatus = "Pending"; // Default status
            applicantIds.Add(applicant.ApplicantId); // Add applicant ID to the list
            firstNames.Add(applicant.FirstName);
            lastNames.Add(applicant.LastName);
            currentEmployers.Add(applicant.CurrentEmployer);
            incomes.Add(applicant.Income);
            referenceInfos.Add(applicant.ReferenceInfo);
            sharingPeopleInfos.Add(applicant.SharingPeopleInfo);
            statuses.Add(applicant.Status);
            emails.Add(applicant.Email);

            
        }

        var JsonData = new 
        {
            ApplicantIds = applicantIds,
            FirstNames = firstNames,
            LastNames = lastNames,
            CurrentEmployers = currentEmployers,
            Incomes = incomes,
            ReferenceInfos = referenceInfos,
            SharingPeopleInfos = sharingPeopleInfos,
            Statuses = statuses,
            Emails = emails
        };
        
        return Json(JsonData);
    }
    catch (Exception ex)
    {
        // Return a BadRequest response with the exception message
        return BadRequest(ex.Message);
    }
}




[HttpPost]
[Route("/CRM/UpdateApplicantStatus")]
public async Task<IActionResult> UpdateApplicantStatus(Guid applicantId, string newStatus)
{
    try
    {
        // Retrieve the applicant based on the provided ID
        var applicantsResponse = await _supabase
            .From<Applicant>()
            .Select("*")
            .Where(a => a.ApplicantId == applicantId)
            .Get();

        var applicants = applicantsResponse.Models;

        // Check if any applicants were found
        if (applicants == null || applicants.Count == 0)
        {
            return BadRequest($"No applicant found with ID {applicantId}.");
        }

        // Update the status of the first applicant (assuming applicantId is unique)
        var applicant = applicants[0];
        applicant.Status = newStatus;

        // Save the changes to the database
        var updateResponse = await _supabase
            .From<Applicant>()
            .Update(applicant);

    

        return Ok($"Status for applicant with ID {applicantId} updated successfully to {newStatus}.");
    }
    catch (Exception ex)
    {
        // Return an error message if an exception occurs
        return StatusCode(500, $"Error updating applicant status: {ex.Message}");
    }
}

[HttpPost]
[Route("/CRM/CreateRenter")]
public async Task<IActionResult> CreateRenter(Guid applicantId)
{
    try
    {
        // Retrieve the applicant based on the provided ID
        var applicantsResponse = await _supabase
            .From<Applicant>()
            .Select("*")
            .Where(a => a.ApplicantId == applicantId)
            .Get();

        var applicants = applicantsResponse.Models;

        // Check if any applicants were found
        if (applicants == null || applicants.Count == 0)
        {
            return BadRequest($"No applicant found with ID {applicantId}.");
        }

        // Assuming there is only one applicant with this ID, retrieve it
        var applicant = applicants[0];

        // Create a new Renter record based on the applicant's information
        var newRenter = new Renter
        {
            RenterId = Guid.NewGuid(), // Generate a unique ID for the renter
            ApplicantId = applicant.ApplicantId, // Use the same ID as the applicant
            EmergencyContacts = applicant.ReferenceInfo, // Assuming reference info contains emergency contacts
            FamilyDoctor = "John Doe", // Placeholder for family doctor (replace with actual data)
            
        };

        // Insert the new Renter record into the database
        await _supabase
            .From<Renter>()
            .Insert(newRenter);

        return Ok($"Renter created successfully for applicant with ID {applicantId}.");
    }
    catch (Exception ex)
    {
        // Return an error message if an exception occurs
        return StatusCode(500, $"Error creating renter: {ex.Message}");
    }
}





// renter stuff
  [HttpGet]
        public async Task<IActionResult> GetAllRenters()
        {
            try
            {
                var renterResponse = await _supabase
                    .From<Renter>()
                    .Select("*")
                    .Get();

                if (renterResponse == null)
                {
                    return BadRequest("An error occurred while fetching renters: No data returned from the database.");
                }

                var renterResponseList = renterResponse.Models;

                var renterIds = new List<Guid>();
                var applicantIds = new List<Guid>();
                var emergencyContacts = new List<string>();
                var familyDoctors = new List<string>();
                var statuses = new List<string>();

                foreach (var renter in renterResponseList)
                {
                    renterIds.Add(renter.RenterId);
                    applicantIds.Add(renter.ApplicantId);
                    emergencyContacts.Add(renter.EmergencyContacts);
                    familyDoctors.Add(renter.FamilyDoctor);
                    statuses.Add(renter.Status.ToString());
                }

                var jsonData = new
                {
                    RenterIds = renterIds,
                    ApplicantIds = applicantIds,
                    EmergencyContacts = emergencyContacts,
                    FamilyDoctors = familyDoctors,
                    Statuses = statuses
                };

                return Json(jsonData);
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while fetching renters: " + ex.Message);
            }
        }




    }
}