using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
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
            TestUtils.NavigatesToSpecificSection_WhenClickOnMenuOptions(wait, driver, "Men", "Tops", "Hoodies & Sweatshirts", "https://magento.softwaretestingboard.com/men/tops-men/hoodies-and-sweatshirts-men.html");
            ChecksDisplayedNumberOfJackets_WhenPageWasOpened();
            TestUtils.OpensSelectedProductDescription_WhenSelectProductByName(wait, "Frankie Sweatshirt");
            TestUtils.AddProductToCart_WhenClickOnAddToCartButton(wait, driver, "M", "Green", 1);
            ProceedToCheckout_WhenClickOnCheckoutButton();
            OpensCartAndCheckProduct_WhenSelectToCheckCart("Frankie Sweatshirt", "M", "Green");
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

        private void ProceedToCheckout_WhenClickOnCheckoutButton()
        {
            TestUtils.WaitForElementToBeClickable(wait, By.CssSelector(".action.showcart")).Click();

            wait.Until(driver =>
            {
                var cartProductElements = driver.FindElements(By.CssSelector("strong.product-item-name"));
                return cartProductElements.Count > 0;
            });

            var checkoutButton = TestUtils.WaitForElementToBeClickable(wait, By.CssSelector(".action.viewcart"));
            checkoutButton.Click();
        }

        private void OpensCartAndCheckProduct_WhenSelectToCheckCart(string expectedProductName, string expectedSize, string expectedColor)
        {
            var cartItems = wait.Until(driver => driver.FindElements(By.CssSelector("td.col.item")));
            bool productFound = false;

            foreach (var cartItem in cartItems)
            {
                var productNameElement = cartItem.FindElement(By.CssSelector(".product-item-name a"));
                string productName = productNameElement.Text.Trim();

                var itemOptions = cartItem.FindElement(By.CssSelector(".item-options"));

                var dtElements = itemOptions.FindElements(By.CssSelector("dt"));
                var ddElements = itemOptions.FindElements(By.CssSelector("dd"));

                string actualSize = string.Empty;
                string actualColor = string.Empty;

                for (int i = 0; i < dtElements.Count; i++)
                {
                    if (dtElements[i].Text.Trim().Equals("Size", StringComparison.OrdinalIgnoreCase))
                    {
                        actualSize = ddElements[i].Text.Trim();
                    }
                    else if (dtElements[i].Text.Trim().Equals("Color", StringComparison.OrdinalIgnoreCase))
                    {
                        actualColor = ddElements[i].Text.Trim();
                    }
                }

                if (productName.Equals(expectedProductName, StringComparison.OrdinalIgnoreCase) &&
                    actualSize.Equals(expectedSize, StringComparison.OrdinalIgnoreCase) &&
                    actualColor.Equals(expectedColor, StringComparison.OrdinalIgnoreCase))
                {
                    productFound = true;
                    break;
                }
            }

            Assert.IsTrue(productFound, $"The product '{expectedProductName}' with size '{expectedSize}' and color '{expectedColor}' was not found in the cart.");
        }

        private void OpensCheckoutPage_WhenProceedToCheckout()
        {
            var elements = driver.FindElements(By.CssSelector(".action.primary.checkout"));
            var proceedToCheckoutButton = elements[1];
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", proceedToCheckoutButton);
            proceedToCheckoutButton.Click();
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