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

        // GET: HR/HrPayroll_Landing
        public IActionResult HrPayroll_Landing()
        {
            return View("~/Views/HRView/HRPayroll_Landing.cshtml");
        }


        
        // GET: HR/HRPayRoll_Employees?firstName={firstName}&lastName={lastName}
        public async Task<IActionResult> HrPayroll_Employees(string firstName, string lastName)
        {
            const string methodName = "HrPayroll_FetchEmployeeIdWithName";
            Console.WriteLine($"{methodName}: Searching for employee with name [{firstName} {lastName}]...");

            try
            {
                var employees = await HrPayroll_FetchEmployeesWithName(firstName, lastName);

                ViewData["FirstName"] = firstName;
                ViewData["LastName"] = lastName;
                TempData.Put("Employees", employees);

                return View("~/Views/HRView/HRPayRoll_Employees.cshtml");
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e.Message);
                TempData["EmployeeNotFound"] = true;
                TempData["EmployeeName"] = $"{firstName} {lastName}";

                return View("~/Views/HRView/HRPayroll_Landing.cshtml");
            }
        }

        /* Helper Method for HrPayroll_Employees.
         * Fetches employees with the given name.
         */
        private async Task<IList<Employee>> HrPayroll_FetchEmployeesWithName(string firstName, string lastName)
        {
            const string methodName = "HrPayroll_FetchEmployeeIdWithName";
            Console.WriteLine($"{methodName}: Fetching employee ids with name [{firstName} {lastName}]...");

            var response = await _supabase
                .From<Employee>()
                .Select("*")
                .Where(m => m.FirstName == firstName & m.LastName == lastName)
                .Get();

            var employees = response.Models;

            if (employees.Count <= 0)
            {
                throw new KeyNotFoundException(
                    $"{methodName}-Exception: Employee with name [{firstName} {lastName}] was not found.");
            }

            Console.WriteLine($"{methodName}: Total of {employees.Count} are found.");

            return employees;
        }


        
        // GET: HR/HrPayroll_Payrolls?employeeId={employeeId}
        public async Task<IActionResult> HrPayroll_Payrolls(Guid employeeId)
        {
            const string methodName = "HrPayroll_Payrolls";
            Console.WriteLine($"{methodName}: Searching for payrolls of employee with id [{employeeId}]...");

            try
            {
                var payrolls = await HrPayroll_FetchAllPayrolls(employeeId);
                ViewData["EmployeeId"] = employeeId;
                TempData.Put("Payrolls", payrolls);
                return View("~/Views/HRView/HRPayroll_Payrolls.cshtml");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Helper Method for HrPayroll_Payrolls.
        private async Task<IList<Payroll>> HrPayroll_FetchAllPayrolls(Guid employeeId)
        {
            const string methodName = "HrPayroll_FetchAllPayrolls";
            Console.WriteLine($"{methodName}: Fetching payrolls with employee id [{employeeId}]...");

            var payrollResponse = await _supabase
                .From<Payroll>()
                .Select("*")
                .Where(m => m.EmployeeId == employeeId)
                .Get();

            return payrollResponse.Models;
        }


        
        // GET: HR/HRPayRoll_PayrollInfo?employeeId={employeeId}
        public async Task<IActionResult> HrPayroll_PayrollInfo(Guid payrollId)
        {
            const string methodName = "HrPayroll_PayrollInfo";
            Console.WriteLine($"{methodName}: Searching for payroll data with id [{payrollId}]...");

            try
            {
                var payroll = await HrPayroll_FetchPayroll(payrollId);
                ViewData["PayrollId"] = payrollId;
                TempData.Put("Payroll", payroll);
                return View("~/Views/HRView/HRPayroll_PayrollInfo.cshtml");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        // Helper Method for HrPayroll_PayrollInfo.
        private async Task<Payroll> HrPayroll_FetchPayroll(Guid payrollId)
        {
            const string methodName = "HrPayroll_FetchPayroll";
            Console.WriteLine($"{methodName}: Fetching payroll with id [{payrollId}]...");

            var payrollResponse = await _supabase
                .From<Payroll>()
                .Select("*")
                .Where(m => m.PayrollId == payrollId)
                .Get();

            var payroll = payrollResponse.Model;

            if (payroll == null)
                throw new KeyNotFoundException(
                    $"{methodName}-Exception: Payroll was not found with id {payrollId}.");
            
            return payroll;
        }

        /* Helper Method for HrPayroll_PayrollInfo.
         Fetches all shifts that have specific employeeId within given pay period.
         */
        private async Task<IList<Shift>> HrPayroll_FetchAllShifts(Guid employeeId, DateTime payPeriodStart,
            DateTime payPeriodEnd)
        {
            const string methodName = "HrPayroll_FetchAllShifts";
            Console.WriteLine(
                $"{methodName}: Fetching shifts with id [{employeeId}] from [{payPeriodStart}] to [{payPeriodEnd}]...");

            var employeeShiftsResponse = await _supabase
                .From<EmployeeShift>()
                .Select("*")
                .Where(m => m.EmployeeId == employeeId)
                .Where(m => m.Date >= payPeriodStart && m.Date < payPeriodEnd)
                .Get();
            var employeeShifts = employeeShiftsResponse.Models;
            if (employeeShifts.Count <= 0)
            {
                throw new KeyNotFoundException(
                    $"{methodName}-Exception: Shifts with employee id [{employeeId}] was not found.");
            }

            var shiftIds = employeeShifts.Select(employeeShift => employeeShift.ShiftId).ToList();
            var shifts = new List<Shift>();

            await foreach (var fetchedShifts in HrPayroll_FetchShifts(shiftIds))
            {
                shifts.AddRange(fetchedShifts);
            }

            return shifts;
        }

        /* Helper method for HrPayroll_FetchAllShifts
         * Fetches shifts from Shift Table asynchronously using the shiftIds.
         * This method returns something immediately once every shifts are found with shift Id.
         */
        private async IAsyncEnumerable<IList<Shift>> HrPayroll_FetchShifts(List<Guid> shiftIds)
        {
            const string methodName = "HrPayroll_FetchShift";

            foreach (var shiftId in shiftIds)
            {
                Console.WriteLine($"{methodName}: Fetching shift with shift id [{shiftId}]...");
                var shiftResponse = await _supabase
                    .From<Shift>()
                    .Select("*")
                    .Where(m => m.ShiftId == shiftId)
                    .Get();

                yield return shiftResponse.Models;
            }
        }

        /* Helper Method for HrPayroll_PayrollInfo.
         * Fetches all attendances that have specific employeeId.
         */
        private async Task<IList<Attendance>> HrPayroll_FetchAttendances(Guid employeeId)
        {
            const string methodName = "HrPayroll_FetchAttendances";
            Console.WriteLine($"{methodName}: Fetching attendances with employee id [{employeeId}]...");

            var attendanceResponse = await _supabase
                .From<Attendance>()
                .Select("*")
                .Where(m => m.EmployeeId == employeeId)
                .Get();

            return attendanceResponse.Models;
        }
    }
}