using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
    }
}
