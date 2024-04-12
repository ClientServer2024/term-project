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
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using term_project.Models.HRModels;

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

            //var employeeQualifications= new List<List<String>>();
            //foreach (var employee in employeeList)
            //{

            //    var employeeQualificationResponse = await _supabase
            //        .From<EmployeeQualification>()
            //        .Select("*")
            //        .Where(q => q.EmployeeId == employee.EmployeeId)
            //        .Get();

            //    var qualificationList = employeeQualificationResponse.Models;
            //    var qualificationDescriptions = new List<String>();
            //    foreach (var qualification in qualificationList)
            //    {
            //        var qualificationResponse = await _supabase
            //            .From<Qualification>()
            //            .Select("*")
            //            .Where(q => q.QualificationId == qualification.QualificationId)
            //            .Get();
            //        var qualificationDescriptionList = qualificationResponse.Models;
            //        foreach (var qualificationDescription in qualificationDescriptionList)
            //        {
            //            qualificationDescriptionList.Add(qualificationDescription);
            //        }
            //    }

            //    employeeQualifications.Add(qualificationDescriptions);
            //}

            var JsonData = new
            {
                employeeIDs = employeeIDs,
                employeeFirstNames = employeeFirstNames,
                employeeLastNames = employeeLastNames,
                employeeJobTitles = employeeJobTitles,
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

            var employeeQualifications = new List<String>();
            var employeeQualificationResponse = await _supabase
                .From<EmployeeQualification>()
                .Select("*")
                .Where(q => q.EmployeeId == employeeResponse.EmployeeId)
                .Get();

            var employeeQualificationModel = employeeQualificationResponse.Models;
            foreach (var qualification in employeeQualificationModel)
            {
                var qualificationResponse = await _supabase
                    .From<Qualification>()
                    .Select("*")
                    .Where(q => q.QualificationId == qualification.QualificationId)
                    .Single();

                employeeQualifications.Add(qualificationResponse.QualificationName);
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
                employeeEmail = employeeResponse.Email,
                employeeQualifications = employeeQualifications
            };

            return Json(employeeData);
        }

        public IActionResult HRViewEmployee(Guid employeeID)
        {
            ViewData["EmployeeID"] = employeeID;
            return View("~/Views/HRView/HRViewEmployee.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> EditEmployee(Guid employeeID)
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

        [HttpGet]
        public async Task<IActionResult> HRGetAllShiftsAndEmployees()
        {
            try
            {
                // Fetch all shifts
                var shiftResponse = await _supabase
                    .From<Shift>()
                    .Select("*")
                    .Get();

                var shiftList = shiftResponse.Models;

                // Fetch employees assigned to each shift
                var shiftEmployeeMap = new Dictionary<Guid, List<Guid>>();
                foreach (var shift in shiftList)
                {
                    var employeeShiftResponse = await _supabase
                        .From<EmployeeShift>()
                        .Select("employee_id")
                        .Where(es => es.ShiftId == shift.ShiftId)
                        .Get();

                    var employeeIds = employeeShiftResponse.Models.Select(es => es.EmployeeId).ToList();
                    shiftEmployeeMap.Add(shift.ShiftId, employeeIds);
                }

                // Construct JSON data
                var jsonData = new List<object>();
                foreach (var shift in shiftList)
                {
                    var shiftData = new
                    {
                        ShiftId = shift.ShiftId,
                        ShiftType = shift.ShiftType,
                        StartTime = shift.StartTime,
                        EndTime = shift.EndTime,
                        ShiftDate = shift.ShiftDate,
                        Employees = shiftEmployeeMap.ContainsKey(shift.ShiftId)
                            ? shiftEmployeeMap[shift.ShiftId]
                            : new List<Guid>()
                    };
                    jsonData.Add(shiftData);
                }

                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching shifts and employees: {ex}");
                return StatusCode(500, "An error occurred while fetching shifts and employees.");
            }
        }

        [HttpGet]
        public IActionResult HRShiftModification()
        {
            return View("~/Views/HRView/HRShiftModification.cshtml");
        }

        [HttpGet]
        public IActionResult HREditShift(Guid shiftId)
        {
            ViewData["ShiftID"] = shiftId;
            return PartialView("~/Views/HRView/HREditShift.cshtml");
        }

        [HttpGet]
        public IActionResult HRDeleteShift(Guid shiftId)
        {
            ViewData["ShiftID"] = shiftId;
            return PartialView("~/Views/HRView/HRDeleteShift.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteShift(Guid shiftId)
        {
            try
            {
                // Deleting Shift records from EmployeeShift table
                Console.WriteLine("Deleting Shift records from EmployeeShift table...");
                await _supabase
                    .From<EmployeeShift>()
                    .Where(es => es.ShiftId == shiftId)
                    .Delete();

                // Deleting Shift record from Shift table
                Console.WriteLine("Deleting Shift record from Shift table...");
                await _supabase
                    .From<Shift>()
                    .Where(s => s.ShiftId == shiftId)
                    .Delete();

                return Json(new { redirect = Url.Action("HRManageShifts", "HR") });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while deleting shift: {ex}");
                return BadRequest(ex);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetShift(Guid shiftId)
        {
            try
            {
                // Fetch all shifts
                var shiftResponse = await _supabase
                    .From<Shift>()
                    .Select("*")
                    .Where(s => s.ShiftId == shiftId)
                    .Get();

                var shiftList = shiftResponse.Models;

                // Fetch employees assigned to each shift
                var shiftEmployeeMap = new Dictionary<Guid, List<Guid>>();
                foreach (var shift in shiftList)
                {
                    var employeeShiftResponse = await _supabase
                        .From<EmployeeShift>()
                        .Select("employee_id")
                        .Where(es => es.ShiftId == shift.ShiftId)
                        .Get();

                    var employeeIds = employeeShiftResponse.Models.Select(es => es.EmployeeId).ToList();
                    shiftEmployeeMap.Add(shift.ShiftId, employeeIds);
                }

                // Construct JSON data
                var jsonData = new List<object>();
                foreach (var shift in shiftList)
                {
                    var shiftData = new
                    {
                        ShiftId = shift.ShiftId,
                        ShiftType = shift.ShiftType,
                        StartTime = shift.StartTime,
                        EndTime = shift.EndTime,
                        ShiftDate = shift.ShiftDate,
                        Employees = shiftEmployeeMap.ContainsKey(shift.ShiftId)
                            ? shiftEmployeeMap[shift.ShiftId]
                            : new List<Guid>()
                    };
                    jsonData.Add(shiftData);
                }

                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching shifts and employees: {ex}");
                return StatusCode(500, "An error occurred while fetching shifts and employees.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateShift(Guid shiftId)
        {
            Console.WriteLine("Updating shift table...");

            try
            {
                string requestBody;
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    requestBody = await reader.ReadToEndAsync();
                }

                JObject jsonData = JObject.Parse(requestBody);

                string shiftType = (string)jsonData["shiftType"];
                DateTime shiftDate = (DateTime)jsonData["shiftDate"];
                TimeSpan startTime = (TimeSpan)jsonData["startTime"];
                TimeSpan endTime = (TimeSpan)jsonData["endTime"];
                string assignedEmployeesStr = (string)jsonData["assignedEmployees"];
                string additionalEmployeesStr = (string)jsonData["additionalEmployees"];

                // Parse string values into arrays of GUIDs
                List<Guid> assignedEmployees = ParseGuidArray(assignedEmployeesStr);
                List<Guid> additionalEmployees = ParseGuidArray(additionalEmployeesStr);

                // Combine both lists
                List<Guid> allEmployees = new List<Guid>();
                allEmployees.AddRange(assignedEmployees);
                allEmployees.AddRange(additionalEmployees);

                var updateShift = await _supabase
                    .From<Shift>()
                    .Where(s => s.ShiftId == shiftId)
                    .Set(s => s.ShiftType, shiftType)
                    .Set(s => s.ShiftDate, shiftDate)
                    .Set(s => s.StartTime, startTime)
                    .Set(s => s.EndTime, endTime)
                    .Update();

                Console.WriteLine("Shift Table updated successfully!");
                Console.WriteLine("Updating EmployeeShift Table...");

                await _supabase
                    .From<EmployeeShift>()
                    .Where(es => es.ShiftId == shiftId)
                    .Delete();

                foreach (var emp_id in allEmployees)
                {
                    var employeeShift_Insert = await _supabase
                        .From<EmployeeShift>()
                        .Insert(new EmployeeShift
                        {
                            ShiftId = shiftId,
                            EmployeeId = emp_id
                        });
                }

                Console.WriteLine("EmployeeShift Table records updated successfully!");

                return Json(new { redirect = Url.Action("HRManageShifts", "HR") });
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occurred while updating shift:" + e);
                return BadRequest(e);
            }
        }

        public List<Guid> ParseGuidArray(string input)
        {
            List<Guid> guidList = new List<Guid>();
            if (!string.IsNullOrEmpty(input))
            {
                string[] stringArray = input.Split(',');
                foreach (string str in stringArray)
                {
                    if (Guid.TryParse(str.Trim(), out Guid guid))
                    {
                        guidList.Add(guid);
                    }
                }
            }

            return guidList;
        }

        [HttpGet]
        public async Task<IActionResult> PayHistory(int page = 1, int pageSize = 10)
        {
            // Calculate the offset based on the page number and page size
            int offset = (page - 1) * pageSize;


            // Retrieve PayHistory data with pagination
            var payHistoryResponse = await _supabase
                .From<PayHistory>()
                .Select("*")
                .Range(offset, offset + pageSize - 1) // Specify the range for pagination
                .Get();

            var payHistoryList = payHistoryResponse.Models;

            // Create a list to store pay history entries with employee details
            var payHistoryWithEmployeeDetails = new List<object>();

            foreach (var payrollEntry in payHistoryList)
            {
                // Extract pay history details
                var payRaiseDate = payrollEntry.PayRaiseDate;
                var previousSalaryRate = payrollEntry.PreviousSalaryRate;
                var newSalaryRate = payrollEntry.NewSalaryRate;

                // Extract employee ID
                var employeeId = payrollEntry.EmployeeId;

                // Retrieve employee details
                var employeeResponse = await _supabase
                    .From<Employee>()
                    .Select("first_name, last_name")
                    .Where(employeeResponse => employeeResponse.EmployeeId == employeeId)
                    .Single();

                if (employeeResponse != null)
                {
                    var employee = employeeResponse;

                    // Extract employee details
                    var employeeFirstName = employee.FirstName;
                    var employeeLastName = employee.LastName;

                    // Create an anonymous object with pay history and employee details
                    var payHistoryEntry = new
                    {
                        EmployeeFirstName = employeeFirstName,
                        EmployeeLastName = employeeLastName,
                        PayRaiseDate = payRaiseDate,
                        PreviousSalaryRate = previousSalaryRate,
                        NewSalaryRate = newSalaryRate
                    };

                    // Add the pay history entry to the list
                    payHistoryWithEmployeeDetails.Add(payHistoryEntry);
                }
            }

            // Serialize the list to JSON and return it
            var jsonData = JsonConvert.SerializeObject(payHistoryWithEmployeeDetails);
            return Content(jsonData, "application/json");
        }

        public IActionResult HRPayHistory()
        {
            return View("~/Views/HRView/HRPayHistory.cshtml");
        }


        [HttpPost]
        public async Task<IActionResult> GetAttendanceRecords(string email, string password)
        {
            JObject jsonToBeVerified = await ReadAndParseRequestBodyAsync();

            string employeeEmail = (string)jsonToBeVerified["email"];
            string employeePassword = (string)jsonToBeVerified["password"];

            var employeeId = await AuthenticateEmployeeAsync(employeeEmail, employeePassword);

            if (employeeId == null)
            {
                return Json(new { error = "Invalid email or password" });
            }

            var attendanceResponse = await _supabase
                .From<Attendance>()
                .Select("*")
                .Where(a => a.EmployeeId == employeeId)
                .Get();

            var attendanceRecords = attendanceResponse.Models;

            if (attendanceRecords == null || !attendanceRecords.Any())
            {
                return Json(new { error = "No attendance records found for the employee" });
            }

            var jsonData = JsonConvert.SerializeObject(attendanceRecords);
            return Content(jsonData, "application/json");
        }

        public IActionResult HREmployeeAttendance()
        {
            return View("~/Views/HRView/HREmployeeAttendance.cshtml");
        }


        [HttpPost]
        public async Task<IActionResult> ManageEmployeeAttendanceRecords()
        {
            JObject jsonToBeVerified = await ReadAndParseRequestBodyAsync();

            string employeeEmail = (string)jsonToBeVerified["email"];
            string employeePassword = (string)jsonToBeVerified["password"];

            var employeeId = await AuthenticateEmployeeAsync(employeeEmail, employeePassword);

            if (employeeId == null)
            {
                return Json(new { error = "Invalid email or password" });
            }

            var managerIDResponse = await _supabase
                .From<Manager>()
                .Select("manager_id")
                .Where(m => m.EmployeeId == employeeId)
                .Single();

            var managerID = managerIDResponse?.ManagerId;

            if (managerID == null)
            {
                return Json(new { error = "Employee is not a manager" });
            }

            var managedEmployeeIdsResponse = await _supabase
                .From<Employee>()
                .Select("*")
                .Where(e => e.ManagerId == managerID)
                .Get();

            var managedEmployeeIds = managedEmployeeIdsResponse.Models?
                .Select(e => e.EmployeeId)
                .ToList();

            if (managedEmployeeIds == null || !managedEmployeeIds.Any())
            {
                return Json(new { error = "No managed employees found" });
            }

            var employeeAttendanceData = new List<object>();

            foreach (var managedEmployeeId in managedEmployeeIds)
            {
                var employeeInfoResponse = await _supabase
                    .From<Employee>()
                    .Select("first_name, last_name")
                    .Where(e => e.EmployeeId == managedEmployeeId)
                    .Single();

                if (employeeInfoResponse == null)
                {
                    return Json(new { error = "Employee information not found for ID: " + managedEmployeeId });
                }

                string firstName = (string)employeeInfoResponse.FirstName;
                string lastName = (string)employeeInfoResponse.LastName;

                var attendanceResponse = await _supabase
                    .From<Attendance>()
                    .Select("*")
                    .Where(a => a.EmployeeId == managedEmployeeId)
                    .Get();

                var attendanceRecords = attendanceResponse.Models;

                var employeeData = new
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Records = attendanceRecords
                };

                employeeAttendanceData.Add(employeeData);
            }

            var jsonData = JsonConvert.SerializeObject(employeeAttendanceData);
            return Content(jsonData, "application/json");
        }


        private async Task<Guid?> AuthenticateEmployeeAsync(string email, string password)
        {
            var employeeResponse = await _supabase
                .From<Employee>()
                .Select("employee_id, password")
                .Where(e => e.Email == email)
                .Single();

            if (employeeResponse == null)
            {
                return null; // Email not found
            }

            var storedPassword = (string)employeeResponse.Password;

            if (password != storedPassword)
            {
                return null; // Incorrect password
            }

            return (Guid)employeeResponse.EmployeeId; // Return the employee ID if authentication is successful
        }

        private async Task<JObject> ReadAndParseRequestBodyAsync()
        {
            string requestBody;
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            return JObject.Parse(requestBody);
        }


        public IActionResult HRManageAttendance()
        {
            return View("~/Views/HRView/HRManageAttendance.cshtml");
        }


        public IActionResult HREditAttendance(string record)
        {
            ViewData["Title"] = "Edit Attendance Record";

            // Deserialize the record data into an Attendance object
            var recordData = JsonConvert.DeserializeObject<Attendance>(record);

            // Pass the Attendance object to the view
            return View("~/Views/HRView/HREditAttendance.cshtml", recordData);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateAttendanceRecord(Guid employeeId, DateTime? clockInTime,
            DateTime? clockOutTime, DateTime date, string status, DateTime? overTimeStart, DateTime? overTimeEnd)
        {
            try
            {
                await _supabase
                    .From<Attendance>()
                    .Where(a => a.EmployeeId == employeeId)
                    .Set(a => a.ClockInTime, clockInTime)
                    .Set(a => a.ClockOutTime, clockOutTime)
                    .Set(a => a.Date, date)
                    .Set(a => a.Status, status)
                    .Set(a => a.OverTimeStart, overTimeStart)
                    .Set(a => a.OverTimeEnd, overTimeEnd)
                    .Update();

                return Json(new { redirect = Url.Action("HRManageAttendance", "HR") });
            }
            catch (Exception ex)
            {
                return Json(new
                    { error = "An error occurred while updating the attendance record: " + ex.Message });
            }
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
            Console.WriteLine(
                $"{methodName}: Redirecting to payroll generation with employee id [{employeeId}]...");

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
        public async Task<IActionResult> HrPayroll_GeneratePayroll_PostInfo(Guid employeeId,
            DateTime payPeriodStart,
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
        public async Task<IActionResult> HrPayroll_GeneratePayroll_PostPayroll(Guid employeeId,
            DateTime payPeriodStart,
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
        private async IAsyncEnumerable<IList<Shift>> HrPayroll_FetchShifts(List<Guid> shiftIds,
            DateTime payPeriodStart,
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
                throw new KeyNotFoundException(
                    $"{methodName}-Exception: Attendances with employee id [{employeeId}] " +
                    $"from [{payPeriodStart}] to [{payPeriodEnd}] was not found.");
            }

            return attendances;
        }
    }
}
