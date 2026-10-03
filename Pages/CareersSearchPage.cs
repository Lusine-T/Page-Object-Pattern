using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Page_Objects_Pattern.Pages;

public class CareersSearchPage : BasePage
{
    private readonly By languageField = By.Name("search");
    private readonly By countryField =
        By.CssSelector("input[aria-label='Choose your country']");
    private readonly By searchOverlay =
        By.CssSelector("div.StickySearchBox_overlay___j_z8");

    private readonly By remoteCheckbox =
        By.XPath("//label[.//span[normalize-space()='Remote']]");

    private readonly By searchButton =
        By.CssSelector("button[name='submit_search_box_button']");

    private readonly By nextPageButton =
        By.CssSelector("[data-testid='pagination'] button[aria-label='next page']");

    private readonly By pagination =
        By.CssSelector("[data-testid='pagination']");

    private readonly By resultsInfo =
        By.CssSelector("[role='status']");

    private readonly By jobCards =
        By.CssSelector("[data-testid='job-card-link']");

    private bool overlayWasVisible;

    public CareersSearchPage(IWebDriver driver, WebDriverWait wait)
     : base(driver, wait)
    {
    }

    public void EnterLanguage(string language)
    {
        AcceptCookiesIfPresent();

        IWebElement field = this.wait.Until(
             driver =>
             {
                 IReadOnlyCollection<IWebElement> elements =
                     driver.FindElements(this.languageField);

                 return elements.Count > 0 && elements.First().Displayed
                     ? elements.First()
                     : null;
             });

        field.Clear();
        field.SendKeys(language);
    }

    public void SelectCountry(string country)
    {
        AcceptCookiesIfPresent();

        IWebElement field = this.wait.Until(
            driver =>
            {
                IReadOnlyCollection<IWebElement> elements =
                    driver.FindElements(this.countryField);

                return elements.Count > 0 && elements.First().Displayed
                    ? elements.First()
                    : null;
            });

        field.Click();
        field.SendKeys(country);
    }


    public void WaitForSearchUpdate()
    {
        // waiting for overlay
        try
        {
            this.wait.Until(driver =>
            {
                IReadOnlyCollection<IWebElement> overlays =
                    driver.FindElements(this.searchOverlay);
                return overlays.Any(o => o.Displayed);
            });
        }
        catch (WebDriverTimeoutException)
        {
            // no overlay, may be request is done
        }

        // waiting for overlay dissapears
        this.wait.Until(driver =>
        {
            IReadOnlyCollection<IWebElement> overlays =
                driver.FindElements(this.searchOverlay);
            return overlays.Count == 0 ||
                   overlays.All(o => !o.Displayed);
        });
    }

    public void SelectRemote()
    {
        IWebElement checkbox = this.wait.Until(
            driver =>
            {
                IReadOnlyCollection<IWebElement> elements =
                    driver.FindElements(this.remoteCheckbox);

                return elements.Count > 0 && elements.First().Displayed
                    ? elements.First()
                    : null;
            });

        checkbox.Click();
    }

    public void ClickSearch()
    {
        IWebElement button = this.wait.Until(driver =>
        {
            IReadOnlyCollection<IWebElement> buttons =
                driver.FindElements(this.searchButton);

            return buttons.Count > 0 &&
                   buttons.First().Displayed &&
                   buttons.First().Enabled
                ? buttons.First()
                : null;
        });

        ((IJavaScriptExecutor)this.driver)
            .ExecuteScript("arguments[0].click();", button);
    }

    public void GoToLastPage()
    {
        while (true)
        {
            IWebElement nextButton = this.wait.Until(
                driver =>
                {
                    IReadOnlyCollection<IWebElement> buttons =
                        driver.FindElements(this.nextPageButton);

                    return buttons.Count > 0 && buttons.First().Displayed
                        ? buttons.First()
                        : null;
                });

            string? disabled = nextButton.GetAttribute("disabled");

            if (disabled != null)
            {
                break;
            }

            ((IJavaScriptExecutor)this.driver)
                .ExecuteScript("arguments[0].click();", nextButton);

            this.wait.Until(driver =>
            {
                IReadOnlyCollection<IWebElement> pages =
                    driver.FindElements(this.pagination);

                return pages.Count > 0 && pages.First().Displayed;
            });
        }
    }

    public void OpenLastJob()
    {
        IReadOnlyCollection<IWebElement> cards = this.wait.Until(
            driver =>
            {
                IReadOnlyCollection<IWebElement> elements =
                    driver.FindElements(this.jobCards);

                return elements.Count > 0 ? elements : null;
            });

        cards.Last().Click();
    }

    public string GetResultsInfo()
    {
        return this.wait.Until(driver =>
        {
            IReadOnlyCollection<IWebElement> elements =
                driver.FindElements(this.resultsInfo);

            return elements.Count > 0 && elements.First().Displayed
                ? elements.First().Text
                : null;
        });
    }

    public void WaitForSearchResultsToUpdate(string previousResults)
    {
        this.wait.Until(driver =>
        {
            IReadOnlyCollection<IWebElement> elements =
                driver.FindElements(this.resultsInfo);

            if (elements.Count == 0 || !elements.First().Displayed)
            {
                return false;
            }

            string currentResults = elements.First().Text;

            return currentResults != previousResults;
        });
    }

    //public int GetJobCardsCount()
    //{
    //    // важно: дождаться, что карточки вообще есть
    //    this.wait.Until(driver =>
    //    {
    //        IReadOnlyCollection<IWebElement> cards =
    //            driver.FindElements(this.jobCards);
    //        return cards.Count > 0;
    //    });

    //    return this.driver.FindElements(this.jobCards).Count;
    //}

    //public void WaitForJobCardsCountChangedFrom(int previousCount)
    //{
    //    this.wait.Until(driver =>
    //    {
    //        // сперва дождаться исчезновения оверлея (если он есть)
    //        IReadOnlyCollection<IWebElement> overlays =
    //            driver.FindElements(this.searchOverlay);
    //        if (overlays.Any(o => o.Displayed)) return false;

    //        IReadOnlyCollection<IWebElement> cards =
    //            driver.FindElements(this.jobCards);

    //        // Ждём: либо карточек стало меньше/больше, либо появились/исчезли
    //        return cards.Count > 0 && cards.Count != previousCount;
    //    });
    //}
}
