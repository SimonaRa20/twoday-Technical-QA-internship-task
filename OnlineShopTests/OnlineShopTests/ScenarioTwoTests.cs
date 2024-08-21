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
            TestUtils.NavigatesToSpecificSection_WhenClickOnMenuOptions(wait, driver, "Women", "Bottoms", "Pants", "https://magento.softwaretestingboard.com/women/bottoms-women/pants-women.html");
            SortPantsByPrice_WhenChangeSortValuePrice();
            SelectCheapestPants_WhenCLickOnFirstPantsOfList();
            TestUtils.AddProductToCart_WhenClickOnAddToCartButton(wait, driver, "28", "Black", 1);
            TestUtils.VerifyCartIconUpdated_WhenClickOnCartIcon(wait, driver, 1);
            TestUtils.OpensSelectedProductDescription_WhenSelectProductByName(wait, "Sylvia Capri");
            TestUtils.AddProductToCart_WhenClickOnAddToCartButton(wait, driver, "29", "Blue", 1);
            TestUtils.VerifyCartIconUpdated_WhenClickOnCartIcon(wait, driver, 2);
            TestUtils.OpensSelectedProductDescription_WhenSelectProductByName(wait, "Emma Leggings");
            TestUtils.AddProductToCart_WhenClickOnAddToCartButton(wait, driver, "29", "Purple", 1);
            TestUtils.VerifyCartIconUpdated_WhenClickOnCartIcon(wait, driver, 3);
            RemoveProductFromCart_WhenClickOnTrashIcon();
            TestUtils.VerifyCartIconUpdated_WhenClickOnCartIcon(wait, driver, 2);
            ProceedToCheckout_WhenClickOnCheckoutButton();
            AddProductFromSuggestedProducts_WhenClickOnAddOfFirstSuggestProduct();
            OpensCheckoutPage_WhenProceedToCheckout();
            TestUtils.FillsShippingDetails_WhenCheckoutPageIsOpened(wait, driver, "test@gmail.com", "FirstNameTest", "LastNameTest", "123 Test Street", "Test 4", "TestCity", "California", "12345", "United States", "+3705555555");
            TestUtils.CompletesTheOrder_WhenFilledShippingDetails(wait, driver);
            TestUtils.GetsSuccessMessage_WhenAssertOrderSuccess(wait, "The success message is not displayed.");
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
            string beforeSelectUrl = driver.Url;
            var productList = wait.Until(driver => driver.FindElements(By.CssSelector(".product-item")));
            var cheapestProduct = productList.First();
            var selectProductLink = cheapestProduct.FindElement(By.CssSelector(".product-item-link"));
            selectProductLink.Click();
            string afterSelectUrl = driver.Url;
            Assert.AreNotEqual(beforeSelectUrl, afterSelectUrl);
        }

        private void RemoveProductFromCart_WhenClickOnTrashIcon()
        {
            var beforeRemoveCartCounterText = driver.FindElement(By.CssSelector(".counter-number")).Text;
            int beforeRemoveCount = int.Parse(beforeRemoveCartCounterText);

            var cartIcon = TestUtils.WaitForElementToBeClickable(wait, By.ClassName("minicart-wrapper"));
            cartIcon.Click();

            var cartItems = wait.Until(driver => driver.FindElements(By.CssSelector(".item.product.product-item")));
            var removeButton = cartItems[0].FindElement(By.CssSelector(".action.delete"));
            removeButton.Click();

            var confirmButton = TestUtils.WaitForElementToBeClickable(wait, By.CssSelector(".action-primary.action-accept"));
            confirmButton.Click();

            wait.Until(driver =>
            {
                var afterRemoveCartCounterText = driver.FindElement(By.CssSelector(".counter-number")).Text;
                return int.TryParse(afterRemoveCartCounterText, out int afterRemoveCount) && afterRemoveCount < beforeRemoveCount;
            });

            var afterRemoveCartCounterTextFinal = driver.FindElement(By.CssSelector(".counter-number")).Text;
            int afterRemoveCountFinal = int.Parse(afterRemoveCartCounterTextFinal);
            Assert.AreEqual(beforeRemoveCount - 1, afterRemoveCountFinal, "The cart counter did not decrease after removing the product.");
        }

        private void ProceedToCheckout_WhenClickOnCheckoutButton()
        {
            var checkoutButton = TestUtils.WaitForElementToBeClickable(wait, By.CssSelector(".action.viewcart"));
            checkoutButton.Click();
        }

        private void AddProductFromSuggestedProducts_WhenClickOnAddOfFirstSuggestProduct()
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
