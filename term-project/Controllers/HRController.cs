using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using term_project.Models.CareModels;
using dotenv.net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Supabase;
using term_project.Models.CRMModels;
using Guid = System.Guid;
using System.IO.Compression;

namespace term_project.Controllers
{
    public class HRController : Controller
    {
        private readonly Supabase.Client _supabase;

        public HRController()
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

        public async Task<IActionResult> HRLanding()
        {
            return View("~/Views/HRView/HRLanding.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employeeResponse = await _supabase
                .From<Employee>()
                .Select("*")
                .Get();

            var employeeList = employeeResponse.Models;

            var employeeIDs = new List<Guid>();
            foreach (var employee in employeeList)
            {
                employeeIDs.Add(employee.EmployeeId);
            }

            var employeeFirstNames = new List<string>();
            foreach (var employee in employeeList)
            {
                employeeFirstNames.Add(employee.FirstName);
            }

            var employeeLastNames = new List<string>();
            foreach (var employee in employeeList)
            {
                employeeLastNames.Add(employee.LastName);
            }

            var employeeJobTitles = new List<string>();
            foreach (var employee in employeeList)
            {
                employeeJobTitles.Add(employee.JobTitle);
            }

            var JsonData = new
            {
                employeeIDs = employeeIDs,
                employeeFirstNames = employeeFirstNames,
                employeeLastNames = employeeLastNames,
                employeeJobTitles = employeeJobTitles
            };

            return Json(JsonData);
        }   

        public IActionResult HRManageEmployees()
        {
            return View("~/Views/HRView/HRManageEmployees.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployee(Guid employeeID)
        {
            Console.WriteLine("ID: " + employeeID.ToString());
            var employeeResponse = await _supabase
                .From<Employee>()
                .Select("*")
                .Where(e => e.EmployeeId == employeeID)
                .Single();

            if (employeeResponse == null)
            {
                return Json(new { error = "Employee not found" });
            }

            var employeeData = new
            {
                employeeFirstName = employeeResponse.FirstName,
                employeeLastName = employeeResponse.LastName,
                employeeAddress = employeeResponse.Address,
                employeeEmergencyContact = employeeResponse.EmergencyContact,
                employeeJobTitle = employeeResponse.JobTitle,
                employeeEmploymentType = employeeResponse.EmploymentType,
                employeeSalaryRate = employeeResponse.SalaryRate,
                employeeEmail = employeeResponse.Email
            };

            return Json(employeeData);
        }

        public IActionResult HRViewEmployee(Guid employeeID)
        {
            ViewData["EmployeeID"] = employeeID;
            return View("~/Views/HRView/HRViewEmployee.cshtml");
        }

        [HttpPost]
        public async Task <IActionResult> EditEmployee(Guid employeeID)
        {
            string requestBody;
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            JObject jsonData = JObject.Parse(requestBody);

            Console.WriteLine(jsonData.ToString());

            string firstName = (string)jsonData["employeefirstName"];
            string lastName = (string)jsonData["employeelastName"];
            string address = (string)jsonData["employeeaddress"];
            string emergencyContact = (string)jsonData["employeeEmergencyContact"];
            string jobTitle = (string)jsonData["employeeJobTitle"];
            string employmentType = (string)jsonData["employeeEmploymentType"];
            string salaryRateString = (string)jsonData["employeeSalaryRate"];
            string email = (string)jsonData["employeeEmail"];


            if (float.TryParse(salaryRateString, out float salaryRate))
            {
                var update = await _supabase
                    .From<Employee>()
                    .Where(e => e.EmployeeId == employeeID)
                    .Set(e => e.FirstName, firstName)
                    .Set(e => e.LastName, lastName)
                    .Set(e => e.Address, address)
                    .Set(e => e.EmergencyContact, emergencyContact)
                    .Set(e => e.JobTitle, jobTitle)
                    .Set(e => e.EmploymentType, employmentType)
                    .Set(e => e.SalaryRate, salaryRate)
                    .Set(e => e.Email, email)
                    .Update();

                return Json(new { redirect = Url.Action("HRManageEmployees", "HR") });
            }
            else
            {
                // Return an error response
                return BadRequest("Failed to parse salary rate.");
            }
        }

        public IActionResult HREditEmployee(Guid employeeID)
        {
            ViewData["EmployeeID"] = employeeID;
            return View("~/Views/HRView/HREditEmployee.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(Guid employeeID)
        {
            var deleteEntry = _supabase
                .From<Employee>()
                .Where(e => e.EmployeeId == employeeID)
                .Delete();

            return Json(new { redirect = Url.Action("HRManageEmployees", "HR") });
        }

        public IActionResult HRDeleteEmployee(Guid employeeID)
        {
            ViewData["EmployeeID"] = employeeID;
            return View("~/Views/HRView/HRDeleteEmployee.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee()
        {
            string requestBody;
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            JObject jsonData = JObject.Parse(requestBody);

            Console.WriteLine(jsonData.ToString());

            string firstName = (string)jsonData["employeefirstName"];
            string lastName = (string)jsonData["employeelastName"];
            string address = (string)jsonData["employeeaddress"];
            string emergencyContact = (string)jsonData["employeeEmergencyContact"];
            string jobTitle = (string)jsonData["employeeJobTitle"];
            string employmentType = (string)jsonData["employeeEmploymentType"];
            string salaryRateString = (string)jsonData["employeeSalaryRate"];
            string email = (string)jsonData["employeeEmail"];

            if (float.TryParse(salaryRateString, out float salaryRate))
            {

                var model = new Employee
                {
                    EmployeeId = Guid.NewGuid(),
                    FirstName = firstName,
                    LastName = lastName,
                    Address = address,
                    EmergencyContact = emergencyContact,
                    JobTitle = jobTitle,
                    EmploymentType = employmentType,
                    SalaryRate = salaryRate,
                    Email = email
                };

                await _supabase.From<Employee>().Insert(model);

                return Json(new { redirect = Url.Action("HRManageEmployees", "HR") });
            }
            else
            {
                // Return an error response
                return BadRequest("Failed to parse salary rate.");
            }
        }

        public IActionResult HRCreateEmployee()
        {
            return View("~/Views/HRView/HRCreateEmployee.cshtml");
        }
        
        /** Shift Management */
        public IActionResult HRManageShifts()
        {
            return View("~/Views/HRView/HRManageShifts.cshtml");
        }

        public IActionResult HRCreateShift()
        {
            return View("~/Views/HRView/HRCreateShift.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateShift()
        {
            Console.WriteLine("Creating shift...");
            
            try
            {
                string requestBody;
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
                JObject jsonData = JObject.Parse(requestBody);
                Console.WriteLine(jsonData.ToString());

                string shiftType = (string)jsonData["shiftType"];
                DateTime shiftDate = (DateTime)jsonData["shiftDate"];
                TimeSpan startTime = (TimeSpan)jsonData["startTime"];
                TimeSpan endTime = (TimeSpan)jsonData["endTime"];
                
                var new_shift = new Shift
                {
                    ShiftId = Guid.NewGuid(),
                    ShiftType = shiftType,
                    StartTime = startTime,
                    EndTime = endTime,
                    ShiftDate = shiftDate
                };
                Console.WriteLine(new_shift);

                await _supabase.From<Shift>().Insert(new_shift);
                return Json(new { redirect = Url.Action("HRManageShifts", "HR") });
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occurred while creating new shift:" + e);
                return BadRequest(e);
            }
        }
    }
}
