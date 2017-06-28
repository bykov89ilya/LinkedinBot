using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
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

        private void ScrollDown()
        {
            //var elementToScroll = Driver.FindElement(By.XPath("//*[contains(text(),'LinkedIn Corporation')]"));
            var script = String.Format("window.scrollTo({0}, {1})", 0, 12000);
            ((IJavaScriptExecutor)Driver).ExecuteScript(script);
            Thread.Sleep(TimeSpan.FromSeconds(10));
        }

        private void ScrollUp()
        {
            //var elementToScroll = Driver.FindElement(By.XPath("//*[contains(text(),'LinkedIn Corporation')]"));
            var script = String.Format("window.scrollTo({0}, {1})", 0, 0);
            ((IJavaScriptExecutor)Driver).ExecuteScript(script);
            Thread.Sleep(TimeSpan.FromSeconds(10));
        }

        public void GoSearch()
        {
            var pattern = "https://www.linkedin.com/search/results/people/?facetGeoRegion=%5B%22ru%3A{0}%22%5D&facetNetwork=%5B%22S%22%2C%22O%22%5D&keywords={1}&origin=GLOBAL_SEARCH_HEADER&page={2}";

            // Enumerate regions
            foreach (RegionElement region in _jobVacanciesConfigSection.RegionsItems)
            {
                // Enumerate vacancies
                foreach (VacancyElement vacancy in _jobVacanciesConfigSection.VacanciesItems)
                {
                    // Enumerate pages
                    for (int page = 1; page <= 100; page++)
                    {
                        Driver.Navigate().GoToUrl(
                        string.Format(pattern, 
                            region.Place, HttpUtility.UrlEncode(vacancy.SearchWord), page));

                        ScrollDown();

                        var accountLinks = Driver.FindElements(By.XPath("//div[contains(@class,'search-result__info')]/a"));

                        if (accountLinks.Count == 0) break;

                        // Enumerate accounts on page
                        foreach (var link in accountLinks)
                        {
                            try
                            {
                                var buttonMakeContact = link.FindElement(By.XPath("../..")).FindElement(By.ClassName("search-result__actions--primary"));
                                if (!buttonMakeContact.Text.Contains("Установить контакт")) continue;
                            }
                            catch(NotFoundException)
                            {
                                continue;
                            }

                            var mainWindowHandle = Driver.WindowHandles[0];
                            var scriptExecutor = (IJavaScriptExecutor)Driver;
                            scriptExecutor.ExecuteScript(string.Format("window.open('{0}', '_blank');", link.GetAttribute("href")));
                            var newWindowHandle = Driver.WindowHandles[1];
                            Driver.SwitchTo().Window(newWindowHandle);
                            Thread.Sleep(TimeSpan.FromSeconds(30));

                            ScrollDown();

                            var expandSkillsButton = Driver.FindElement(By.XPath("//*[contains(@class,'pv-skills-section__additional-skills')]"));
                            expandSkillsButton.Click();

                            var skills = Driver.FindElements(By.ClassName("pv-skill-entity__skill-name"));
                            var textSkills = new List<string>();
                            foreach (var skill in skills)
                            {
                                textSkills.Add(skill.Text); 
                            }
                            var existKeyWordsOnPage = true;
                            //foreach (KeyWordElement keyWord in vacancy.keyWords)
                            //{
                            //    var words = keyWord.Word.Split(new[] { '|' });
                            //    foreach (var word in words)
                            //    {
                            //        if (textSkills.Contains(word))
                            //        {
                            //            existKeyWordsOnPage = true;
                            //            break;
                            //        }
                            //        else
                            //        {
                            //            existKeyWordsOnPage = false;
                            //        }
                            //    }
                            //    if (!existKeyWordsOnPage) break;
                            //}
                            // If true make contact
                            if (existKeyWordsOnPage)
                            {
                                try
                                {
                                    ScrollUp();

                                    var buttonMakeContact = Driver.FindElement(By.XPath(string.Format("//*[contains(text(), '{0}')]/..", "Установить контакт")));
                                    buttonMakeContact.Click(); 

                                    var buttonPersonalize = Driver.FindElement(By.XPath("//*[.='Персонализировать']"));
                                    buttonPersonalize.Click();

                                    var textAreaMessage = Driver.FindElement(By.Id("custom-message"));
                                    textAreaMessage.SendKeys(_jobVacanciesConfigSection.MakeContact.Text + " " + vacancy.Id);

                                    var buttonInvite = Driver.FindElement(By.XPath(string.Format("//*[contains(text(), '{0}')/..]", "Отправить приглашение")));
                                    buttonInvite.Click();
                                    Thread.Sleep(TimeSpan.FromSeconds(30));
                                }
                                catch(NotFoundException)
                                {
                                    
                                }
                            }
                            Driver.Close();
                            Driver.SwitchTo().Window(mainWindowHandle);
                        }
                    }
                }
            }
        }
    }
}
