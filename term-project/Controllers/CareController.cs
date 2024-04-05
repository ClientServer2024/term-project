using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using term_project.Models.CareModels;
using dotenv.net;
using Newtonsoft.Json;
using Supabase;
using term_project.Models.CRMModels;
using Guid = System.Guid;

namespace term_project.Controllers
{
    public class CareController : Controller
    {
        private readonly Supabase.Client _supabase;
        private const double TAX = 0.12;

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
        public async Task<IActionResult> GetAllApplicants()
        {
            Console.Write("REACHING INSIDE THE APPLICANT METHOD");

            var applicantResponse = await _supabase
                .From<Applicant>()
                .Select("*")
                .Get();

            var applicantResponseList = applicantResponse.Models;

            var applicantNames = new List<String>();
            
            foreach (var applicant in applicantResponseList)
            {
                applicantNames.Add(applicant.FirstName);
            }

            var renterId = new List<Guid>();

            var renterResponseList = new List<Renter>();

            foreach (var renter in applicantResponseList)
            {
                var renterResponse = await _supabase
                    .From<Renter>()
                    .Select("*")
                    .Where(r => r.ApplicantId == renter.ApplicantId)
                    .Get();
                
                renterResponseList.AddRange(renterResponse.Models);
            }

            foreach (var renter in renterResponseList)
            {
                renterId.Add(renter.RenterId);
            }

            var JsonData = new
            {
                applicantNames = applicantNames,
                renterIds = renterId
            };

            return Json(JsonData);
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

            if (string.IsNullOrEmpty(servicesJson))
            {
                // Handle the case where servicesJson is null or empty
                // You can return an error message or redirect the user to fetch services again
                return RedirectToAction("CareLanding");
            }

            // Deserialize the JSON string back to a list of Service objects
            var services = JsonConvert.DeserializeObject<List<Service>>(servicesJson);

            // Pass the services data to the view
            ViewBag.Services = services;

            // This action method will render the Employee Selection page
            return View("~/Views/CareView/EmployeeSelection.cshtml");
        }

        
        [HttpPost]
        public async Task<IActionResult> RegisterService(Guid serviceScheduleEmployeeId, Guid renterId)
        {
            try
            {
                // Logic to register the service by updating the SERVICE_REGISTER table
                // You can use Supabase or any other ORM to perform the database operation
        
                Console.WriteLine("RENTER ID VALUE: " + renterId);

                var serviceScheduleId = await _supabase
                    .From<ServiceScheduleEmployee>()
                    .Select("*")
                    .Where(s => s.ServiceScheduleEmployeeId == serviceScheduleEmployeeId)
                    .Single();

                var serviceIdTable = await _supabase
                    .From<ServiceSchedule>()
                    .Select("*")
                    .Where(s => s.ServiceScheduleId == serviceScheduleId.ServiceScheduleId)
                    .Single();

                var serviceId = await _supabase
                    .From<Service>()
                    .Select("*")
                    .Where(s => s.ServiceId == serviceIdTable.ServiceId)
                    .Single();
                
                // For example, using Supabase
                var insertResponse = await _supabase
                    .From<ServiceRegister>()
                    .Insert(new ServiceRegister
                    {
                        ServiceScheduleEmployeeId = serviceScheduleEmployeeId,
                        RenterId= renterId,
                        Status= "Not Sent",
                        InvoiceId = Guid.NewGuid(),
                        ServiceId = serviceId.ServiceId
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

            var renterResponseList = new List<Renter>();

            foreach(var customer in response.Models)
            {
                var customerResponse = await _supabase
                    .From<Renter>()
                    .Select("*")
                    .Where(c => c.RenterId == customer.RenterId)
                    .Get();
                
                renterResponseList.AddRange(customerResponse.Models);
            }

            var ApplicantResponseList = new List<Applicant>();

            foreach (var renter in renterResponseList)
            {
                var renterResponse = await _supabase
                    .From<Applicant>()
                    .Select("*")
                    .Where(r => r.ApplicantId == renter.ApplicantId)
                    .Get();
                
                ApplicantResponseList.AddRange(renterResponse.Models);
            }

            foreach (var applicant in ApplicantResponseList)
            {
                customerNames.Add(applicant.FirstName);
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
            
            var serviceRegisterIds = new List<Guid>(); // New list to store ServiceRegisterIds

            foreach (var serviceRegister in response.Models)
            {

                serviceRegisterIds.Add(serviceRegister.ServiceRegisterId); // Add ServiceRegisterId to the list
            }

            var serviceInvoiceStatus = new List<string>();
            foreach (var status in response.Models)
            {
                serviceInvoiceStatus.Add(status.Status);
            }
            
            // Create an anonymous object containing the required data
            var jsonData = new
            {
                EmployeeNames = employeeNames,
                CustomerNames = customerNames,
                ServiceNames = serviceNames,
                ServiceScheduleTimes = scheduleTimes,
                ServiceRegisterIds = serviceRegisterIds,
                InvoiceStatus = serviceInvoiceStatus
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

        public async Task<IActionResult> QualifyEmployeeView()
        {
            var employees = await _supabase.From<Employee>().Select("*").Get();
            ViewBag.Employees = employees.Models;

            var qualifications = await _supabase.From<Qualification>().Select("*").Get();
            ViewBag.Qualifications = qualifications.Models;

            return View("~/Views/CareView/QualifyEmployeeView.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> QualifyEmployee(Guid employeeId, Guid qualificationId)
        {
            try
            {
                await _supabase.From<EmployeeQualification>().Insert(new EmployeeQualification
                {
                    EmployeeId = employeeId,
                    QualificationId = qualificationId
                });

                return Ok("Employee Qualification added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteService(Guid serviceRegisterId)
        {
            try
            {
                // Logic to delete the service from the database using the provided ServiceRegisterId
                // Example using Supabase
                 await _supabase
                    .From<ServiceRegister>()
                    .Where(d => d.ServiceRegisterId == serviceRegisterId)
                    .Delete();

                 return Ok("Worked");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetServiceRevenue()
        {   
            try
            {
                // fetch service register data where status is "sent"
                var serviceRegisterResponse = await _supabase
                    .From<ServiceRegister>()
                    .Select(sr => new object[] { sr.InvoiceDate, sr.ServiceId })
                    .Where(sr => sr.Status == "sent")
                    .Get();

                var serviceRegisterList = serviceRegisterResponse.Models;

                // fetch service rates
                var serviceIds = serviceRegisterList.Select(sr => sr.ServiceId).ToList();
                var serviceRates = new Dictionary<Guid, float>();
                foreach (var serviceId in serviceIds)
                {
                    var serviceResponse = await _supabase
                        .From<Service>()
                        .Select(s => new object[] { s.ServiceId, s.Rate! })
                        .Where(s=>s.ServiceId == serviceId)
                        .Get();
                    var serviceModel = serviceResponse.Models.FirstOrDefault();
                    if (serviceModel != null)
                    {
                        serviceRates[serviceId] = serviceModel.Rate ?? 0;
                    }
                }
        
                // calculate revenue for each invoice and group by month
                var revenueByMonth = new Dictionary<string, float>();
                foreach (var serviceRegister in serviceRegisterList)
                {
                    var invoiceDate = serviceRegister.InvoiceDate.UtcDateTime;
                    var monthYear = invoiceDate.ToString("MMM yyyy");
                    var serviceId = serviceRegister.ServiceId;

                    if (serviceRates.TryGetValue(serviceId, out var rate)) 
                    {
                        var revenue = rate;

                        if (revenueByMonth.TryGetValue(monthYear, out var existingRevenue)) 
                        {
                            revenueByMonth[monthYear] = existingRevenue + revenue;
                        }
                        else 
                        {
                            revenueByMonth.Add(monthYear, revenue);
                        }
                    }
                }

                // Prepare data for Chart.js
                var chartData = PrepareChartData(revenueByMonth);
                var jsonData = JsonConvert.SerializeObject(chartData);

                return Content(jsonData, "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // prepare data for Chart.js
        private object PrepareChartData(Dictionary<string, float> revenueByMonth)
        {
            var currentDate = DateTime.UtcNow;
            var labels = new List<string>();
            var data = new List<float>();

            // iterate over the past 12 months
            for (var i = 0; i < 12; i++)
            {
                var monthYear = currentDate.AddMonths(-i).ToString("MMM yyyy");
                if (revenueByMonth.TryGetValue(monthYear, out var revenue))
                {
                    labels.Insert(0, monthYear); 
                    data.Insert(0, revenueByMonth[monthYear]);
                }
                else
                {
                    labels.Insert(0, monthYear);
                    data.Insert(0, 0); 
                }
            }
            return new
            {
                labels,
                datasets = new[]
                {
                    new
                    {
                        label = "Revenue",
                        data
                    }
                }
            };
        }
        
        public async Task<IActionResult> ServicesView()
        {
            var services = await _supabase.From<Service>().Select("*, SERVICE_QUALIFICATION(*, QUALIFICATION(*))").Get();
            var ret = new List<object>();

            foreach (var service in services.Models)
            {
                var serv_quals = await _supabase.From<ServiceQualification>().Select("*")
                    .Where(qualification => qualification.ServiceId == service.ServiceId).Get();
                var Qualifications = new List<object>();

                foreach (var s_qual in serv_quals.Models)
                {
                    var qual = await _supabase.From<Qualification>().Select("*").Where(qualification =>
                        qualification.QualificationId == s_qual.QualificationId).Get();
                    Qualifications.Add(new
                    {
                        qual.Model?.QualificationId,
                        qual.Model?.QualificationName
                    });
                }
                
                ret.Add(new
                {
                    service.ServiceId,
                    service.ServiceName,
                    service.Rate,
                    Qualifications
                });
            }
            
            ViewBag.Services = ret;
            
            return View("~/Views/CareView/ServicesView.cshtml");
        }
        
        public IActionResult RegisteredInvoicesView()
        {
            return View("~/Views/CareView/RegisteredInvoices.cshtml");

        } 
        [HttpPost]
    public async Task<IActionResult> RegisteredInvoices()
    {
        
        // initialize a list for each column
        var invoiceIDs = new List<Guid>();
        var dates = new List<DateTimeOffset>();
        var customerEmails = new List<String>();
        var serviceNames = new List<String>();
        var serviceCharges = new List<float?>();
        var taxCharges = new List<double?>();
        var totalAmounts = new List<double?>();
    
        // will be used to get customer names and emails
        var renters = new List<Renter>();
        var applicants = new List<Applicant>();
        
        // will be used to get service names
        var serviceIDs = new List<Service>();
        
        // grab all the invoices that have been sent
        var response = await _supabase
            .From<ServiceRegister>()
            .Select("*")
            .Where(s => s.Status == "sent" || s.Status == "SENT")
            .Get();


        
        // grab all the fields we need from invoice table
        foreach (var invoice in response.Models)
        {
            invoiceIDs.Add(invoice.InvoiceId);
            dates.Add(invoice.InvoiceDate);
    
            var renter = await _supabase
                .From<Renter>()
                .Select("*")
                .Where(r => r.RenterId == invoice.RenterId)
                .Get();
            renters.AddRange(renter.Models);
    
            var serviceSchedule = await _supabase
                .From<Service>()
                .Select("*")
                .Where(s => s.ServiceId == invoice.ServiceId)
                .Get();
            serviceIDs.AddRange(serviceSchedule.Models);
        }
    
        // now grab each applicant from renterIDs
        foreach (var renter in renters)
        {
            var applicant = await _supabase
                .From<Applicant>()
                .Select("*")
                .Where(a => a.ApplicantId == renter.ApplicantId)
                .Get();
            
            applicants.AddRange(applicant.Models);
        }
        
        // now grab each customers email
        foreach (var applicant in applicants)
        {
            var email = applicant.Email;
            customerEmails.Add(email);
        }
        
        
        // now grab each service name and rate
        foreach (var serviceID in serviceIDs)
        {
            var serviceName = serviceID.ServiceName;
            serviceNames.Add(serviceName);
            var serviceCharge = serviceID.Rate;
            serviceCharges.Add(serviceCharge);
            // calculate tax and total amount here
            var thisTaxCharge = serviceCharge * TAX;
            taxCharges.Add(thisTaxCharge);
            var thisTotalAmount = serviceCharge + thisTaxCharge;
            totalAmounts.Add(thisTotalAmount);
        }
        
        // Create an anonymous object containing the required data
        var jsonData = new
        {
            InvoiceIDs = invoiceIDs,
            Dates = dates,
            CustomerEmails = customerEmails,
            ServiceNames = serviceNames,
            ServiceCharges = serviceCharges,
            TaxCharges = taxCharges,
            TotalAmounts = totalAmounts
        };
        
        Console.WriteLine("ABOUT TO RETURN INVOICE JSON");
    
        return Json(jsonData);
        
    }
    }
}

