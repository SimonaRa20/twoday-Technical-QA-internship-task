# twoday
## Technical QA internship task
### Online Shop Automated Test Suite

This project is an automated test suite for an online shop (https://magento.softwaretestingboard.com/). The tests are designed to validate key functionalities such as product selection, adding items to the cart, and completing orders. The task was completed using **C#** with **Selenium**.

## Prerequisites

- **NUnit:** Version 3.14.0
- **NUnit.Analyzers:** Version 3.9.0
- **NUnit3TestAdapter:** Version 4.5.0
- **Selenium.Support:** Version 4.23.0
- **Selenium.WebDriver:** Version 4.23.0
- **Selenium.WebDriver.ChromeDriver:** Version matching the installed Chrome browser (used 127.0.6533.11900)
- **SeleniumExtras.WaitHelpers:** Version 1.0.2

## Installation

1. Clone the repository
`git clone https://github.com/yourusername/your-repo-name.git
cd your-repo-name`
3. Install dependencies
  * Ensure you have .NET SDK installed on your machine. You can download it from [here](https://dotnet.microsoft.com/en-us/download/dotnet).
  * Install the required NuGet packages by running the following command in the project directory: 
`dotnet restore`
3. Run tests
  * To execute the tests, you can use the following command: dotnet test

## Project Structure

* _OnlineShopTests/ScenarioOneTests.cs_: Contains tests for the first scenario where the user selects a men's sweatshirt, adds it to the cart, and completes an order.
* _OnlineShopTests/ScenarioTwoTests.cs_: Contains tests for the second scenario where the user selects women's pants, adds multiple items to the cart, removes one, and completes the order.
* _OnlineShopTests/TestUtils.cs_: Utility methods used across both test scenarios for navigation, interaction, and validation.

## Test Scenarios
### Scenario One: Ordering a Men's Sweatshirt
1. Navigate to Men's Hoodies & Sweatshirts Section
* Validate that the displayed number of jackets matches the selected number per page.
2. Select and Customize "Frankie Sweatshirt"
* Choose size, color, and quantity.
* Add the product to the cart and verify the cart icon is updated with the correct quantity.
3. Checkout
* Open the cart and confirm the product details.
* Proceed to checkout and fill in the shipping details.
* Complete the order and verify the success message.

### Scenario Two: Ordering Women's Pants
1. Navigate to Women's Pants Section
* Filter the products to show the cheapest ones.
2. Select and Add the Cheapest Pants
* Add the cheapest available pants to the cart.
3. Add More Products and Update Cart
* Add two more products to the cart.
* Verify the cart icon updates accordingly.
* Remove one product from the cart.
4. Checkout
* Proceed to checkout, and add a product from the suggested items list.
* Complete the order and verify the success message.

## Running the Tests
### Run All Tests

Execute the entire test suite using the dotnet test command in the terminal. This will run both Scenario One and Scenario Two tests.

### NOTES

## How I Tested

I used **Visual Studio Community 2022** to execute the test suite. Here's how I ran the tests:

1. **Open the Project** in Visual Studio Community 2022.

2. From the **menu bar at the top**, navigate to: ***Test -> Run All Tests***

3. **Visual Studio** will build the project, discover all the tests, and run them automatically. 

4. You can view the results in the **Test Explorer** window within Visual Studio.

![image](https://github.com/user-attachments/assets/5722abde-8ce4-42b0-a60a-c1f0a195b7c2)


