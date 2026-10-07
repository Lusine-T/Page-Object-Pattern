using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Page_Objects_Pattern.Pages;

namespace Page_Objects_Pattern.Pages;
public class PoliciesPage : BasePage
    {
        private readonly By codeOfConductLink =
            By.CssSelector("div.policies a[href*='Code_of_Ethical_Conduct.pdf']");

        private readonly By policiesSection =
            By.CssSelector("div.policies");

        public PoliciesPage(IWebDriver driver, WebDriverWait wait)
            : base(driver, wait)
        {
        }

        public void ScrollToFooter()
        {
            AcceptCookiesIfPresent();
            IWebElement policies = this.wait.Until(
            driver => driver.FindElement(this.policiesSection));

            Actions actions = new(this.driver);

            actions.MoveToElement(policies).ScrollByAmount(0, 500).Perform();


        }

        public void ClickCodeOfConduct()
        {

            IWebElement link = this.wait.Until(driver =>
            {
                IWebElement element = driver.FindElement(
                    this.codeOfConductLink);

                return element.Displayed && element.Enabled
                    ? element
                    : null;
            });

            ((IJavaScriptExecutor)this.driver).ExecuteScript(
                "arguments[0].scrollIntoView({block: 'center'});",
                link);

            IWebElement clickableLink = this.wait.Until(driver =>
            {
                IWebElement element = driver.FindElement(
                    this.codeOfConductLink);

                return element.Displayed && element.Enabled
                    ? element
                    : null;
            });

            clickableLink.Click();
        }
    }