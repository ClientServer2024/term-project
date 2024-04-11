using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using OpenQA.Selenium.DevTools.V121.CSS;


namespace term_project.Tests
{
    public class UITests : IDisposable
    {
        private readonly IWebDriver driver;
  public UITests()
{
    string chromeDriverPath = "/bin/chromedriver";
    ChromeOptions options = new ChromeOptions();
    driver = new ChromeDriver(chromeDriverPath, options);
}




        [Fact]
        public async Task Tests(){
            await Task.Run(() => CA03());
            await Task.Run(() => CA04());
            await Task.Run(() => CA05());
        }

        // As a client service representative, I want to register clients for services to ensure they are enrolled in the appropriate care or recreational programs.
        // - Client service representatives must be able to enroll clients in services, recording all necessary details.
    // - The system should confirm registration to both the client and the service provider.

        [Fact]
        public void CA03()
        {
            driver.Navigate().GoToUrl("http://localhost:5153");
            System.Threading.Thread.Sleep(2000);
            IWebElement careLink = driver.FindElement(By.XPath("//a[contains(text(), 'Care')]"));
            string careHref = careLink.GetAttribute("href");
            driver.Navigate().GoToUrl(careHref);
            System.Threading.Thread.Sleep(2000);

            IWebElement employeeSelection = driver.FindElement(By.Id("redirectButton"));
            employeeSelection.Click();
            System.Threading.Thread.Sleep(2000);

            IWebElement serviceDropdown = driver.FindElement(By.Id("serviceId"));
            SelectElement selectService = new SelectElement(serviceDropdown);
            selectService.SelectByText("RavTestSecond");
            System.Threading.Thread.Sleep(3000);


            IWebElement customerDropdown = driver.FindElement(By.Id("customerName"));
            SelectElement selectCustomer = new SelectElement(customerDropdown);
            selectCustomer.SelectByText("Gursidh");
            System.Threading.Thread.Sleep(2000);

            IWebElement employeeDropdown = driver.FindElement(By.Id("employeeId"));
            SelectElement selectEmployee = new SelectElement(employeeDropdown);
            selectEmployee.SelectByText("Troy");
            System.Threading.Thread.Sleep(2000);

            IWebElement registerService = driver.FindElement(By.Id("registerServiceBtn"));
            registerService.Click();

        }
        /*
        As a client service representative, I want to ensure that employees scheduled to complete a service meet 
        //the skill requirements to ensure they are capable of providing that service.
        - The system must check in the employee table that the employee has the appropriate qualification in order to provide the service (please coordinate with HR team and HR PM*)
        */
        [Fact]
        public void CA04()
        {

            driver.Navigate().GoToUrl("http://localhost:5153");
            System.Threading.Thread.Sleep(2000);
            IWebElement careLink = driver.FindElement(By.XPath("//a[contains(text(), 'Care')]"));
            string careHref = careLink.GetAttribute("href");
            driver.Navigate().GoToUrl(careHref);
            System.Threading.Thread.Sleep(2000);

            IWebElement employeeSelection = driver.FindElement(By.Id("redirectButton"));
            employeeSelection.Click();
            System.Threading.Thread.Sleep(2000);

            IWebElement serviceDropdown = driver.FindElement(By.Id("serviceId"));
            SelectElement selectService = new SelectElement(serviceDropdown);
            selectService.SelectByText("RavTestSecond");
            System.Threading.Thread.Sleep(3000);



            IWebElement customerDropdown = driver.FindElement(By.Id("customerName"));
            IList<IWebElement> options = customerDropdown.FindElements(By.TagName("option"));
            List<string> customerNames = new List<string>();

            System.Threading.Thread.Sleep(2000);
            foreach (IWebElement option in options)
            {
                string value = option.GetAttribute("value");
                customerNames.Add(option.Text);
            }

            System.Threading.Thread.Sleep(2000);
            HashSet<string> uniqueNames = new HashSet<string>();
            foreach (string name in customerNames){
                if (!uniqueNames.Add(name)) {
                    throw new ArgumentException($"Duplciate name found: '{name}'.");
                }
            }

        }

        
        public IWebElement WaitForElementVisible(By by, int timeoutInSeconds)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            return wait.Until(condition =>
            {
                try
                {
                    var element = driver.FindElement(by);
                    return element.Displayed ? element : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
            });
        }

        [Fact]
        public void CA05()
        {
            Console.WriteLine("\n\nCA05 TEST\n\n");
            driver.Navigate().GoToUrl("http://localhost:5153/Care/CareLanding");
            // Wait for the chart container to be visible
            var chartContainer = WaitForElementVisible(By.Id("chartContainer"), 10);
            Assert.NotNull(chartContainer);

            var canvas = chartContainer.FindElement(By.Id("myChart"));
            Assert.NotNull(canvas);
        }

        public void Dispose()
        {
            driver.Quit();
        }
    }
}