using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Page_Objects_Pattern.Pages;

public class HomePage : BasePage
{   
    private readonly By careersLink = By.LinkText("Careers");
    private readonly By careersSearchLink =
        By.CssSelector("a.button-body[href*='/en/jobs/']");

    private readonly By globalSearchButton =
        By.CssSelector("button.header-search__button");

    private readonly By globalSearchInput =
    By.CssSelector("input#new_form_search");

    private readonly By globalSearchSubmitButton =
        By.CssSelector("button.custom-search-button");

    private readonly By globalSearchResults =
        By.CssSelector("article.search-results__item");

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
        IWebElement searchButton = this.wait.Until(driver =>
        {
            IReadOnlyCollection<IWebElement> elements =
                driver.FindElements(this.globalSearchButton);

            return elements.FirstOrDefault(
                element => element.Displayed && element.Enabled);
        });

        searchButton.Click();

        this.wait.Until(driver =>
        {
            IReadOnlyCollection<IWebElement> buttons =
                driver.FindElements(this.globalSearchButton);

            IWebElement? button = buttons.FirstOrDefault(
                element => element.Displayed);

            return button != null &&
                   button.GetAttribute("aria-expanded") == "true";
        });

        this.wait.Until(driver =>
        {
            IReadOnlyCollection<IWebElement> inputs =
                driver.FindElements(this.globalSearchInput);

            IWebElement? input = inputs.FirstOrDefault();

            return input != null &&
                   input.Displayed &&
                   input.Enabled &&
                   input.Size.Width > 0 &&
                   input.Size.Height > 0;
        });
    }

    public void OpenCareersSearch()
    {
        IWebElement searchLink = this.wait.Until(
            driver => driver.FindElement(this.careersSearchLink));

        searchLink.Click();
    }

    public void SearchGlobal(string searchKeyword)
    {
        AcceptCookiesIfPresent();

        this.wait.Until(driver =>
        {
            IReadOnlyCollection<IWebElement> elements =
                driver.FindElements(this.globalSearchInput);

            IWebElement? input = elements.FirstOrDefault();

            if (input == null ||
                !input.Displayed ||
                !input.Enabled ||
                input.Size.Width == 0 ||
                input.Size.Height == 0)
            {
                return false;
            }

            try
            {
                input.Click();
                input.SendKeys(searchKeyword);

                return input.GetAttribute("value") == searchKeyword;
            }
            catch (WebDriverException)
            {
                return false;
            }
        });

        this.wait.Until(driver =>
        {
            IReadOnlyCollection<IWebElement> elements =
                driver.FindElements(this.globalSearchSubmitButton);

            IWebElement? button = elements.FirstOrDefault(
                element => element.Displayed && element.Enabled);

            if (button == null)
            {
                return false;
            }

            try
            {
                button.Click();
                return true;
            }
            catch (WebDriverException)
            {
                return false;
            }
        });

        this.wait.Until(driver =>
        {
            IReadOnlyCollection<IWebElement> results =
                driver.FindElements(this.globalSearchResults);

            return results.Any(element => element.Displayed);
        });
    }

    
    public IReadOnlyCollection<string> GetGlobalSearchResults()
    {
        IReadOnlyCollection<IWebElement> results =
            this.wait.Until(driver =>
            {
                IReadOnlyCollection<IWebElement> elements =
                    driver.FindElements(this.globalSearchResults);

                return elements.Count > 0 ? elements : null;
            });

        return results
            .Select(result => result.Text)
            .ToList();
    }

}