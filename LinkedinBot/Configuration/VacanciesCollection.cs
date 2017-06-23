using System.Configuration;

namespace LinkedinBot.Configuration
{
    [ConfigurationCollection(typeof(VacancyElement), AddItemName = "vacancy")]
    public class VacanciesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new VacancyElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((VacancyElement)(element)).Id;
        }

        public VacancyElement this[int idx]
        {
            get { return (VacancyElement)BaseGet(idx); }
        }
    }
}
