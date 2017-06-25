using System.Configuration;

namespace LinkedinBot.Configuration
{
    public class JobVacanciesConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("makeContact")]
        public MakeContactElement MakeContact
        {
            get
            {
                return (MakeContactElement) this["makeContact"];
            }
        }

        [ConfigurationProperty("vacancies")]
        public VacanciesCollection VacanciesItems
        {
            get { return ((VacanciesCollection)(this["vacancies"])); }
        }

        [ConfigurationProperty("regions")]
        public RegionsCollection RegionsItems
        {
            get { return ((RegionsCollection)(this["regions"])); }
        }
    }
}
