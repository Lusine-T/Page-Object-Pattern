using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Page_Objects_Pattern.Pages;

public class HomePage : BasePage
{   
    private readonly By careersLink = By.LinkText("Careers");
    private readonly By searchButton = By.CssSelector("button.header-search__button");
   
    private readonly By careersSearchLink =
        By.CssSelector("a.button-body[href*='/en/jobs/']");

    public HomePage(IWebDriver driver, WebDriverWait wait)
     : base(driver, wait)
    {        
    }

    public void OpenCareers()
    {
        IWebElement careers = wait.Until(
            driver => driver.FindElement(careersLink));

        careers.Click();

        AcceptCookiesIfPresent();
    }

    public void OpenGlobalSearch()
    {
        IWebElement search = wait.Until(
            driver => driver.FindElement(searchButton));

        search.Click();
    }

    //private void AcceptCookiesIfPresent()
    //{
    //    IReadOnlyCollection<IWebElement> buttons =
    //        driver.FindElements(acceptCookiesButton);

    //    if (buttons.Count > 0 && buttons.First().Displayed)
    //    {
    //        buttons.First().Click();
    //    }
    //}
    public void OpenCareersSearch()
    {
        IWebElement searchLink = this.wait.Until(
            driver => driver.FindElement(this.careersSearchLink));

        searchLink.Click();
    }
}