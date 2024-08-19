using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
