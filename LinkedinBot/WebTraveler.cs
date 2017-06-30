using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Web;
using LinkedinBot.Configuration;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace LinkedinBot
{
    public class WebTraveler : IDisposable
    {
        public IWebDriver Driver;
        private readonly JobVacanciesConfigSection _jobVacanciesConfigSection;
        private Logger logger = LogManager.GetCurrentClassLogger();

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
            var script = String.Format("window.scrollTo({0}, {1})", 0, 12000);
            ((IJavaScriptExecutor)Driver).ExecuteScript(script);
            Thread.Sleep(TimeSpan.FromSeconds(10));
        }

        private void ScrollDownBySteps()
        {
            for (int i = 1000; i <= 4000; i = i + 100)
            {
                var script = String.Format("window.scrollTo({0}, {1})", 0, i);
                ((IJavaScriptExecutor)Driver).ExecuteScript(script);
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }

        private void ScrollUp()
        {
            var script = String.Format("window.scrollTo({0}, {1})", 0, 0);
            ((IJavaScriptExecutor)Driver).ExecuteScript(script);
            Thread.Sleep(TimeSpan.FromSeconds(10));
        }

        private void ScrollToCoordinates(int x, int y)
        {
            // on the middle of window
            y = y - 300;
            var script = String.Format("window.scrollTo({0}, {1})", x, y);
            ((IJavaScriptExecutor)Driver).ExecuteScript(script);
            Thread.Sleep(TimeSpan.FromSeconds(10));
        }

        public void GoSearch()
        {
            var pattern = "https://www.linkedin.com/search/results/people/?facetGeoRegion=%5B%22ru%3A{0}%22%5D&facetNetwork=%5B%22S%22%2C%22O%22%5D&keywords={1}&origin=GLOBAL_SEARCH_HEADER&page={2}";

            // Enumerate regions
            foreach (RegionElement region in _jobVacanciesConfigSection.RegionsItems)
            {
                logger.Trace("Исследуем регион: {0}", region.Place);

                // Enumerate vacancies
                foreach (VacancyElement vacancy in _jobVacanciesConfigSection.VacanciesItems)
                {
                    logger.Trace("Ищем людей на вакансию {0} c id {1}", vacancy.Description, vacancy.Id);

                    // Enumerate pages
                    for (int page = 1; page <= 100; page++)
                    {
                        var urlToAnalyse = string.Format(pattern,
                            region.Place, HttpUtility.UrlEncode(vacancy.SearchWord), page);

                        Driver.Navigate().GoToUrl(urlToAnalyse);

                        logger.Trace("Анализируем страницу {0}, переходя по url {1}", page, urlToAnalyse);

                        ScrollDown();

                        var accountLinks = Driver.FindElements(By.XPath("//div[contains(@class,'search-result__info')]/a"));

                        logger.Trace("Количество аккаунтов на странице {0}", accountLinks.Count);

                        if (accountLinks.Count == 0) break;

                        // Enumerate accounts on page
                        foreach (var link in accountLinks)
                        {
                            logger.Trace("Анализируем ссылку на аккаунт {0}", link.Text);

                            try
                            {
                                var buttonMakeContact = link.FindElement(By.XPath("../..")).FindElement(By.ClassName("search-result__actions--primary"));
                                if (!buttonMakeContact.Text.Contains("Установить контакт"))
                                {
                                    logger.Trace(
                                        "Справа от ссылки на аккаунт нет кнопки \"Установить контакт\", текст на кнопке \"{0}\"", buttonMakeContact.Text);
                                    continue;
                                }
                                else
                                {
                                    logger.Trace("Анализируем страницу по ссылке на аккаунт {0}", link.Text);
                                }
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

                            ScrollDownBySteps();

                            var expandSkillsButton = Driver.FindElement(By.XPath("//*[contains(@class,'pv-skills-section__additional-skills')]"));
                            ScrollToCoordinates(
                                x: expandSkillsButton.Location.X,
                                y: expandSkillsButton.Location.Y);
                            expandSkillsButton.Click();

                            var skills = Driver.FindElements(By.ClassName("pv-skill-entity__skill-name"));
                            var textSkills = new List<string>();
                            foreach (var skill in skills)
                            {
                                logger.Trace("Найден скилл {0}", skill.Text);
                                textSkills.Add(skill.Text); 
                            }
                            var existKeyWordsOnPage = true;
                            foreach (KeyWordElement keyWord in vacancy.keyWords)
                            {
                                logger.Trace("Ищем слово(а) в скиллах {0}", keyWord.Word);
                                var words = keyWord.Word.Split(new[] { '|' });
                                foreach (var word in words)
                                {
                                    if (textSkills.Contains(word))
                                    {
                                        existKeyWordsOnPage = true;
                                        break;
                                    }
                                    else
                                    {
                                        existKeyWordsOnPage = false;
                                    }
                                }
                                if (!existKeyWordsOnPage) break;
                            }
                            // If true make contact
                            if (existKeyWordsOnPage)
                            {
                                try
                                {
                                    logger.Trace("Пытаемся установить контакт");

                                    ScrollUp();

                                    logger.Trace("Ищем кнопку \"Установить контакт\"");

                                    var buttonMakeContact = Driver.FindElement(By.XPath(string.Format("//*[contains(text(), '{0}')]/..", "Установить контакт")));
                                    buttonMakeContact.Click();

                                    Thread.Sleep(TimeSpan.FromSeconds(10));

                                    ScreenshotHelper.Make(vacancy.Id, "makeContact");

                                    logger.Trace("Ищем кнопку \"Персонализировать\"");

                                    var buttonPersonalize = Driver.FindElement(By.XPath("//*[contains(@class,'send-invite__actions')]/button[contains(@class,'button-secondary-large')]"));
                                    buttonPersonalize.Click();

                                    Thread.Sleep(TimeSpan.FromSeconds(10));

                                    ScreenshotHelper.Make(vacancy.Id, "Personalize");

                                    logger.Trace("Ищем поле для ввода текста приглашения");

                                    var textAreaMessage = Driver.FindElement(By.Id("custom-message"));
                                    textAreaMessage.SendKeys(_jobVacanciesConfigSection.MakeContact.Text + " " + vacancy.Id);

                                    ScreenshotHelper.Make(vacancy.Id, "InputInTextArea");

                                    logger.Trace("Ищем кнопку \"Отправить\"");

                                    var buttonInvite = Driver.FindElement(By.XPath("//*[contains(@class,'send-invite__actions')]/button[contains(@class,'ml3')]"));
                                    buttonInvite.Click();
                                    Thread.Sleep(TimeSpan.FromSeconds(30));

                                    ScreenshotHelper.Make(vacancy.Id, "SentInvite");

                                    logger.Trace("Контакт установлен");
                                }
                                catch (NotFoundException ex)
                                {
                                    logger.Error("Возникла ошибка при установлении контакта: {0}",
                                        ex.Message);
                                }
                            }
                            else
                            {
                                logger.Trace("Страница не содержит нужные скиллы для вакансии");
                            }
                            logger.Trace("Закрываем вкладку, возращаемся к основному списку");
                            Driver.Close();
                            Driver.SwitchTo().Window(mainWindowHandle);
                        }
                    }
                }
            }
        }
    }
}
