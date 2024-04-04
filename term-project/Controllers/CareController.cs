using Microsoft.AspNetCore.Mvc;
using term_project.Models.CareModels;
using dotenv.net;
using Newtonsoft.Json;
using Supabase;
using Guid = System.Guid;

namespace term_project.Controllers
{
    public class CareController : Controller
    {
        private readonly Supabase.Client _supabase;

        public CareController()
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

        // GET: Care
        public IActionResult Index()
        {
            return View();
        }

        // GET: Care/CareLanding
        // GET: Care/CareLanding
        public async Task<IActionResult> CareLanding()
        {
            Console.WriteLine("TESTIONG");
            try
            {
                // Fetch services from the database
                var servicesResponse = await _supabase
                    .From<Service>()
                    .Select("*")
                    .Get();

                // Extract the services data
                var services = servicesResponse.Models;

                if (services != null && services.Any())
                {
                    var firstEmployee = services.First();
                    Console.WriteLine(firstEmployee);
                }
                else
                {
                    Console.WriteLine("No Services");
                }

                // Store the services data in TempData
                TempData["Services"] = JsonConvert.SerializeObject(services);

                return View("~/Views/CareView/CareLanding.cshtml");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableEmployees(Guid serviceId)
        {
            Console.WriteLine("REACHING EMPLOYEE METHOD");
            try
            {
                Console.WriteLine("SERVICE ID: " + serviceId);
                // Fetch available employees for the selected service from the database
                var serviceResponse = await _supabase
                    .From<ServiceSchedule>()
                    .Select("*")
                    .Where(s => s.ServiceId == serviceId)
                    .Get();

                var service = serviceResponse.Models;

                if (service != null && service.Any())
                {
                    var firstEmployee = service.First();
                    Console.WriteLine(firstEmployee);
                }
                else
                {
                    Console.WriteLine("No Services");
                }

                var serviceScheduleResponseList = new List<ServiceScheduleEmployee>();

                foreach (var serviceSchedule in service)
                {
                    var serviceScheduleResponse = await _supabase
                        .From<ServiceScheduleEmployee>()
                        .Select("*")
                        .Where(s => s.ServiceScheduleId == serviceSchedule.ServiceScheduleId)
                        .Get();

                    serviceScheduleResponseList.AddRange(serviceScheduleResponse.Models);
                }

                var employeeResponseList = new List<Employee>();

                foreach (var employee in serviceScheduleResponseList)
                {
                    var employeeResponse = await _supabase
                        .From<Employee>()
                        .Select("*")
                        .Where(s => s.EmployeeId == employee.EmployeeId)
                        .Get();

                    employeeResponseList.AddRange(employeeResponse.Models);
                }

                var employeeNames = new List<string>(); // List to store employee names

                var serviceScheduleId = new List<ServiceScheduleEmployee>();

                foreach (var employee in employeeResponseList)
                {
                    var serviceScheduleResponseId = await _supabase
                        .From<ServiceScheduleEmployee>()
                        .Select("*")
                        .Where(s => s.EmployeeId == employee.EmployeeId)
                        .Get();

                    serviceScheduleId.AddRange(serviceScheduleResponseId.Models);
                }

                foreach (var employee in employeeResponseList)
                {
                    employeeNames.Add(employee.FirstName); // Add each employee name to the list
                }

                var serviceScheduleIdStrings = new List<Guid>();

                foreach (var serviceResponseId in serviceScheduleId)
                {
                    serviceScheduleIdStrings.Add(serviceResponseId.ServiceScheduleEmployeeId);
                }

                // Combine the service schedule IDs and employee names into a list of anonymous objects
                var combinedData = new List<object>();

                for (int i = 0; i < Math.Min(serviceScheduleIdStrings.Count, employeeNames.Count); i++)
                {
                    combinedData.Add(new
                    {
                        ServiceScheduleId = serviceScheduleIdStrings[i],
                        EmployeeName = employeeNames[i]
                    });
                }

                // Serialize the combined data to JSON format
                var jsonOutput = JsonConvert.SerializeObject(combinedData);

                return Content(jsonOutput, "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult EmployeeSelection()
        {
            // Retrieve the services data JSON string from TempData
            var servicesJson = TempData["Services"] as string;

            // Deserialize the JSON string back to a list of Service objects
            var services = JsonConvert.DeserializeObject<List<Service>>(servicesJson);

            // Pass the services data to the view
            ViewBag.Services = services;

            // This action method will render the Employee Selection page
            return View("~/Views/CareView/EmployeeSelection.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterService(Guid serviceScheduleEmployeeId, string customerName)
        {
            try
            {
                // Logic to register the service by updating the SERVICE_REGISTER table
                // You can use Supabase or any other ORM to perform the database operation

                // For example, using Supabase
                var insertResponse = await _supabase
                    .From<ServiceRegister>()
                    .Insert(new ServiceRegister
                    {
                        ServiceScheduleEmployeeId = serviceScheduleEmployeeId,
                        CustomerName = customerName
                    });

                // Service successfully registered
                return Ok("Service registered successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisteredServices()
        {
            var response = await _supabase
                .From<ServiceRegister>()
                .Get();

            var customerNames = new List<String>();

            foreach (var registerService in response.Models)
            {
                customerNames.Add(registerService.CustomerName);
            }

            var serviceScheduleResponseEmployeeList = new List<ServiceScheduleEmployee>();

            foreach (var service in response.Models)
            {
                var serviceResponse = await _supabase
                    .From<ServiceScheduleEmployee>()
                    .Select("*")
                    .Where(s => s.ServiceScheduleEmployeeId == service.ServiceScheduleEmployeeId)
                    .Get();

                serviceScheduleResponseEmployeeList.AddRange(serviceResponse.Models);
            }

            var serviceScheduleResponseList = new List<ServiceSchedule>();

            foreach (var service in serviceScheduleResponseEmployeeList)
            {
                var serviceResponse = await _supabase
                    .From<ServiceSchedule>()
                    .Select("*")
                    .Where(s => s.ServiceScheduleId == service.ServiceScheduleId)
                    .Get();

                serviceScheduleResponseList.AddRange(serviceResponse.Models);
            }

            var scheduleTimes = new List<DateTimeOffset>();
            foreach (var service in serviceScheduleResponseList)
            {
                scheduleTimes.Add(service.Schedule);
            }

            var serviceResponseList = new List<Service>();
            foreach (var service in serviceScheduleResponseList)
            {
                var serviceResponse = await _supabase
                    .From<Service>()
                    .Select("*")
                    .Where(s => s.ServiceId == service.ServiceId)
                    .Get();

                serviceResponseList.AddRange(serviceResponse.Models);
            }

            var serviceNames = new List<String>();
            foreach (var service in serviceResponseList)
            {
                serviceNames.Add(service.ServiceName);
            }

            var employeeResponseList = new List<Employee>();
            foreach (var service in serviceScheduleResponseEmployeeList)
            {
                var serviceResponse = await _supabase
                    .From<Employee>()
                    .Select("*")
                    .Where(s => s.EmployeeId == service.EmployeeId)
                    .Get();

                employeeResponseList.AddRange(serviceResponse.Models);
            }

            var employeeNames = new List<String>();
            foreach (var employee in employeeResponseList)
            {
                employeeNames.Add(employee.FirstName);
            }

            // Create an anonymous object containing the required data
            var jsonData = new
            {
                EmployeeNames = employeeNames,
                CustomerNames = customerNames,
                ServiceNames = serviceNames,
                ServiceScheduleTimes = scheduleTimes
            };

            Console.WriteLine("ABOUT TO RETURN JSON");

            return Json(jsonData);
        }

        public IActionResult RegisteredServicesView()
        {
            return View("~/Views/CareView/RegisteredServices.cshtml");
        }

        public IActionResult CreateServiceView()
        {
            return View("~/Views/CareView/CreateService.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateService(string serviceName, float serviceRate)
        {
            try
            {
                await _supabase.From<Service>().Insert(new Service
                {
                    ServiceName = serviceName,
                    Rate = serviceRate
                });

                return Ok("Service created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> AddQualificationToServiceView()
        {
            var services = await _supabase.From<Service>().Select("*").Get();
            ViewBag.Services = services.Models;

            var qualifications = await _supabase.From<Qualification>().Select("*").Get();
            ViewBag.Qualifications = qualifications.Models;
            
            return View("~/Views/CareView/AddQualificationToServiceView.cshtml");
        }
        
        [HttpPost]
        public async Task<IActionResult> AddQualificationToService(Guid serviceId, Guid qualificationId)
        {
            try
            {
                await _supabase.From<ServiceQualification>().Insert(new ServiceQualification
                {
                    ServiceId = serviceId,
                    QualificationId = qualificationId
                });

                return Ok("Qualification added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}