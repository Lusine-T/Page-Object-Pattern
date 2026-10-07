using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Page_Objects_Pattern.Pages;

namespace Page_Objects_Pattern;

public class InsightsPage : BasePage
{
    private readonly By insightsLink =
        By.LinkText("Insights");

    private readonly By activeSlide =
        By.CssSelector(".owl-item.active[aria-hidden='false']");

    private readonly By nextSlideButton =
        By.CssSelector("button.slider__right-arrow");

    private readonly By carouselTitle =
        By.CssSelector(".single-slide__content .text .font-size-60");

    private readonly By readMoreLink =
        By.CssSelector("a.slider-cta-link");

    private readonly By articleTitle =
        By.CssSelector(".header_and_download h1");

    public InsightsPage(IWebDriver driver, WebDriverWait wait)
        : base(driver, wait)
    {
    }

    public void OpenInsights()
    {
        AcceptCookiesIfPresent();

        IWebElement insights = this.wait.Until(driver =>
        {
            IReadOnlyCollection<IWebElement> elements =
                driver.FindElements(this.insightsLink);

            return elements.FirstOrDefault(
                element => element.Displayed && element.Enabled);
        });

        insights.Click();

        this.wait.Until(driver =>
            driver.Url.Contains("/insights", StringComparison.OrdinalIgnoreCase));
    }

    public void SwipeCarousel(int times)
    {
        if (times < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(times));
        }

        for (int i = 0; i < times; i++)
        {
            string previousTitle = this.GetCurrentArticleTitle();

            TestContext.WriteLine(
                $"BEFORE CLICK {i + 1}: '{previousTitle}'");

            IWebElement nextButton = this.wait.Until(driver =>
            {
                IReadOnlyCollection<IWebElement> elements =
                    driver.FindElements(this.nextSlideButton);

                return elements.FirstOrDefault(
                    element => element.Displayed && element.Enabled);
            });

            nextButton.Click();

            this.WaitForSlideChange(previousTitle);

            string currentTitle = this.GetCurrentArticleTitle();

            TestContext.WriteLine(
                $"AFTER CLICK {i + 1}: '{currentTitle}'");
        }
    }

    public string GetCurrentArticleTitle()
    {
        return this.wait.Until(driver =>
        {
            try
            {
                IWebElement slide = this.GetActiveSlide();

                IWebElement title = slide.FindElement(
                    this.carouselTitle);

                string text = title.Text.Trim();

                return string.IsNullOrWhiteSpace(text)
                    ? null
                    : text;
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        });
    }

    public void ClickReadMore()
    {
        this.wait.Until(driver =>
        {
            try
            {
                IWebElement slide = this.GetActiveSlide();

                IWebElement title = slide.FindElement(
                    this.carouselTitle);

                IWebElement readMore = slide.FindElement(
                    this.readMoreLink);

                if (!title.Displayed ||
                    string.IsNullOrWhiteSpace(title.Text) ||
                    !readMore.Displayed ||
                    !readMore.Enabled)
                {
                    return false;
                }

                readMore.Click();

                return true;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
            catch (ElementClickInterceptedException)
            {
                return false;
            }
            catch (ElementNotInteractableException)
            {
                return false;
            }
        });

        this.wait.Until(driver =>
        {
            IReadOnlyCollection<IWebElement> titles =
                driver.FindElements(this.articleTitle);

            IWebElement? title = titles.FirstOrDefault(
                element => element.Displayed &&
                            !string.IsNullOrWhiteSpace(element.Text));

            return title != null;
        });
    }

    public string GetArticleTitle()
    {
        IWebElement title = this.wait.Until(driver =>
        {
            IReadOnlyCollection<IWebElement> elements =
                driver.FindElements(this.articleTitle);

            return elements.FirstOrDefault(
                element =>
                    element.Displayed &&
                    !string.IsNullOrWhiteSpace(element.Text));
        });

        return title.Text.Trim();
    }

    private IWebElement GetActiveSlide()
    {
        return this.wait.Until(driver =>
        {
            IReadOnlyCollection<IWebElement> elements =
                driver.FindElements(this.activeSlide);

            return elements.FirstOrDefault(
                element => element.Displayed);
        });
    }

    private void WaitForSlideChange(string previousTitle)
    {
        this.wait.Until(driver =>
        {
            try
            {
                IWebElement slide = this.GetActiveSlide();

                IWebElement title = slide.FindElement(
                    this.carouselTitle);

                string currentTitle = title.Text.Trim();

                return !string.IsNullOrWhiteSpace(currentTitle) &&
                       !string.Equals(
                           currentTitle,
                           previousTitle,
                           StringComparison.Ordinal);
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        });
    }
}