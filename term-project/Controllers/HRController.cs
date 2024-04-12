using System.Data;
using System.Runtime.InteropServices.JavaScript;
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
        [HttpGet]
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
        [HttpGet]
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

            var payrolls = payrollResponse.Models;
            
            if (payrolls.Count <= 0)
            {
                throw new KeyNotFoundException("");
            }

            return payrolls;
        }


        // GET: HR/HrPayroll_GeneratePayroll?employeeId={employeeId}
        [HttpGet]
        public async Task<IActionResult> HrPayroll_GeneratePayroll(Guid employeeId)
        {
            const string methodName = "HrPayroll_GeneratePayroll";
            Console.WriteLine($"{methodName}: Redirecting to payroll generation with employee id [{employeeId}]...");

            try
            {
                var payPeriodStart = DateTime.Now.AddDays(-14).Date.ToString().Split(" ")[0];
                var payPeriodEnd = DateTime.Now.Date.ToString().Split(" ")[0];
                TempData["PayPeriodStart"] = payPeriodStart;
                TempData["PayPeriodEnd"] = payPeriodEnd;
                TempData["EmployeeId"] = employeeId;

                return View("~/Views/HRView/HRPayroll_GeneratePayroll.cshtml");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost]

        // POST: HR/HrPayroll_GeneratePayroll_PostInfo
        public async Task<IActionResult> HrPayroll_GeneratePayroll_PostInfo(Guid employeeId, DateTime payPeriodStart,
            DateTime payPeriodEnd)
        {
            const string methodName = "HrPayroll_GeneratePayroll_PostInfo";
            Console.WriteLine(
                $"{methodName}: Fetching shifts and attendances with employee id [{employeeId}] from [{payPeriodStart}] to [{payPeriodEnd}]...");

            try
            {
                var shifts = await HrPayroll_FetchAllShifts(employeeId, payPeriodStart, payPeriodEnd);
                var attendances = await HrPayroll_FetchAttendances(employeeId, payPeriodStart, payPeriodEnd);
                var employee = await HrPayroll_FetchEmployee(employeeId);

                float grossPay = 0f;
                float taxRate = 0.12f; 
                float deduction = 0f;
                float netPay = 0f;
                float salaryRate = employee.SalaryRate!.Value;

                foreach (var attendance in attendances)
                {
                    grossPay += (attendance.ClockOutTime!.Value - attendance.ClockInTime!.Value).Hours * salaryRate;
                    if (attendance.OverTimeStart != null || attendance.OverTimeEnd != null)
                    {
                        grossPay += (attendance.OverTimeEnd!.Value - attendance.OverTimeStart!.Value).Hours * 0.5f;
                    }
                }

                deduction = grossPay * taxRate;
                netPay = grossPay - deduction;

                TempData.Put("Shifts", shifts);
                TempData.Put("Attendances", attendances);
                TempData["SalaryRate"] = salaryRate.ToString();
                TempData["GrossPay"] = grossPay.ToString();
                TempData["TaxRate"] = taxRate.ToString();
                TempData["Deduction"] = deduction.ToString();
                TempData["NetPay"] = netPay.ToString();
                
                ViewData["SalaryRate"] = salaryRate.ToString();
                ViewData["GrossPay"] = grossPay.ToString();
                ViewData["TaxRate"] = taxRate.ToString();
                ViewData["Deduction"] = deduction.ToString();
                ViewData["NetPay"] = netPay.ToString();

                return PartialView("~/Views/HRView/HRPayroll_Partial_GeneratePayroll_Info.cshtml");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // POST: HR/HrPayroll_GeneratePayroll_PostPayroll
        [HttpPost]
        public async Task<IActionResult> HrPayroll_GeneratePayroll_PostPayroll(Guid employeeId, DateTime payPeriodStart,
            DateTime payPeriodEnd, float grossPay, float taxRate, float deduction, float netPay)
        {
            const string methodName = "HrPayroll_GeneratePayroll_PostPayroll";
            Console.WriteLine(
                $"{methodName}: Generating payrolls with employee id [{employeeId}] from [{payPeriodStart}] to [{payPeriodEnd}]...");
            try
            {
                var employee = await HrPayroll_FetchEmployee(employeeId);
                var payroll = new Payroll
                {
                    EmployeeId = employeeId,
                    PayPeriodStart = payPeriodStart,
                    PayPeriodEnd = payPeriodEnd,
                    GrossPay = grossPay,
                    Deductions = deduction,
                    NetPay = netPay,
                    TaxRate = taxRate
                };
                var result = await _supabase.From<Payroll>()
                    .Insert(payroll);

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task<Employee> HrPayroll_FetchEmployee(Guid employeeId)
        {
            const string methodName = "HrPayroll_GeneratePayroll_PostPayroll";
            Console.WriteLine(
                $"{methodName}: Fetching employee with id [{employeeId}]...");

            var employeeResponse = await _supabase
                .From<Employee>()
                .Select("*")
                .Where(m => m.EmployeeId == employeeId)
                .Get();

            var employee = employeeResponse.Model!;

            return employee;
        }


        // GET: HR/HRPayRoll_PayrollInfo?employeeId={employeeId}
        [HttpGet]
        public async Task<IActionResult> HrPayroll_PayrollInfo(Guid payrollId)
        {
            const string methodName = "HrPayroll_PayrollInfo";
            Console.WriteLine($"{methodName}: Searching for payroll data with id [{payrollId}]...");

            try
            {
                var payroll = await HrPayroll_FetchPayroll(payrollId);
                ViewData["PayrollId"] = payrollId;
                ViewData["PayPeriodStart"] = payroll.PayPeriodStart.ToString().Split(" ")[0];
                ViewData["PayPeriodEnd"] = payroll.PayPeriodEnd.ToString().Split(" ")[0];
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
                .Get();

            var employeeShifts = employeeShiftsResponse.Models;
            
            if (employeeShifts.Count <= 0)
            {
                throw new KeyNotFoundException(
                    $"{methodName}-Exception: Shifts with employee id [{employeeId}] was not found.");
            }

            var shiftIds = employeeShifts.Select(employeeShift => employeeShift.ShiftId).ToList();
            var shifts = new List<Shift>();

            await foreach (var fetchedShifts in HrPayroll_FetchShifts(shiftIds, payPeriodStart, payPeriodEnd))
            {
                shifts.AddRange(fetchedShifts);
            }

            return shifts;
        }

        /* Helper method for HrPayroll_FetchAllShifts
         * Fetches shifts from Shift Table asynchronously using the shiftIds.
         * This method returns something immediately once every shifts are found with shift Id.
         */
        private async IAsyncEnumerable<IList<Shift>> HrPayroll_FetchShifts(List<Guid> shiftIds, DateTime payPeriodStart,
            DateTime payPeriodEnd)
        {
            const string methodName = "HrPayroll_FetchShifts";

            foreach (var shiftId in shiftIds)
            {
                Console.WriteLine($"{methodName}: Fetching shift with shift id [{shiftId}]...");
                var shiftResponse = await _supabase
                    .From<Shift>()
                    .Select("*")
                    .Where(m => m.ShiftId == shiftId)
                    .Where(m => m.ShiftDate >= payPeriodStart && m.ShiftDate < payPeriodEnd)
                    .Get();

                var shifts = shiftResponse.Models;

                yield return shifts;
            }
        }

        /* Helper Method for HrPayroll_PayrollInfo.
         * Fetches all attendances that have specific employeeId within given pay period.
         */
        private async Task<IList<Attendance>> HrPayroll_FetchAttendances(Guid employeeId, DateTime payPeriodStart,
            DateTime payPeriodEnd)
        {
            const string methodName = "HrPayroll_FetchAttendances";
            Console.WriteLine($"{methodName}: Fetching attendances with employee id [{employeeId}]" +
                              $"from [{payPeriodStart}] to [{payPeriodEnd}]...");

            var attendanceResponse = await _supabase
                .From<Attendance>()
                .Select("*")
                .Where(m => m.EmployeeId == employeeId)
                .Where(m => m.Date >= payPeriodStart && m.Date < payPeriodEnd)
                .Get();

            var attendances = attendanceResponse.Models;
            
            if (attendances.Count <= 0 || attendances == null)
            {
                throw new KeyNotFoundException($"{methodName}-Exception: Attendances with employee id [{employeeId}] " +
                                               $"from [{payPeriodStart}] to [{payPeriodEnd}] was not found.");
            }

            return attendances;
        }
    }
}