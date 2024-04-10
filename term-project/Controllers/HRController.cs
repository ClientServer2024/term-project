using System.Data;
using dotenv.net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Supabase;
using term_project.Models.CareModels;

namespace term_project.Controllers
{
    public class HRController : Controller
    {
        private readonly Supabase.Client _supabase;

        public HRController()
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

        // GET: HR
        public IActionResult Index()
        {
            return View();
        }

        // GET: HR/HRLanding
        public IActionResult HRLanding()
        {
            return View("~/Views/HRView/HRLanding.cshtml");
        }
        
        // GET: HR/HRManagePayrolls
        public IActionResult HrPayroll_Landing()
        {
            return View("~/Views/HRView/HRPayroll_Landing.cshtml");
        }

        public async Task<IActionResult> HrPayroll_Employees(string firstName, string lastName)
        {
            const string methodName = "HrPayroll_FetchEmployeeIdWithName";
            try
            {
                Console.WriteLine($"{methodName}: Searching for employee with the name [{firstName} {lastName}]...");
                var employees = await HrPayroll_FetchEmployeesWithName(firstName, lastName);

                ViewData["FirstName"] = firstName;
                ViewData["LastName"] = lastName;
                TempData.Put("Employees", employees);
                
                return View("~/Views/HRView/HRPayRoll_Employees.cshtml");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public async Task<IActionResult> HrPayroll_PayrollInfo(Guid employeeId)
        {
            const string methodName = "HrPayroll_PayrollInfo";
            
            try
            {
                Console.WriteLine($"{methodName}: Employee Id is {employeeId}.");

                return Ok();

                //return View("~/Views/HRView/HRPayRoll_PayrollInfo.cshtml");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private async Task<IList<Employee>> HrPayroll_FetchEmployeesWithName(string firstName, string lastName)
        { 
            const string methodName = "HrPayroll_FetchEmployeeIdWithName";
            Console.WriteLine($"{methodName}: Fetching employee ids with [{firstName} {lastName}]...");
            try
            {
                var response = await _supabase
                    .From<Employee>()
                    .Select("*")
                    .Where(m => m.FirstName == firstName & m.LastName == lastName)
                    .Get();

                var employees = response.Models;

                if (employees.Count <= 0)
                {
                    throw new KeyNotFoundException($"{methodName}-Exception: Employee with the name [{firstName} {lastName}] was not found.");
                }
                
                Console.WriteLine($"{methodName}: Total of {employees.Count} are found.");
                
                return employees;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}