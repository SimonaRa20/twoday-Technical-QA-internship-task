using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace OnlineShopTests
{
    public class ScenarioOneTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        }

        [Test]
        public void CompleteOrderForFrankieSweatshirt()
        {
            NavigateToMenHoodiesSection();
        }

        private void NavigateToMenHoodiesSection()
        {
            driver.Navigate().GoToUrl("https://magento.softwaretestingboard.com/");
            var menMenu = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Men")));
            menMenu.Click();
            var hoodiesAndSweatshirtsLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Hoodies & Sweatshirts")));
            hoodiesAndSweatshirtsLink.Click();
        }

        [TearDown]
        public void Teardown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }
    }
}