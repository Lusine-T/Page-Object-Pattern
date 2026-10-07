using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Page_Objects_Pattern.Pages;

namespace Page_Objects_Pattern
{
    [TestFixture]
    public class Tests
    {

        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;
        private string _downloadDirectory = null!;

        private string WebsiteUrl
        {
            get
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                return configuration["WebsiteUrl"]!;
            }
        }

        [SetUp]
        public void Setup()
        {
            this._downloadDirectory = Path.Combine(
                TestContext.CurrentContext.WorkDirectory,
                 "Downloads");

            Directory.CreateDirectory(this._downloadDirectory);

            ChromeOptions options = new();

            options.AddUserProfilePreference("download.default_directory", this._downloadDirectory);

            options.AddUserProfilePreference("download.prompt_for_download", false);

            options.AddUserProfilePreference("plugins.always_open_pdf_externally", true);

            this._driver = new ChromeDriver(options);

            this._wait = new WebDriverWait(this._driver, TimeSpan.FromSeconds(10));

            this._driver.Manage().Window.Maximize();
        }

        [Test]
        [TestCase("Java", "Armenia")]
        public void Test1(string language, string country)
        {
            this._driver.Navigate().GoToUrl(this.WebsiteUrl);

            HomePage homePage = new HomePage(this._driver, this._wait);

            homePage.OpenCareers();

            homePage.OpenCareersSearch();
            
            CareersSearchPage careersSearchPage = new CareersSearchPage(
                this._driver,
                this._wait);

            careersSearchPage.EnterLanguage(language);

            careersSearchPage.SelectCountry(country);

            careersSearchPage.WaitForSearchUpdate();

            careersSearchPage.SelectRemote();

            careersSearchPage.WaitForSearchUpdate();

            string resultsBeforeSearch = careersSearchPage.GetResultsInfo();

            careersSearchPage.ClickSearch();

            careersSearchPage.WaitForSearchResultsToUpdate(resultsBeforeSearch);

            careersSearchPage.GoToLastPage();

            careersSearchPage.OpenLastJob();


            careersSearchPage.WaitForJobPageLoaded();

            string pageText = careersSearchPage.GetPageText();

            Assert.That(pageText, Does.Contain(language));
        }

        [Test]
        [TestCase("BLOCKCHAIN")]
        [TestCase("Cloud")]
        [TestCase("Automation")]
        public void GlobalSearch_ValidKeyword_SearchResultsContainKeyword(string searchKeyword)
        {
            this._driver.Navigate().GoToUrl(this.WebsiteUrl);

            HomePage homePage = new HomePage(this._driver, this._wait);

            homePage.OpenGlobalSearch();

            homePage.SearchGlobal(searchKeyword);

            IReadOnlyCollection<string> results =
                homePage.GetGlobalSearchResults();

            foreach (string result in results)
            {
                TestContext.WriteLine($"RESULT: '{result}'");
            }

            bool allResultsContainKeyword = results.All(
                result => result.Contains(
                    searchKeyword,
                    StringComparison.OrdinalIgnoreCase));

            Assert.That(allResultsContainKeyword, Is.True);
        }

        [Test]
        [TestCase("Code_of_Ethical_Conduct.pdf")]
        public void DownloadCodeOfConduct_ValidFile_FileDownloaded(string fileName)
        {
            string filePath = Path.Combine(this._downloadDirectory, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            this._driver.Navigate().GoToUrl(this.WebsiteUrl);

            PoliciesPage policiesPage = new PoliciesPage(this._driver, this._wait);

            policiesPage.ScrollToFooter();

            policiesPage.ClickCodeOfConduct();

            this._wait.Until(driver => File.Exists(filePath));

            Assert.That(File.Exists(filePath), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            this._driver.Quit();
            this._driver.Dispose();
        }

        [Test]
        [TestCase(2)]
        public void InsightsCarousel_ArticleTitle_TitleMatches(int swipeCount)
        {
            this._driver.Navigate().GoToUrl(this.WebsiteUrl);

            InsightsPage insightsPage =
                new InsightsPage(this._driver, this._wait);

            insightsPage.OpenInsights();

            insightsPage.SwipeCarousel(swipeCount);

            string carouselTitle =
                insightsPage.GetCurrentArticleTitle();

            TestContext.WriteLine($"Carousel article title: '{carouselTitle}'");

            insightsPage.ClickReadMore();

            string articleTitle =
                insightsPage.GetArticleTitle();

            TestContext.WriteLine($"Article page title: '{articleTitle}'");

            Assert.That(
                articleTitle,
                Is.EqualTo(carouselTitle));
        }
    }
}
