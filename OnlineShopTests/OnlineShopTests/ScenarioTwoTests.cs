using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            AddPantsToCart_WhenClickOnAddToCartButton();
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
            if (productList.Count == 0)
            {
                throw new NoSuchElementException("No products found.");
            }

            var cheapestProduct = productList.First();
            var selectProductLink = cheapestProduct.FindElement(By.CssSelector(".product-item-link"));
            selectProductLink.Click();
        }

        private void AddPantsToCart_WhenClickOnAddToCartButton()
        {
            var addToCartButton = TestUtils.WaitForElementToBeClickable(wait, By.CssSelector(".action.tocart"));
            addToCartButton.Click();
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
