using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Interactions;

namespace OnlineShopTests
{
    public static class TestUtils
    {
        public const string BaseUrl = "https://magento.softwaretestingboard.com/";

        public static IWebElement WaitForElementToBeClickable(WebDriverWait wait, By locator)
        {
            return wait.Until(ExpectedConditions.ElementToBeClickable(locator));
        }

        public static IWebElement WaitForElementToBeVisible(WebDriverWait wait, By locator)
        {
            return wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        public static IWebElement WaitForElementToHaveText(WebDriverWait wait, By locator, string expectedText)
        {
            return wait.Until(driver =>
            {
                var element = driver.FindElement(locator);
                return element.Text.Contains(expectedText) ? element : null;
            });
        }

        public static void FillInputField(WebDriverWait wait, By locator, string value)
        {
            var element = WaitForElementToBeVisible(wait, locator);
            element.Clear();
            element.SendKeys(value);
        }

        public static void NavigatesToSpecificSection_WhenClickOnMenuOptions(WebDriverWait wait, IWebDriver driver, string section1, string section2, string section3, string expectedUrl)
        {
            driver.Navigate().GoToUrl(BaseUrl);
            IWebElement section1Menu = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(section1)));
            Actions actions = new Actions(driver);
            actions.MoveToElement(section1Menu).Perform();

            IWebElement section2Menu = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(section2)));
            actions.MoveToElement(section2Menu).Perform();

            IWebElement section3MenuItem = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(section3)));
            section3MenuItem.Click();

            wait.Until(driver => driver.Url.Equals(expectedUrl, StringComparison.OrdinalIgnoreCase));
            string currentUrl = driver.Url;
            Assert.AreEqual(expectedUrl, currentUrl, $"The URL after navigating to {section1} > {section2} > {section3} did not match the expected URL.");
        }

        public static void OpensSelectedProductDescription_WhenSelectProductByName(WebDriverWait wait, string productName)
        {
            WaitForElementToBeClickable(wait, By.LinkText(productName)).Click();
        }

        public static void AddProductToCart_WhenClickOnAddToCartButton(WebDriverWait wait, IWebDriver driver, string size, string color, int quantity)
        {
            WaitForElementToBeClickable(wait, By.CssSelector($".swatch-attribute.size .swatch-option[aria-label='{size}']")).Click();
            WaitForElementToBeClickable(wait, By.CssSelector($".swatch-attribute.color .swatch-option[aria-label='{color}']")).Click();
            FillInputField(wait, By.Id("qty"), quantity.ToString());

            var addToCartButton = driver.FindElement(By.CssSelector(".action.tocart"));
            string beforeAddButtonText = addToCartButton.Text;
            addToCartButton.Click();

            wait.Until(ExpectedConditions.TextToBePresentInElement(addToCartButton, "Added"));

            string afterAddButtonText = addToCartButton.Text;
            Assert.AreNotEqual(beforeAddButtonText, afterAddButtonText, "The button text did not change to 'Added' after clicking.");

            driver.Navigate().Back();
        }

        public static void VerifyCartIconUpdated_WhenClickOnCartIcon(WebDriverWait wait, IWebDriver driver, int expectedCount)
        {
            WaitForElementToHaveText(wait, By.CssSelector(".counter-number"), expectedCount.ToString());
            var updatedCartCounterText = driver.FindElement(By.CssSelector(".counter-number")).Text;
            Assert.IsTrue(int.TryParse(updatedCartCounterText, out int itemCount) && itemCount > 0, "Cart counter value is not greater than 0.");
        }

        public static void FillsShippingDetails_WhenCheckoutPageIsOpened(WebDriverWait wait, IWebDriver driver, string email, string firstName, string lastName, string street, string streetLine2, string city, string region, string postalCode, string country, string phoneNumber)
        {
            var waitAndGetElement = (By locator) => WaitForElementToBeVisible(wait, locator);
            
            FillInputField(wait, By.CssSelector("input#customer-email"), email);
            FillInputField(wait, By.Name("firstname"), firstName);
            FillInputField(wait, By.Name("lastname"), lastName);
            FillInputField(wait, By.Name("street[0]"), street);
            FillInputField(wait, By.Name("street[1]"), streetLine2);
            FillInputField(wait, By.Name("city"), city);
            FillInputField(wait, By.Name("postcode"), postalCode);
            FillInputField(wait, By.Name("telephone"), phoneNumber);

            var regionDropdown = waitAndGetElement(By.Name("region_id"));
            new SelectElement(regionDropdown).SelectByText(region);

            var countryDropdown = waitAndGetElement(By.Name("country_id"));
            new SelectElement(countryDropdown).SelectByText(country);

            var shippingMethodRadio = waitAndGetElement(By.CssSelector("input[value='flatrate_flatrate']"));
            shippingMethodRadio.Click();

            var continueButton = waitAndGetElement(By.CssSelector("[data-role='opc-continue']"));
            continueButton.Click();
        }

        public static void CompletesTheOrder_WhenFilledShippingDetails(WebDriverWait wait, IWebDriver driver)
        {
            var placeOrderButton = WaitForElementToBeClickable(wait, By.CssSelector("#checkout-payment-method-load button.checkout"));
            var actions = new Actions(driver);
            actions.MoveToElement(placeOrderButton).Perform();
            placeOrderButton.Click();

            wait.Until(driver => driver.Url.Contains("/checkout/onepage/success/"));
        }

        public static void GetsSuccessMessage_WhenAssertOrderSuccess(WebDriverWait wait, string message)
        {
            var successMessage = WaitForElementToBeVisible(wait, By.ClassName("page-title"));
            Assert.IsTrue(successMessage.Displayed, message);
        }
    }
}
