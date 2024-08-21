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

        public static void FillInputField(IWebDriver driver, By locator, string value)
        {
            var element = driver.FindElement(locator);
            element.Clear();
            element.SendKeys(value);
        }

        public static void SelectDropdownOption(IWebDriver driver, By by, string optionText)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            var dropdown = wait.Until(drv => drv.FindElement(by));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", dropdown);

            try
            {
                dropdown.Click();
            }
            catch (ElementNotInteractableException)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", dropdown);
            }

            var selectElement = new SelectElement(dropdown);
            if (selectElement.SelectedOption.Text.Trim() != optionText)
            {
                try
                {
                    selectElement.SelectByText(optionText);
                }
                catch (NoSuchElementException)
                {
                    throw new NoSuchElementException($"The option '{optionText}' was not found in the dropdown.");
                }
            }
        }

        public static void NavigatesToSpecificSection_WhenClickOnMenuOptions(WebDriverWait wait, IWebDriver driver, string section1, string section2, string section3)
        {
            driver.Navigate().GoToUrl(BaseUrl);
            IWebElement section1Menu = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(section1)));
            Actions actions = new Actions(driver);
            actions.MoveToElement(section1Menu).Perform();

            IWebElement section2Menu = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(section2)));
            actions.MoveToElement(section2Menu).Perform();

            IWebElement section3MenuItem = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(section3)));
            section3MenuItem.Click();
        }

        public static void OpensSelectedProductDescription_WhenSelectProductByName(WebDriverWait wait, string productName)
        {
            WaitForElementToBeClickable(wait, By.LinkText(productName)).Click();
        }

        public static void AddPantsToCart_WhenClickOnAddToCartButton(WebDriverWait wait, IWebDriver driver, string size, string color, int quantity)
        {
            WaitForElementToBeClickable(wait, By.CssSelector($".swatch-attribute.size .swatch-option[aria-label='{size}']")).Click();
            WaitForElementToBeClickable(wait, By.CssSelector($".swatch-attribute.color .swatch-option[aria-label='{color}']")).Click();
            FillInputField(driver, By.Id("qty"), quantity.ToString());

            var addToCartButton = driver.FindElement(By.CssSelector(".action.tocart"));
            addToCartButton.Click();

            wait.Until(ExpectedConditions.TextToBePresentInElement(addToCartButton, "Added"));
            driver.Navigate().Back();
        }

        public static void VerifyCartIconUpdated(WebDriverWait wait, IWebDriver driver, int expectedCount)
        {
            WaitForElementToHaveText(wait, By.CssSelector(".counter-number"), expectedCount.ToString());

            var updatedCartCounterText = driver.FindElement(By.CssSelector(".counter-number")).Text;
            Assert.IsTrue(int.TryParse(updatedCartCounterText, out int itemCount) && itemCount > 0, "Cart counter value is not greater than 0.");
        }

        public static string GetDisplayedNumberOfJackets(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var toolbarAmount = wait.Until(drv => drv.FindElement(By.Id("toolbar-amount")));

            var displayedText = toolbarAmount.Text;

            var match = Regex.Match(displayedText, @"Items (\d+)-(\d+) of \d+");

            if (match.Success)
            {
                return match.Groups[2].Value;
            }

            throw new Exception("Failed to parse the number of displayed items.");
        }

        public static void FillsShippingDetails_WhenCheckoutPageIsOpened(WebDriverWait wait, IWebDriver driver, string email, string firstName, string lastName, string street, string streetLine2, string city, string region, string postalCode, string country, string phoneNumber)
        {
            IWebElement WaitForAndGetElement(By locator)
            {
                var element = WaitForElementToBeVisible(wait, locator);
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

        public static void CompletesTheOrder_WhenFilledShippingDetails(WebDriverWait wait, IWebDriver driver)
        {
            var placeOrderButton = TestUtils.WaitForElementToBeClickable(wait, By.CssSelector("#checkout-payment-method-load button.checkout"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", placeOrderButton);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", placeOrderButton);

            wait.Until(driver => driver.Url.Contains("/checkout/onepage/success/"));
        }

        public static void GetsSuccessMessage_WhenAssertOrderSuccess(WebDriverWait wait)
        {
            var successMessage = TestUtils.WaitForElementToBeVisible(wait, By.ClassName("page-title"));
            Assert.IsTrue(successMessage.Displayed, "The success message is not displayed.");
        }
    }
}
