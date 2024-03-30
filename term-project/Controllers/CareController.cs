using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using term_project.Models.CareModels;
using dotenv.net;
using Supabase;

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

                // Pass services data to the view
                ViewBag.Services = services;

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
        
                foreach (var employee in employeeResponseList)
                {
                    employeeNames.Add(employee.FirstName); // Add each employee name to the list
                }
                
                return Json(employeeNames);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
