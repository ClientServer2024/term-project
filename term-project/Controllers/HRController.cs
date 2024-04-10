using Microsoft.AspNetCore.Mvc;
using term_project.Models.CareModels;
using dotenv.net;
using Newtonsoft.Json;
using Supabase;


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
        public async Task<IActionResult> PayHistory()
        {
            try
            {
                // Perform a join query to get pay history along with employee details
                var payHistoryResponse = await _supabase
                    .From<Payroll>()
                    .Select("*, EMPLOYEE(*)") // Include all fields from the Employee table
                    .Get();

                var payHistory = payHistoryResponse.Models.ToList(); // Convert to list for easier manipulation

                // Create a list to hold pay history entries
                var payHistoryWithEmployeeDetails = new List<object>();

                foreach (var payrollEntry in payHistory)
                {
                    // Extract payroll details
                    var payPeriodStart = payrollEntry.PayPeriodStart;
                    var payPeriodEnd = payrollEntry.PayPeriodEnd;
                    var grossPay = payrollEntry.GrossPay;
                    var deductions = payrollEntry.Deductions;
                    var netPay = payrollEntry.NetPay;
                    var taxRate = payrollEntry.TaxRate;

                    // Extract employee details
                    var employeeId = payrollEntry.EmployeeId;
                    var employeeResponse = await _supabase
                        .From<Employee>()
                        .Select("*")
                        .Where( employeeResponse => employeeResponse.EmployeeId == employeeId)
                        .Single();

                    if (employeeResponse == null)
                    {
                        // Skip if employee not found
                        continue;
                    }

                    var employee = employeeResponse;

                    // Check if employee details are available
                    if (employee != null)
                    {
                        var employeeFirstName = employee.FirstName;
                        var employeeLastName = employee.LastName;

                        // Create an anonymous object with pay history and employee details
                        var payHistoryEntry = new
                        {
                            EmployeeFirstName = employeeFirstName,
                            EmployeeLastName = employeeLastName,
                            PayPeriodStart = payPeriodStart,
                            PayPeriodEnd = payPeriodEnd,
                            GrossPay = grossPay,
                            Deductions = deductions,
                            NetPay = netPay,
                            TaxRate = taxRate
                        };

                        // Add the pay history entry to the list
                        payHistoryWithEmployeeDetails.Add(payHistoryEntry);
                    }
                }

                // Serialize the list to JSON and return it


                return Content(JsonConvert.SerializeObject(payHistoryWithEmployeeDetails),"application/json");
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, ex.Message);
            }
        }

        public IActionResult HRPayHistory()
        {
            return View("~/Views/HRView/HRPayHistory.cshtml");
        }


        
    }
}