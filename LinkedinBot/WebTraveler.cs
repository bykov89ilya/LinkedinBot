using System;
using System.Configuration;
using System.Web;
using LinkedinBot.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace LinkedinBot
{
    public class WebTraveler : IDisposable
    {
        public IWebDriver Driver;
        private readonly JobVacanciesConfigSection _jobVacanciesConfigSection;

        public WebTraveler()
        {
            Driver = new ChromeDriver();
            _jobVacanciesConfigSection = (JobVacanciesConfigSection)ConfigurationManager.GetSection("jobVacancies");
        }

        public void Dispose()
        {
            Driver.Dispose();
        }

        public void NavigateLinkedin()
        {
            Driver.Manage().Window.Maximize();
            Driver.Navigate().GoToUrl("https://linkedin.com");
        }

        public void Authorize()
        {
            var emailTextBox = Driver.FindElement(By.Id("login-email"));
            emailTextBox.Click();
            emailTextBox.SendKeys(ConfigurationManager.AppSettings["login"]);

            var passwordTextBox = Driver.FindElement(By.Id("login-password"));
            passwordTextBox.Click();
            passwordTextBox.SendKeys(ConfigurationManager.AppSettings["password"]);
            passwordTextBox.Submit();
        }

        public void GoSearch()
        {
            var pattern = "https://www.linkedin.com/search/results/people/?facetGeoRegion=%5B%22ru%3A{0}%22%5D&facetNetwork=%5B%22S%22%2C%22O%22%5D&keywords={1}&origin=GLOBAL_SEARCH_HEADER";

            // Enumerate regions
            foreach (RegionElement region in _jobVacanciesConfigSection.RegionsItems)
            {
                // Enumerate vacancies
                foreach (VacancyElement vacancy in _jobVacanciesConfigSection.VacanciesItems)
                {
                    Driver.Navigate().GoToUrl(
                        string.Format(pattern, region.Place, HttpUtility.UrlEncode(vacancy.SearchWord)));
                }
            }
        }
    }
}
