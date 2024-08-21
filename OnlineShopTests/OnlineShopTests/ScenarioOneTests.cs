using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

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
            TestUtils.VerifyCartIconUpdated(wait, driver, 1);
            OpensCartAndCheckProduct_WhenSelectToCheckCart("Frankie Sweatshirt");
            OpensCheckoutPage_WhenProceedToCheckout();
            TestUtils.FillsShippingDetails_WhenCheckoutPageIsOpened(wait, driver, "test@gmail.com", "FirstNameTest", "LastNameTest", "123 Test Street", "Test 4", "TestCity", "California", "12345", "United States", "+3705555555");
            TestUtils.CompletesTheOrder_WhenFilledShippingDetails(wait, driver);
            TestUtils.GetsSuccessMessage_WhenAssertOrderSuccess(wait);
        }

        private void ChecksDisplayedNumberOfJackets_WhenPageWasOpened()
        {
            string itemsPerPage = "12";
            TestUtils.SelectDropdownOption(driver, By.Id("limiter"), itemsPerPage);
            var displayedNumberOfJackets = TestUtils.GetDisplayedNumberOfJackets(driver);
            Assert.AreEqual(itemsPerPage, displayedNumberOfJackets, $"The displayed number of jackets '{displayedNumberOfJackets}' does not match the selected number '{itemsPerPage}'.");
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