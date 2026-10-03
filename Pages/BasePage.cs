using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Page_Objects_Pattern.Pages;

public abstract class BasePage
{
    protected readonly IWebDriver driver;
    protected readonly WebDriverWait wait;

    private readonly By acceptCookiesButton =
        By.Id("onetrust-accept-btn-handler");

    protected BasePage(IWebDriver driver, WebDriverWait wait)
    {
        this.driver = driver;
        this.wait = wait;
    }

    protected void AcceptCookiesIfPresent()
    {
        IReadOnlyCollection<IWebElement> buttons =
            this.driver.FindElements(this.acceptCookiesButton);

        if (buttons.Count > 0 && buttons.First().Displayed)
        {
            buttons.First().Click();
        }
    }
}