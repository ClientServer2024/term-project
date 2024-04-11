// Ensure you have the correct using directives for your Supabase client library
using term_project.Models.CareModels;

using dotenv.net;
using term_project.Models.CRMModels;

// Inside the appropriate method
DotEnv.Load();


var builder = WebApplication.CreateBuilder(args);

var supabaseUrl = Environment.GetEnvironmentVariable("Supabase__Url");
var supabaseKey = Environment.GetEnvironmentVariable("Supabase__Key");

// Use the variables in your configuration
builder.Configuration["Supabase:Url"] = supabaseUrl;
builder.Configuration["Supabase:Key"] = supabaseKey;

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Adjust this part according to how you instantiate your Supabase client

    await InitializeSupabase(supabaseUrl, supabaseKey);


app.Run();

async Task InitializeSupabase(string url, string key)
{
    var options = new Supabase.SupabaseOptions
    {
        AutoConnectRealtime = true
    };

    var supabase = new Supabase.Client(url, key, options);
    await supabase.InitializeAsync();

    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Reached!");

    await InsertEmployee(supabase, logger);

    await GetFirstUserEmail(supabase, logger);

    await InsertAsset(supabase, logger);


}

// QA TEAM: try catch block is for testing purposes only. It should be removed when code gets pushed to prod.

async Task InsertAsset(Supabase.Client supabase, ILogger logger)
{
    try
    {
        // Create a new asset object
        var newAsset = new Asset
        {
            AssetId = Guid.NewGuid(), // Generate a new GUID
            Type = "House", // Set the type of asset
            Status = "Unavailable", // Set the status of the asset
            ApplicationCount = 0, // Initialize application count
            Rate = 1800.00f // Set the rate for the asset
        };

        // Perform the insert operation
        var response = await supabase.From<Asset>().Insert(newAsset);

        logger.LogInformation("New asset inserted successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError($"An exception occurred while inserting the asset: {ex.Message}");
    }
}
async Task InsertEmployee(Supabase.Client supabase, ILogger logger)
{
    try
    {
        // Create a new employee object
        var newEmployee = new Employee
        {
            EmployeeId = Guid.NewGuid(), // Generate a new GUID if your primary key is a GUID and not auto-generated
            Email = "newemployee@example.com",
            Password = "securepassword", // Make sure to never store plain text passwords in production
            FirstName = "New",
            LastName = "Employee",
            Address = "123 New Street",
            EmergencyContact = "9876543210",
            JobTitle = "Developer",
            EmploymentType = "full-time",
            SalaryRate = 60000, // Example salary
            ManagerId = 1 // Example manager ID, adjust accordingly
        };

        // Perform the insert operation
        var response = await supabase.From<Employee>().Insert(newEmployee);


        logger.LogInformation("New employee inserted successfully.");


    }
    catch (Exception ex)
    {
        logger.LogError($"An exception occurred while inserting the employee: {ex.Message}");
    }
}

async Task GetFirstUserEmail(Supabase.Client supabase, ILogger logger)
{
    try
    {
        var result = await supabase.From<Employee>().Get();
        var employees = result.Models;

        if (employees != null && employees.Any())
        {
            var firstEmployee = employees.First();
            logger.LogInformation($"First Employee's Email: {firstEmployee.Email}");
        }
        else
        {
            logger.LogInformation("No employee data found.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError($"An error occurred while fetching the email: {ex.Message}");
    }
}

