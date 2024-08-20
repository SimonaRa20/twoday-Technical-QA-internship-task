using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Diagnostics.Metrics;
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
            NavigatesToMenHoodiesSection_WhenSelectMen_HoodiesAndSweatshirts();
            ChecksDisplayedNumberOfJackets_WhenPageWasOpened();
            OpensSelectedProductDescription_WhenSelectProductByName("Frankie Sweatshirt");
            UpdateSizeColorAndQuantitySelection_WhenSelectSizeColorAndQuantity("M", "Green", 1);
            AddProductToCart_WhenSelectToAddProductToCart();
            OpensCartAndCheckProduct_WhenSelectToCheckCart("Frankie Sweatshirt");
            OpensCheckoutPage_WhenProceedToCheckout();
            FillsShippingDetails_WhenCheckoutPageIsOpened("test@gmail.com", "FirstNameTest", "LastNameTest", "123 Test Street", "Test 4", "TestCity", "California", "12345", "United States", "+3705555555");
            CompletesTheOrder_WhenFilledShippingDetails();
            GetsSuccessMessage_WhenAssertOrderSuccess();
        }

        private void NavigatesToMenHoodiesSection_WhenSelectMen_HoodiesAndSweatshirts()
        {
            driver.Navigate().GoToUrl(TestUtils.BaseUrl);
            TestUtils.WaitForElementToBeClickable(wait, By.LinkText("Men")).Click();
            TestUtils.WaitForElementToBeClickable(wait, By.LinkText("Hoodies & Sweatshirts")).Click();
        }

        private void ChecksDisplayedNumberOfJackets_WhenPageWasOpened()
        {
            string itemsPerPage = "12";
            TestUtils.SelectDropdownOption(driver, By.Id("limiter"), itemsPerPage);
            var displayedNumberOfJackets = TestUtils.GetDisplayedNumberOfJackets(driver);
            Assert.AreEqual(itemsPerPage, displayedNumberOfJackets, $"The displayed number of jackets '{displayedNumberOfJackets}' does not match the selected number '{itemsPerPage}'.");
        }

        private void OpensSelectedProductDescription_WhenSelectProductByName(string productName)
        {
            TestUtils.WaitForElementToBeClickable(wait, By.LinkText(productName)).Click();
        }

        private void UpdateSizeColorAndQuantitySelection_WhenSelectSizeColorAndQuantity(string size, string color, int quantity)
        {
            TestUtils.WaitForElementToBeClickable(wait, By.CssSelector($".swatch-attribute.size .swatch-option[aria-label='{size}']")).Click();
            TestUtils.WaitForElementToBeClickable(wait, By.CssSelector($".swatch-attribute.color .swatch-option[aria-label='{color}']")).Click();
            TestUtils.FillInputField(driver, By.Id("qty"), quantity.ToString());
        }

        private void AddProductToCart_WhenSelectToAddProductToCart()
        {
            var addToCartButton = TestUtils.WaitForElementToBeClickable(wait, By.CssSelector(".action.tocart"));
            addToCartButton.Click();

            TestUtils.WaitForElementToHaveText(wait, By.CssSelector(".counter-number"), "1");

            var updatedCartCounterText = driver.FindElement(By.CssSelector(".counter-number")).Text;
            Assert.IsTrue(int.TryParse(updatedCartCounterText, out int itemCount) && itemCount > 0, "Cart counter value is not greater than 0.");
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

        private void FillsShippingDetails_WhenCheckoutPageIsOpened(string email, string firstName, string lastName, string street, string streetLine2, string city, string region, string postalCode, string country, string phoneNumber)
        {
            IWebElement WaitForAndGetElement(By locator)
            {
                var element = TestUtils.WaitForElementToBeVisible(wait, locator);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
                return element;
            }

            var emailInput = WaitForAndGetElement(By.CssSelector("input#customer-email"));
            emailInput.Clear();
            emailInput.SendKeys(email);

            var firstNameInput = WaitForAndGetElement(By.Name("firstname"));
            firstNameInput.Clear();
            firstNameInput.SendKeys(firstName);

            var lastNameInput = WaitForAndGetElement(By.Name("lastname"));
            lastNameInput.Clear();
            lastNameInput.SendKeys(lastName);

            var streetInput = WaitForAndGetElement(By.Name("street[0]"));
            streetInput.Clear();
            streetInput.SendKeys(street);

            var streetInputLine2 = WaitForAndGetElement(By.Name("street[1]"));
            streetInputLine2.Clear();
            streetInputLine2.SendKeys(streetLine2);

            var cityInput = WaitForAndGetElement(By.Name("city"));
            cityInput.Clear();
            cityInput.SendKeys(city);

            var regionDropdown = WaitForAndGetElement(By.Name("region_id"));
            new SelectElement(regionDropdown).SelectByText(region);

            var postalCodeInput = WaitForAndGetElement(By.Name("postcode"));
            postalCodeInput.Clear();
            postalCodeInput.SendKeys(postalCode);

            var countryDropdown = WaitForAndGetElement(By.Name("country_id"));
            new SelectElement(countryDropdown).SelectByText(country);

            var phoneNumberInput = WaitForAndGetElement(By.Name("telephone"));
            phoneNumberInput.Clear();
            phoneNumberInput.SendKeys(phoneNumber);

            var shippingMethodRadio = WaitForAndGetElement(By.CssSelector("input[value='flatrate_flatrate']"));
            shippingMethodRadio.Click();

            var continueButton = WaitForAndGetElement(By.CssSelector("[data-role='opc-continue']"));
            continueButton.Click();
        }

        private void CompletesTheOrder_WhenFilledShippingDetails()
        {
            var placeOrderButton = TestUtils.WaitForElementToBeClickable(wait, By.CssSelector("#checkout-payment-method-load button.checkout"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", placeOrderButton);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", placeOrderButton);

            wait.Until(driver => driver.Url.Contains("/checkout/onepage/success/"));
        }

        private void GetsSuccessMessage_WhenAssertOrderSuccess()
        {
            var successMessage = TestUtils.WaitForElementToBeVisible(wait, By.ClassName("page-title"));
            Assert.IsTrue(successMessage.Displayed, "The success message is not displayed.");
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