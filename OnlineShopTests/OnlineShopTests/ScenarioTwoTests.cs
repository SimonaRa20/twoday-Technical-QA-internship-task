using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace OnlineShopTests
{
    public class ScenarioTwoTests
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
        public void CompleteOrderForWomenPants()
        {
            NavigatesToWomenPantsSection_WhenClickOnWomenAndPants();
            SortPantsByPrice_WhenChangeSortValuePrice();
            SelectCheapestPants_WhenCLickOnFirstPantsOfList();
            AddPantsToCart_WhenClickOnAddToCartButton("28", "Black", 1);
            VerifyCartIconUpdated(1);
            AddProductToCart_WhenClickOnAddToCartButton("Sylvia Capri", "29", "Blue", 1);
            VerifyCartIconUpdated(2);
            AddProductToCart_WhenClickOnAddToCartButton("Emma Leggings", "29", "Purple", 1);
            VerifyCartIconUpdated(3);
            RemoveProductFromCart("Sylvia Capri");
            VerifyCartIconUpdated(2);
            ProceedToCheckout();
            AddProductFromSuggestedProducts();
        }

        private void NavigatesToWomenPantsSection_WhenClickOnWomenAndPants()
        {
            driver.Navigate().GoToUrl(TestUtils.BaseUrl);
            TestUtils.WaitForElementToBeClickable(wait, By.LinkText("Women")).Click();
            TestUtils.WaitForElementToBeClickable(wait, By.LinkText("Pants")).Click();
        }

        private void SortPantsByPrice_WhenChangeSortValuePrice()
        {
            var sorterDropdown = TestUtils.WaitForElementToBeClickable(wait, By.Id("sorter"));
            var selectElement = new SelectElement(sorterDropdown);
            selectElement.SelectByValue("price");
        }

        private void SelectCheapestPants_WhenCLickOnFirstPantsOfList()
        {
            var productList = wait.Until(driver => driver.FindElements(By.CssSelector(".product-item")));
            var cheapestProduct = productList.First();
            var selectProductLink = cheapestProduct.FindElement(By.CssSelector(".product-item-link"));
            selectProductLink.Click();
        }

        private void AddPantsToCart_WhenClickOnAddToCartButton(string size, string color, int quantity)
        {
            TestUtils.WaitForElementToBeClickable(wait, By.CssSelector($".swatch-attribute.size .swatch-option[aria-label='{size}']")).Click();
            TestUtils.WaitForElementToBeClickable(wait, By.CssSelector($".swatch-attribute.color .swatch-option[aria-label='{color}']")).Click();
            TestUtils.FillInputField(driver, By.Id("qty"), quantity.ToString());

            var addToCartButton = driver.FindElement(By.CssSelector(".action.tocart"));
            addToCartButton.Click();

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElement(addToCartButton, "Added"));
            driver.Navigate().Back();
        }

        private void AddProductToCart_WhenClickOnAddToCartButton(string title, string size, string color, int quantity)
        {
            
            TestUtils.WaitForElementToBeClickable(wait, By.LinkText(title)).Click();
            TestUtils.WaitForElementToBeClickable(wait, By.CssSelector($".swatch-attribute.size .swatch-option[aria-label='{size}']")).Click();
            TestUtils.WaitForElementToBeClickable(wait, By.CssSelector($".swatch-attribute.color .swatch-option[aria-label='{color}']")).Click();
            TestUtils.FillInputField(driver, By.Id("qty"), quantity.ToString());

            var addToCartButton = driver.FindElement(By.CssSelector(".action.tocart"));
            addToCartButton.Click();

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElement(addToCartButton, "Added"));
            driver.Navigate().Back();
        }

        private void VerifyCartIconUpdated(int expectedCount)
        {
            TestUtils.WaitForElementToHaveText(wait, By.CssSelector(".counter-number"), expectedCount.ToString());

            var updatedCartCounterText = driver.FindElement(By.CssSelector(".counter-number")).Text;
            Assert.IsTrue(int.TryParse(updatedCartCounterText, out int itemCount) && itemCount > 0, "Cart counter value is not greater than 0.");
        }

        private void RemoveProductFromCart(string itemName)
        {
            var cartIcon = TestUtils.WaitForElementToBeClickable(wait, By.ClassName("minicart-wrapper"));
            cartIcon.Click();

            var cartItems = wait.Until(driver => driver.FindElements(By.CssSelector(".item.product.product-item")));

            foreach (var item in cartItems)
            {
                var productNameElement = item.FindElement(By.CssSelector(".product-item-name a"));
                if (productNameElement.Text.Equals(itemName, StringComparison.OrdinalIgnoreCase))
                {
                    var removeButton = item.FindElement(By.CssSelector(".action.delete"));
                    removeButton.Click();

                    var confirmButton = TestUtils.WaitForElementToBeClickable(wait, By.CssSelector(".action-primary.action-accept"));
                    confirmButton.Click();
                    break;
                }
            }
        }

        private void ProceedToCheckout()
        {
            var checkoutButton = TestUtils.WaitForElementToBeClickable(wait, By.CssSelector(".action.viewcart"));
            checkoutButton.Click();
        }

        private void AddProductFromSuggestedProducts()
        {
            var suggestedProducts = wait.Until(driver => driver.FindElements(By.CssSelector("ol.products.list.items.product-items li.item.product.product-item")));

            foreach (var product in suggestedProducts)
            {
                var addToCartButton = product.FindElement(By.CssSelector(".actions-primary .action.tocart"));
                addToCartButton.Click();
                break;
            }
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
