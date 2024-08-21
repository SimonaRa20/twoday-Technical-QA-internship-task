using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

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
            TestUtils.NavigatesToSpecificSection_WhenClickOnMenuOptions(wait, driver, "Women", "Bottoms", "Pants");
            SortPantsByPrice_WhenChangeSortValuePrice();
            SelectCheapestPants_WhenCLickOnFirstPantsOfList();
            TestUtils.AddPantsToCart_WhenClickOnAddToCartButton(wait, driver, "28", "Black", 1);
            TestUtils.VerifyCartIconUpdated(wait, driver, 1);
            TestUtils.OpensSelectedProductDescription_WhenSelectProductByName(wait, "Sylvia Capri");
            TestUtils.AddPantsToCart_WhenClickOnAddToCartButton(wait, driver, "29", "Blue", 1);
            TestUtils.VerifyCartIconUpdated(wait, driver, 2);
            TestUtils.OpensSelectedProductDescription_WhenSelectProductByName(wait, "Emma Leggings");
            TestUtils.AddPantsToCart_WhenClickOnAddToCartButton(wait, driver, "29", "Purple", 1);
            TestUtils.VerifyCartIconUpdated(wait, driver, 3);
            RemoveProductFromCart("Sylvia Capri");
            TestUtils.VerifyCartIconUpdated(wait, driver, 2);
            ProceedToCheckout();
            AddProductFromSuggestedProducts();
            OpensCheckoutPage_WhenProceedToCheckout();
            TestUtils.FillsShippingDetails_WhenCheckoutPageIsOpened(wait, driver, "test@gmail.com", "FirstNameTest", "LastNameTest", "123 Test Street", "Test 4", "TestCity", "California", "12345", "United States", "+3705555555");
            TestUtils.CompletesTheOrder_WhenFilledShippingDetails(wait, driver);
            TestUtils.GetsSuccessMessage_WhenAssertOrderSuccess(wait);
        }

        private void SortPantsByPrice_WhenChangeSortValuePrice()
        {
            var sorterDropdown = TestUtils.WaitForElementToBeClickable(wait, By.Id("sorter"));
            var selectElement = new SelectElement(sorterDropdown);
            selectElement.SelectByValue("price");
            TestUtils.WaitForElementToBeVisible(wait, By.Id("sorter"));

            sorterDropdown = TestUtils.WaitForElementToBeClickable(wait, By.Id("sorter"));
            selectElement = new SelectElement(sorterDropdown);

            var selectedOption = selectElement.SelectedOption;
            Assert.AreEqual("price", selectedOption.GetAttribute("value"));
        }

        private void SelectCheapestPants_WhenCLickOnFirstPantsOfList()
        {
            var productList = wait.Until(driver => driver.FindElements(By.CssSelector(".product-item")));
            var cheapestProduct = productList.First();
            var selectProductLink = cheapestProduct.FindElement(By.CssSelector(".product-item-link"));
            selectProductLink.Click();
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
            var suggestedProduct = wait.Until(driver => driver.FindElements(By.CssSelector("ol.products.list.items.product-items li.item.product.product-item"))).FirstOrDefault();
            var addToCartButton = suggestedProduct.FindElement(By.CssSelector(".actions-primary .action.tocart"));
            addToCartButton.Click();
        }

        private void OpensCheckoutPage_WhenProceedToCheckout()
        {
            TestUtils.WaitForElementToBeClickable(wait, By.CssSelector(".action.primary.checkout")).Click();
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
