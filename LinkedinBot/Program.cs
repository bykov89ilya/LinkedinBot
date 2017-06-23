using System.Configuration;
using System.Diagnostics;
using LinkedinBot.Configuration;

namespace LinkedinBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var section = (JobVacanciesConfigSection)ConfigurationManager.GetSection("jobVacancies");
            Debug.WriteLine(section.MakeContact.Text);
            foreach (VacancyElement item in section.VacanciesItems)
            {
                Debug.WriteLine(item.Id);
                foreach (KeyWordElement keyWord in item.keyWords)
                {
                    Debug.WriteLine(keyWord.Word);
                }
            }
        }
    }
}
