using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

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
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        }

        [Test]
        public void CompleteOrderForFrankieSweatshirt()
        {
            TestUtils.NavigatesToSpecificSection_WhenClickOnMenuOptions(wait, driver, "Men", "Tops", "Hoodies & Sweatshirts");
            ChecksDisplayedNumberOfJackets_WhenPageWasOpened();
            TestUtils.OpensSelectedProductDescription_WhenSelectProductByName(wait, "Frankie Sweatshirt");
            TestUtils.AddPantsToCart_WhenClickOnAddToCartButton(wait, driver, "M", "Green", 1);
            TestUtils.VerifyCartIconUpdated_WhenClickOnCartIcon(wait, driver, 1);
            OpensCartAndCheckProduct_WhenSelectToCheckCart("Frankie Sweatshirt");
            OpensCheckoutPage_WhenProceedToCheckout();
            TestUtils.FillsShippingDetails_WhenCheckoutPageIsOpened(wait, driver, "test@gmail.com", "FirstNameTest", "LastNameTest", "123 Test Street", "Test 4", "TestCity", "California", "12345", "United States", "+3705555555");
            TestUtils.CompletesTheOrder_WhenFilledShippingDetails(wait, driver);
            TestUtils.GetsSuccessMessage_WhenAssertOrderSuccess(wait, "The success message is not displayed.");
        }

        private void ChecksDisplayedNumberOfJackets_WhenPageWasOpened()
        {
            var element = driver.FindElement(By.CssSelector("#limiter"));
            string selectedText = element.GetAttribute("value");

            var toolbarAmount = wait.Until(drv => drv.FindElement(By.Id("toolbar-amount")));
            var displayedText = toolbarAmount.Text;
            var match = Regex.Match(displayedText, @"Items (\d+)-(\d+) of \d+");

            Assert.AreEqual(selectedText, match.Groups[2].Value, $"The displayed number of jackets '{match.Groups[2].Value}' does not match the selected number '{selectedText}'.");
        }

        private void OpensCartAndCheckProduct_WhenSelectToCheckCart(string expectedProductName)
        {
            TestUtils.WaitForElementToBeClickable(wait, By.CssSelector(".action.showcart")).Click();

            wait.Until(driver =>
            {
                var cartProductElements = driver.FindElements(By.CssSelector("strong.product-item-name"));
                return cartProductElements.Count > 0 && cartProductElements.Any(e => e.Text.Contains(expectedProductName));
            });

            var cartProduct = driver.FindElement(By.CssSelector("strong.product-item-name"));
            Assert.IsTrue(cartProduct.Text.Contains(expectedProductName), $"Product '{expectedProductName}' not found in the cart.");
        }

        private void OpensCheckoutPage_WhenProceedToCheckout()
        {
            TestUtils.WaitForElementToBeClickable(wait, By.Id("top-cart-btn-checkout")).Click();
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